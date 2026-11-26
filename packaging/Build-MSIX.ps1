[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$NativeOutputDirectory,
    [Parameter(Mandatory)][ValidateSet('x64', 'arm64')][string]$Architecture,
    [Parameter(Mandatory)][string]$OutputDirectory,
    [string]$Publisher,
    [string]$MakeAppx,
    [switch]$TestSign
)

$ErrorActionPreference = 'Stop'

if ($Publisher -and $TestSign) {
    throw 'Use -Publisher for distribution signing or -TestSign for a local test package.'
}

if (!$MakeAppx) {
    $MakeAppx = (Get-Command makeappx.exe -ErrorAction SilentlyContinue).Source
}
if (!$MakeAppx) {
    $kits = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
    $sdkDirectories = Get-ChildItem -LiteralPath $kits -Directory |
        Where-Object Name -Match '^\d+\.\d+\.\d+\.\d+$' |
        Sort-Object { [version]$_.Name } -Descending
    $MakeAppx = $sdkDirectories | ForEach-Object {
        Join-Path $_.FullName 'x64\makeappx.exe'
    } | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
}
if (!$MakeAppx) {
    throw 'MakeAppx.exe was not found. Install the Windows SDK or supply -MakeAppx.'
}

$nativeDirectory = (Resolve-Path -LiteralPath $NativeOutputDirectory).Path
$architectureName = $Architecture.ToLowerInvariant()
$expectedMachine = @{ x64 = 0x8664; arm64 = 0xaa64 }[$architectureName]
$binaries = 'Sonnenberg.exe', 'SonnenbergExplorerCommand.dll'
foreach ($binary in $binaries) {
    $reader = [IO.BinaryReader]::new([IO.File]::OpenRead((Join-Path $nativeDirectory $binary)))
    try {
        $reader.BaseStream.Position = 0x3c
        $reader.BaseStream.Position = $reader.ReadInt32()
        if ($reader.ReadUInt32() -ne 0x4550 -or $reader.ReadUInt16() -ne $expectedMachine) {
            throw "$binary does not match the requested $architectureName architecture."
        }
    } finally {
        $reader.Dispose()
    }
}

$output = (New-Item -ItemType Directory -Path $OutputDirectory -Force).FullName
[xml]$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'AppxManifest.template.xml') -Raw
$manifest.Package.Identity.ProcessorArchitecture = $architectureName
if ($Publisher) {
    $manifest.Package.Identity.Publisher = $Publisher
} elseif ($TestSign) {
    $manifest.Package.Identity.Publisher = 'CN=Sonnenberg Explorer Test'
}
$manifestPath = Join-Path $output 'AppxManifest.xml'
$manifest.Save($manifestPath)

$mapping = @('[Files]', ('"{0}" "AppxManifest.xml"' -f $manifestPath))
foreach ($binary in $binaries) {
    $mapping += '"{0}" "{1}"' -f (Join-Path $nativeDirectory $binary), $binary
}
foreach ($asset in 'StoreLogo.png', 'Square44x44Logo.png', 'Square150x150Logo.png',
    'Sonnenberg.ico', 'CopyPath.ico', 'CopyForwardPath.ico') {
    $mapping += '"{0}" "Assets\{1}"' -f (Join-Path $PSScriptRoot "Assets\$asset"), $asset
}
$mappingPath = Join-Path $output 'msix-files.txt'
Set-Content -LiteralPath $mappingPath -Value $mapping -Encoding Unicode

$packagePath = Join-Path $output "Sonnenberg-Explorer-$architectureName.msix"
& $MakeAppx pack /f $mappingPath /p $packagePath /o
if ($LASTEXITCODE -ne 0) {
    throw "MakeAppx failed with exit code $LASTEXITCODE."
}

if ($TestSign) {
    $signTool = Join-Path (Split-Path -Parent $MakeAppx) 'signtool.exe'
    if (!(Test-Path -LiteralPath $signTool -PathType Leaf)) {
        throw 'SignTool.exe was not found next to MakeAppx.exe in the Windows SDK.'
    }
    $key = [Security.Cryptography.RSACng]::new(2048)
    $subject = [Security.Cryptography.X509Certificates.X500DistinguishedName]::new(
        $manifest.Package.Identity.Publisher)
    $request = [Security.Cryptography.X509Certificates.CertificateRequest]::new(
        $subject, $key, [Security.Cryptography.HashAlgorithmName]::SHA256,
        [Security.Cryptography.RSASignaturePadding]::Pkcs1)
    $usage = [Security.Cryptography.OidCollection]::new()
    $null = $usage.Add([Security.Cryptography.Oid]::new('1.3.6.1.5.5.7.3.3'))
    $request.CertificateExtensions.Add(
        [Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension]::new($usage, $false))
    $request.CertificateExtensions.Add(
        [Security.Cryptography.X509Certificates.X509KeyUsageExtension]::new('DigitalSignature', $true))
    $certificate = $request.CreateSelfSigned([DateTimeOffset]::Now.AddMinutes(-10), [DateTimeOffset]::Now.AddYears(1))
    $pfxPath = Join-Path $output ".sonnenberg-test-$([guid]::NewGuid()).pfx"
    $password = [guid]::NewGuid().ToString('N')
    try {
        [IO.File]::WriteAllBytes($pfxPath, $certificate.Export('Pfx', $password))
        & $signTool sign /fd SHA256 /f $pfxPath /p $password $packagePath
        if ($LASTEXITCODE -ne 0) {
            throw "SignTool failed with exit code $LASTEXITCODE."
        }
        $certificatePath = [IO.Path]::ChangeExtension($packagePath, '.cer')
        [IO.File]::WriteAllBytes($certificatePath, $certificate.Export('Cert'))
        Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Install-Sonnenberg.ps1') -Destination $output -Force
    } finally {
        $certificate.Dispose()
        $key.Dispose()
        if (Test-Path -LiteralPath $pfxPath) {
            Remove-Item -LiteralPath $pfxPath
        }
    }
}

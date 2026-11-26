#Requires -Version 5.1
[CmdletBinding(SupportsShouldProcess)]
param(
    [string]$PackageDirectory
)

$ErrorActionPreference = 'Stop'

# Read the machine architecture even when PowerShell runs under emulation.
$nativeArchitecture = Get-ItemPropertyValue `
    -LiteralPath 'HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager\Environment' `
    -Name PROCESSOR_ARCHITECTURE
$architecture = @{ AMD64 = 'x64'; ARM64 = 'arm64' }[$nativeArchitecture]
if (!$architecture) {
    throw "Unsupported Windows architecture: $nativeArchitecture. Sonnenberg requires x64 or ARM64."
}

$packageName = "Sonnenberg-Explorer-$architecture"
if (!$PackageDirectory) {
    $PackageDirectory = $PSScriptRoot
    if (!(Test-Path -LiteralPath (Join-Path $PackageDirectory "$packageName.msix") -PathType Leaf)) {
        $PackageDirectory = Join-Path $PSScriptRoot "..\Installer\obj\native-$architecture\msix"
    }
}

$packagePath = Join-Path $PackageDirectory "$packageName.msix"
$certificatePath = Join-Path $PackageDirectory "$packageName.cer"
foreach ($path in $packagePath, $certificatePath) {
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Missing file: $path. Supply the matching MSIX and CER from the same build with -PackageDirectory."
    }
}

$packagePath = (Resolve-Path -LiteralPath $packagePath).Path
$certificatePath = (Resolve-Path -LiteralPath $certificatePath).Path
$certificate = [Security.Cryptography.X509Certificates.X509Certificate2]::new($certificatePath)
try {
    $signature = Get-AuthenticodeSignature -LiteralPath $packagePath
    if ($certificate.Subject -ne 'CN=Sonnenberg Explorer Test' -or
        !$signature.SignerCertificate -or $signature.SignerCertificate.Thumbprint -ne $certificate.Thumbprint) {
        throw 'The MSIX signature does not match the supplied Sonnenberg test certificate.'
    }

    if (!$PSCmdlet.ShouldProcess($packagePath, "Trust certificate $($certificate.Thumbprint) and install the $architecture MSIX")) {
        return
    }
    $principal = [Security.Principal.WindowsPrincipal]::new([Security.Principal.WindowsIdentity]::GetCurrent())
    if (!$principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw 'Run this script from Windows PowerShell as Administrator.'
    }
    Get-Command Add-AppxPackage -ErrorAction Stop | Out-Null

    $store = [Security.Cryptography.X509Certificates.X509Store]::new('TrustedPeople', 'LocalMachine')
    $addedCertificate = $false
    try {
        $store.Open('ReadWrite')
        if ($certificate.Thumbprint -notin $store.Certificates.Thumbprint) {
            $store.Add($certificate)
            $addedCertificate = $true
        }

        $signature = Get-AuthenticodeSignature -LiteralPath $packagePath
        if ($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Thumbprint -ne $certificate.Thumbprint) {
            throw "Windows could not validate the MSIX signature: $($signature.StatusMessage)"
        }

        Add-AppxPackage -Path $packagePath
        Write-Output "Sonnenberg Explorer ($architecture) is installed."
    } catch {
        if ($addedCertificate) {
            $store.Remove($certificate)
        }
        throw
    } finally {
        $store.Dispose()
    }
} finally {
    $certificate.Dispose()
}

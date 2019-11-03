
# Retrieves the user name for use as copyright information in the AssemblyInfo file
$UserName = Read-Host -Prompt "Please enter your name, which will be used as copyright information on each assembly file (or leave blank)"
$ManufacturerName = Read-Host -Prompt "Please enter a manufacturer name that will be added to the Product.wxs file (required)"

Write-Output "`nUser Name: $UserName"
Write-Output "Manufacturer Name: $ManufacturerName`n"

$AssemblyInfoFiles = Get-ChildItem $PSScriptRoot -Filter AssemblyInfo.cs -Recurse | % { $_.FullName }
<#foreach($AssemblyInfoFile in $AssemblyInfoFiles){
    Write-Output "AssemblyInfo files: $AssemblyInfoFile"
}#>

$ProductFile = Get-ChildItem $PSScriptRoot -Filter Product.wxs -Recurse | % { $_.FullName }

# Path to VsDevCmd.bat
$DevCmd = Get-ChildItem "C:\Program Files (x86)\Microsoft Visual Studio\" -Filter VsDevCmd.bat -Recurse | Select-Object -First 1 | % { $_.FullName }
# Path to Sn.exe
$Sn = Get-ChildItem "C:\Program Files (x86)\Microsoft SDKs\Windows\" -Filter Sn.exe -Recurse | Select-Object -First 1 | % { $_.FullName }
Write-Output "DevCmd: $DevCmd`nSn.exe: $Sn`n"

# Collect all project folder paths
$AssemblyFolders = Get-ChildItem $PSScriptRoot -Filter *.csproj -Recurse | % { $_.DirectoryName }
# Collect the product folder path
$ProductFolder = Get-ChildItem $PSScriptRoot -Filter *.wixproj -Recurse | % { $_.DirectoryName }
[array]$AssemblyFoldersToSign = [array]$AssemblyFolders + [array]$ProductFolder


function AddUserName{
    # Apply user name To AssemblyInfo.cs Files
    Write-Output "Start adding user name to AssemblyInfo.cs files:"
    foreach($AssemblyInfoFile in $AssemblyInfoFiles){
        (Get-Content $AssemblyInfoFile) | ForEach-Object { $_ -replace 'YOUR_NAME_HERE', $UserName } | Set-Content $AssemblyInfoFile
        Write-Output "Applied user name to: $AssemblyInfoFile"
    }
    Write-Output "End adding user name to AssemblyInfo.cs files.`n"
}

function AddManufacturerName {
    <#if($ManufacturerName.Length == 0){
        $ManufacturerName = "YOUR_NAME_HERE"
    }#>
    # Apply manufacturer name To AssemblyInfo.cs Files
    Write-Output "Start adding manufacturer name to AssemblyInfo.cs files:"
    foreach($Product in $ProductFile){
        (Get-Content $Product) | ForEach-Object { $_ -replace 'YOUR_MANUFACTURER_NAME_HERE', $ManufacturerName } | Set-Content $Product
        Write-Output "Applied manufacturer name to: $Product"
    }
    Write-Output "End adding manufacturer name to AssemblyInfo.cs files.`n"
}

function AddAssemblyInfoGuids {
    #[array]$FilesToGuid = [array]$AssemblyInfoFiles + [array]$ProductFile
    #PAUSE
    # Apply GUIDs To AssemblyInfo.cs Files
    Write-Output "Start applying GUID to AssemblyInfo.cs files:"
    foreach($AssemblyInfoFile in $AssemblyInfoFiles){
        (Get-Content $AssemblyInfoFile) | ForEach-Object { $_ -replace 'YOUR_GUID_HERE', [guid]::NewGuid().toString().ToUpper() } | Set-Content $AssemblyInfoFile
        Write-Output "Applied GUID to: $AssemblyInfoFile"
    }
    Write-Output "End applying GUID to AssemblyInfo.cs files.`n"
}

function AddProductWxsGuids {
    # Apply GUIDs To Product.wxs File
    Write-Output "Start applying GUIDs to Product.wxs file:"
    foreach($FileToGuid in $ProductFile){
        (Get-Content $FileToGuid) | ForEach-Object { $_ -replace 'YOUR_GUID_HERE', [guid]::NewGuid().toString().ToUpper() } | Set-Content $FileToGuid
        Write-Output "Applied GUID to: $FileToGuid\:`n $FileToGuid"
    }
    Write-Output "End applying GUIDs to Product.wxs file.`n"
}

# Sign Assemblies With A Strong Name
function SingAssemblies {
    # Sign Assemblies With A Strong Name
    # Collect all project folder paths
    $AssemblyFolders = Get-ChildItem $PSScriptRoot -Filter *.csproj -Recurse | % { $_.DirectoryName }
    # Collect the product folder path
    $ProductFolder = Get-ChildItem $PSScriptRoot -Filter *.wixproj -Recurse | % { $_.DirectoryName }
    [array]$AssemblyFoldersToSign = [array]$AssemblyFolders + [array]$ProductFolder
    Write-Output "Start signing assemblies:"
    foreach($AssemblyFolderToSign in $AssemblyFoldersToSign) {
        # Creates a s clean key file name
        $KeyFileName = $AssemblyFolderToSign.Replace($PSScriptRoot, '')
        $KeyFileName = $KeyFileName.Replace('\IntegrationTests', '')
        $KeyFileName = $KeyFileName.Replace('\UnitTests', '')
        $KeyFileName = $KeyFileName.Replace('\', '')
        $CleanFileName = $KeyFileName.Replace('.Tests', '')
        ### Creates Key File (signs the assembly)
        $file = "$AssemblyFolderToSign\$CleanFileName.snk"
        cmd /c ""$Sn -k $file""
        Write-Output "Signed assembly with a stromg name: '$AssemblyFolderToSign'`nKey file: $file"
    }
    Write-Output "End signing assemblies.`n`n"
}

function Main {
    
    AddUserName
    AddManufacturerName
    AddAssemblyInfoGuids
    AddProductWxsGuids
    SingAssemblies
    
    Write-Output "ALL TASKS COMPLETED"
    PAUSE
    exit
}

Main
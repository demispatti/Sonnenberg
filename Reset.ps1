class ResetSolution {
	
	[String]$userName = 'YOUR_NAME_HERE'
	[String]$manufacturerName = 'YOUR_MANUFACTURER_NAME_HERE'
	[String]$guidText = 'YOUR_GUID_HERE'
	[Array]$assemblyInfoFiles
	[Array]$assemblyFolders
	[String]$productFile
	[String]$year = $this.GetYear()
	[String]$copyright = "Copyright ©"
	
	# Default Constructor
	ResetSolution()
	{}
	
	# Collect all project folder paths
	RetrieveAssemblyFileFoldersList()
	{
		$this.assemblyFolders = Get-ChildItem $PSScriptRoot -Filter AssemblyInfo.cs -Recurse | ForEach-Object { $_.DirectoryName }
	}
	
	# Collect all assembly info files
	RetrieveAssemblyInfoFilesList()
	{
		$this.assemblyInfoFiles = Get-ChildItem $PSScriptRoot -Filter AssemblyInfo.cs -Recurse | ForEach-Object { $_.FullName }
	}
	
	# Set the product file
	RetrieveProductFile()
	{
		$this.productFile = Get-ChildItem $PSScriptRoot -Filter Product.wxs -Recurse | ForEach-Object { $_.FullName }
	}

	
	[String]GetYear()
	{
		return Get-Date -f yyyy
	}
	
	
	ResetUserNameInAssemblyInfoFiles()
	{
		Write-Host "`r`nStart resetting user name in AssemblyInfo.cs files:"
		$text = $this.copyright + " " + $this.userName + " " + $this.year
		foreach ($assemblyInfoFile in $this.assemblyInfoFiles)
		{
			(Get-Content $assemblyInfoFile) | ForEach-Object { $_ -replace '(?<Item1>AssemblyCopyright\(\").*(?<Item2>\"\))', '${Item1}COPYRIGHT_USERNAME_YEAR${Item2}' } | ForEach-Object { $_ -replace 'COPYRIGHT_USERNAME_YEAR', $text } | Set-Content $assemblyInfoFile
			Write-Host "Resetted user name in:" $assemblyInfoFile "`n"
		}
		Write-Host "End resetting user name in AssemblyInfo.cs files."
	}
	
	
	ResetGuidInAssemblyInfoFiles()
	{
		Write-Host "`r`nStart resetting GUID in AssemblyInfo.cs files:"
		
		foreach ($assemblyInfoFile in $this.assemblyInfoFiles)
		{
			(Get-Content $assemblyInfoFile) | ForEach-Object { $_ -replace '(?<Item1>Guid\(\").*(?<Item2>\"\))', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $this.guidText } | Set-Content $assemblyInfoFile
			Write-Host "Resetted GUID in:" $assemblyInfoFile
		}
		Write-Host "End resetting GUID in AssemblyInfo.cs files."
	}
	
	
	ResetManufacturerNameInProductWxsFile()
	{
		Write-Host "`r`nStart resetting manufacturer name in Product.wxs file:"
		
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>Manufacturer=\").*(?<Item2>\")', '${Item1}MANUFACTURERNAME${Item2}' } | ForEach-Object { $_ -replace 'MANUFACTURERNAME', $this.manufacturerName } | Set-Content $this.productFile
		Write-Host "Resetted manufacturer name in:" $this.productFile "`n"
		
		Write-Host "End resetting manufacturer name in Product.wxs file."
	}
	
	
	ResetProductCodesAndUpgradeCodesInProductWxsFile()
	{
		Write-Host "`r`nStart resetting product codes and upgrade codes in Product.wxs file:"
		
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>ProductCode = \{).*(?<Item2>\})', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $this.guidText } | Set-Content $this.productFile
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>UpgradeCode = \{).*(?<Item2>\})', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $this.guidText } | Set-Content $this.productFile
		
		Write-Host "Resetted product codes and upgrade codes in:" $this.productFile
		
		Write-Host "End resetting product codes and upgrade codes in Product.wxs file."
	}
	
	
	ResetGuidInProductWxsFile()
	{
		
		Write-Host "`r`nStart resetting GUIDs in Product.wxs file:"
		$text = "{" + $this.guidText + "}"
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>Guid=\").*(?<Item2>\")', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $text } | Set-Content $this.productFile
		Write-Host "Resetted GUIDs in:" $this.productFile
		
		Write-Host "End resetting GUIDs in Product.wxs file."
	}

	
	RunConfigurationTasks()
	{
		$this.RetrieveAssemblyFileFoldersList()
		$this.RetrieveAssemblyInfoFilesList()
		$this.RetrieveProductFile()
	}
	
	
	RunResetTasks()
	{
		$this.ResetUserNameInAssemblyInfoFiles()
		$this.ResetGuidInAssemblyInfoFiles()
		$this.ResetManufacturerNameInProductWxsFile()
		$this.ResetProductCodesAndUpgradeCodesInProductWxsFile()
		$this.ResetGuidInProductWxsFile()
	}
	
	
	Run()
	{
		$this.RunConfigurationTasks()
		$this.RunResetTasks()
		
		Write-Host "`r`nALL TASKS COMPLETED"
		PAUSE
		exit
	}
	
}

# Creates new instance of class
$Obj = New-Object -TypeName ResetSolution

$Obj.Run()

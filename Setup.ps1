class SetupSolution {
	
	[String]$userName
	[String]$manufacturerName
	[Array]$assemblyInfoFiles
	[Array]$assemblyFolders
	[String]$productFile
	[Bool]$isInvalid = $true
	[String]$pattern = "[\w]"
	[String]$range = "{0,120}"
	[String]$year = $this.GetYear()
	[String]$copyright = "Copyright ©"

	# Default Constructor
	SetupSolution()
	{}

	# Set the user name
	RetrieveUserName()
	{
		[String]$userInput = ""
		while ($this.isInvalid)
		{
			$userInput = Read-Host -Prompt "Please enter your name. Use alphanumeric symbols only. If left blank, it will default to John Doe."
			$userInput = $this.PreValidateUserInput($userInput)
			if ($this.isInvalid)
			{
				$this.isInvalid = $this.ValidateUserInput($userInput)
			}
		}
		
		$this.userName = $userInput
	}
	
	# Set the manufacturer name
	RetrieveManufacturerName()
	{
		[String]$userInput = ""
		$this.isInvalid = $true
		while ($this.isInvalid)
		{
			$userInput = Read-Host -Prompt "Please enter a manufacturer name. Use alphanumeric symbols only. If left blank, it will default to John Doe."
			$userInput = $this.PreValidateUserInput($userInput)
			if ($this.isInvalid)
			{
				$this.isInvalid = $this.ValidateUserInput($userInput)
			}
		}

		$this.manufacturerName = $userInput
	}
	
	
	[String]PreValidateUserInput($userInput)
	{
		if ($userInput.Length -eq 0)
		{
			$userInput = "John Doe"
			$this.isInvalid = $false
		}
		
		return $userInput
	}
	
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
	
	
	[string]GetGuid($brackets = "")
	{
		$guid = [guid]::NewGuid().toString().ToUpper()
		if ($brackets -eq "curlyBrackets")
		{
			return "{" + $guid + "}"
		}
		else
		{
			return $guid
		}
	}

	# Validates the user input
	[Bool]ValidateUserInput($userInput)
	{
		while ($userInput -notmatch $this.pattern)
		{
			return $true
		}
		
		return $false
	}
	
	
	AddUserNameToAssemblyInfoFiles()
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
	
	
	AddManufacturerNameToProductWxsFile()
	{
		Write-Host "`r`nStart resetting manufacturer name in Product.wxs file:"
		
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>Manufacturer=\").*(?<Item2>\")', '${Item1}MANUFACTURERNAME${Item2}' } | ForEach-Object { $_ -replace 'MANUFACTURERNAME', $this.manufacturerName } | Set-Content $this.productFile
		Write-Host "Resetted manufacturer name in:" $this.productFile "`n"
		
		Write-Host "End resetting manufacturer name in Product.wxs file."
	}
	
	
	AddGuidsToAssemblyInfoFiles()
	{
		Write-Output "Start applying GUID to AssemblyInfo.cs files:"
		foreach ($assemblyInfoFile in $this.assemblyInfoFiles)
		{
			(Get-Content $assemblyInfoFile) | ForEach-Object { $_ -replace 'YOUR_GUID_HERE', $this.GetGuid("") } | Set-Content $assemblyInfoFile
			Write-Output "Applied GUID to: $assemblyInfoFile"
		}
		Write-Output "End applying GUID to AssemblyInfo.cs files.`n"
	}
	
	
	AddProductCodesAndUpgradeCodesToProductWxsFile()
	{
		Write-Host "`r`nStart resetting product codes and upgrade codes in Product.wxs file:"
		
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>ProductCode = \{).*(?<Item2>\})', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $this.GetGuid("") } | Set-Content $this.productFile
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '(?<Item1>UpgradeCode = \{).*(?<Item2>\})', '${Item1}GUIDSTRING${Item2}' } | ForEach-Object { $_ -replace 'GUIDSTRING', $this.GetGuid("") } | Set-Content $this.productFile
		
		Write-Host "Resetted product codes and upgrade codes in:" $this.productFile
		
		Write-Host "End resetting product codes and upgrade codes in Product.wxs file."
	}
	
	
	AddGuidsToProductWxsFile()
	{
		
		Write-Output "Start applying GUIDs to Product.wxs file:"
		(Get-Content $this.productFile) | ForEach-Object { $_ -replace '{YOUR_GUID_HERE}', $this.GetGuid("curlyBrackets") } | Set-Content $this.productFile
		Write-Output "Applied GUID to: $this.productFile\:`n"
		Write-Output "End applying GUIDs to Product.wxs file.`n"#>
	}
	
	
	RunConfigurationTasks()
	{
		$this.RetrieveUserName()
		$this.RetrieveManufacturerName()
		$this.RetrieveAssemblyFileFoldersList()
		$this.RetrieveAssemblyInfoFilesList()
		$this.RetrieveProductFile()
	}
	
	
	RunSetupTasks()
	{
		$this.AddUserNameToAssemblyInfoFiles()
		$this.AddManufacturerNameToProductWxsFile()
		$this.AddGuidsToAssemblyInfoFiles()
		$this.AddProductCodesAndUpgradeCodesToProductWxsFile()
		$this.AddGuidsToProductWxsFile()
	}
	
	
	Run()
	{
		$this.RunConfigurationTasks()
		$this.RunSetupTasks()
		
		Write-Host "`r`nALL TASKS COMPLETED"
		PAUSE
		exit
	}
	
}

# Creates new instance of class
$Obj = New-Object -TypeName SetupSolution

$Obj.Run()

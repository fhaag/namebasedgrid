function getFileVersion {
	param(
		[Parameter(Mandatory=$true)][string]$path,
		$incBuild = $false,
		$silent = $true
	)
	
	[string]$result = $null
	
	if (-not $silent) {
		Write-Host -ForegroundColor Cyan "Retrieving version of $path ..."
	}
	
	if (Test-Path "$path") {
		$previousVerInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$dllOutputPath")
		if ($previousVerInfo.FileVersion) {
			if (-not $silent) {
				Write-Host -ForegroundColor Red $previousVerInfo.FileVersion
			}
			
			[System.Version]$previousVer = $null
			if ([System.Version]::TryParse($previousVerInfo.FileVersion, [ref]$previousVer)) {
				if (-not $silent) {
					Write-Host -ForegroundColor Yellow $previousVer
				}
				
				if ($incBuild) {
					$result = "$($previousVer.Major).$($previousVer.Minor).$($previousVer.Build).$($previousVer.Revision + 1)"
				} else {
					$result = $previousVer
				}
				
				if (-not $silent) {
					Write-Host -ForegroundColor Green "Resultin file version: $result"
				}
			}
		}
	}
	
	return $result
}
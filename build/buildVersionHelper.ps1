function getFileBuildVersion {
	param(
		[Parameter(Mandatory=$true)][string]$path,
		$silent = $true
	)
	
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
				
				return $previousVer.Revision
			}
		}
	}
	
	return $null
}
# ------------------------------------------------------------------------------
# This source file is a part of Name-Based Grid.
# 
# Copyright (c) 2024 Florian Haag
# 
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
# 
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
# ------------------------------------------------------------------------------

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
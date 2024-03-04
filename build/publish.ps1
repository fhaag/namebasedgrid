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

Param(
  [Parameter(Mandatory=$true)][String]$version
)

# check version format
if (-not ($version -match '^[0-9]+\.[0-9]+\.[0-9]+$')) {
	Write-Host -ForegroundColor Red "$version is not a basic three-component version in major.minor.patch format."
	exit 5
}

# paths

$scriptDir = $PSScriptRoot
$tmpDir = [System.IO.Path]::Combine("$scriptDir", '..', 'pubinfo', '_tmp')

# check files to release

$srcPath = Join-Path "$tmpDir" "NameBasedGrid-$version-src.tar.gz"
$binPath = Join-Path "$tmpDir" "NameBasedGrid-$version-bin.tar.gz"
$nupkgPath = Join-Path "$tmpDir" "NameBasedGrid.$version.nupkg"

foreach ($fn in @($srcPath, $binPath, $nupkgPath)) {
	if (-not (Test-Path "$fn")) {
		Write-Host -ForegroundColor Red "File not found: $fn"
		exit 10
	}
}

# retrieve Git revision
$rev = &git rev-parse HEAD

# Github

Write-Host -ForegroundColor Cyan 'Publishing GitHub release ...'

$gitHubSettings = @{
	Owner = 'fhaag';
	Project = 'namebasedgrid';
	ReleaseName = "v$version";
}

Import-Module PowerShellForGitHub

Write-Host -ForegroundColor DarkYellow "Checking release number $($gitHubSettings.ReleaseName) ..."
$existingReleases = Get-GitHubRelease -OwnerName "$($gitHubSettings.Owner)" -RepositoryName "$($gitHubSettings.Project)" -Tag "$($gitHubSettings.ReleaseName)" -ErrorAction:Ignore

if ($existingReleases.Count -ge 1) {
	Write-Host -ForegroundColor Red "Release $($gitHubSettings.ReleaseName) already exists on Github."
	exit 20
}

Write-Host -ForegroundColor DarkGreen 'Release number is unique.'

Write-Host -ForegroundColor DarkYellow 'Creating release ...'

$newRel = New-GitHubRelease -OwnerName "$($gitHubSettings.Owner)" -RepositoryName "$($gitHubSettings.Project)" -Tag "$($gitHubSettings.ReleaseName)" -Name "$($gitHubSettings.ReleaseName)" -Body 'Test release to check new publication script.' -Draft

Write-Host -ForegroundColor DarkGreen "New release $($gitHubSettings.ReleaseName) created with ID $($newRel.ID)."

Write-Host -ForegroundColor DarkYellow 'Uploading release assets ...'

New-GitHubReleaseAsset -OwnerName "$($gitHubSettings.Owner)" -RepositoryName "$($gitHubSettings.Project)" -Release $newRel.ID -Label 'Source code (.tar.gz)' -Path "$srcPath" -ContentType 'application/gzip'
New-GitHubReleaseAsset -OwnerName "$($gitHubSettings.Owner)" -RepositoryName "$($gitHubSettings.Project)" -Release $newRel.ID -Label 'Binaries (.tar.gz)' -Path "$binPath" -ContentType 'application/gzip'

Write-Host -ForegroundColor Green 'Done.'

# NuGet

Write-Host -ForegroundColor Cyan 'Publishing NuGet package ...'

& nuget push "$nupkgPath"

Write-Host -ForegroundColor Green 'Done.'

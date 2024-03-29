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

# basic folders
$scriptDir = $PSScriptRoot
$rootDir = [System.IO.Path]::Combine($scriptDir, '..')

# retrieve Git revision
$rev = &git rev-parse HEAD

# check external tools locations
$inkscapePath = $Env:INKSCAPE_PATH
if (-not $inkscapePath) {
	Write-Host -ForegroundColor Red 'Inkscape path not set.'
	exit 8
}

$sevenZipPath = $Env:SEVENZIP_PATH
if (-not $sevenZipPath) {
	Write-Host -ForegroundColor Red '7zip path not set.'
	exit 9
}

# create temp directory
$tempDir = [System.IO.Path]::Combine($rootDir, 'pubinfo', '_tmp')
if (Test-Path "$tempDir") {
	Remove-Item -LiteralPath "$tempDir" -Force -Recurse
}

[System.IO.Directory]::CreateDirectory("$tempDir") | Out-Null

# rasterize logo
$logoPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo.svg')
$logoDestPath = [System.IO.Path]::Combine($tempDir, 'logo128.png')
&"$inkscapePath" --export-type="png" --export-width=128 -o "$logoDestPath" "$logoPath"
Write-Host -ForegroundColor Cyan 'Logo PNG generated.'

# adapt readme file
$readmePath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'readme.txt')
$destReadmePath = [System.IO.Path]::Combine($tempDir, 'readme.txt')
$readme = Get-Content -Path "$readmePath" -Raw
$readme = $readme -replace '%DATE%', [System.DateTime]::UtcNow.ToString('yyyy-MM-dd')
$readme = $readme -replace '%VERSION%', "$version"
$readme = $readme -replace '%REV%', "$rev"
$readme | Out-File -FilePath "$destReadmePath"
Write-Host -ForegroundColor Cyan 'Readme file adapted.'

# retrieve previous assembly file version
$dllOutputPath = [System.IO.Path]::Combine($rootDir, 'bin', 'Release', 'net8.0-windows', 'NameBasedGrid.dll') # any compiled file will do, no matter which target
. '.\buildVersionHelper.ps1'
$previousBuild = getFileBuildVersion -path "$dllOutputPath"
if ($previousBuild -ne $null) {
	$previousBuild = $previousBuild + 1
} else {
	Write-Host -ForegroundColor Yellow "A previous file version could not be retrieved from $dllOutputPath. Build counting will start at 0."
	Write-Host -ForegroundColor Yellow 'If this is the first build of the project, you can safely ignore this message. However, it might also mean that a previously built file is not present on the current machine, or that the script is not looking in the right location.'
	
	$previousBuild = 0
}
$newFileVer = "$version.$previousBuild"
Write-Host -ForegroundColor Cyan "New build version: $newFileVer"

# build and pack project
$slnDir = [System.IO.Path]::Combine($rootDir, 'src')
$slnPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid.sln')
$pjPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid', 'NameBasedGrid.csproj')

&dotnet build -p "Version=$version" -p "FileVersion=$newFileVer" -c Release "$slnPath"
Write-Host -ForegroundColor Cyan 'Code compiled.'

&dotnet pack "$pjPath" --no-build --no-restore -o "$tempDir" -p "Version=$version"
Write-Host -ForegroundColor Cyan 'NuGet package packed.'

# create custom packages
$srcTarPath = [System.IO.Path]::Combine($tempDir, "NameBasedGrid-$version-src.tar")
&"$sevenZipPath" a "-xr!.vs" "-xr!bin" "-xr!obj" "-i!$destReadmePath" -ttar "$srcTarPath" "$([System.IO.Path]::Combine($rootDir, 'src'))"
Write-Host -ForegroundColor Cyan 'Sources release package tarred ...'
&"$sevenZipPath" a -tgzip "$($srcTarPath).gz" "$srcTarPath"
Write-Host -ForegroundColor Cyan '... and gzipped.'

$binTarPath = [System.IO.Path]::Combine($tempDir, "NameBasedGrid-$version-bin.tar")
&"$sevenZipPath" a "-i!$destReadmePath" -ttar "$binTarPath" "$([System.IO.Path]::Combine($rootDir, 'bin', 'Release', '*'))"
Write-Host -ForegroundColor Cyan 'Binaries release package tarred ...'
&"$sevenZipPath" a -tgzip "$($binTarPath).gz" "$binTarPath"
Write-Host -ForegroundColor Cyan '... and gzipped.'

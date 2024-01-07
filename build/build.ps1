Param(
  [Parameter(Mandatory=$true)][String]$version
)

if (-not ($version -match '^[0-9]+\.[0-9]+\.[0-9]+$')) {
	Write-Host -ForegroundColor Red "$version is not a basic three-component version in major.minor.patch format."
	exit 5
}

$scriptDir = $PSScriptRoot
$rootDir = [System.IO.Path]::Combine($scriptDir, '..')

$inkscapePath = $Env:INKSCAPE_PATH
if (-not $inkscapePath) {
	Write-Host -ForegroundColor Red 'Inkscape path not set.'
	exit 8
}

$logoPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo.svg')
$logoDestPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo128.png')
&"$inkscapePath" --export-type="png" --export-width=128 -o "$logoDestPath" "$logoPath"

. '.\buildVersionHelper.ps1'

# retrieve previous assembly file version
$dllOutputPath = [System.IO.Path]::Combine($rootDir, 'bin', 'Release', 'net8.0-windows', 'NameBasedGrid.dll') # any compiled file will do, no matter which target
$newFileVer = getFileVersion -path "$dllOutputPath" -incBuild $true
if (-not $newFileVer) {
	Write-Host -ForegroundColor Yellow "A previous file version could not be retrieved from $dllOutputPath. Build counting will start at 0."
	Write-Host -ForegroundColor Yellow 'If this is the first build of the project, you can safely ignore this message. However, it might also mean that a previously built file is not present on the current machine, or that the script is not looking in the right location.'
	
	$newFileVer = "$version.0"
}
Write-Host -ForegroundColor Cyan "New build version: $newFileVer"

$slnDir = [System.IO.Path]::Combine($rootDir, 'src')
$slnPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid.sln')
$pjPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid', 'NameBasedGrid.csproj')

&dotnet build -p "Version=$version" -p "FileVersion=$newFileVer" -c Release "$slnPath"

&dotnet pack "$pjPath" --no-build --no-restore -o "$([System.IO.Path]::Combine($rootDir, 'pubinfo'))" -p "Version=$version"
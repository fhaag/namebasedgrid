Param(
  [Parameter(Mandatory=$true)][String]$version
)

$scriptDir = $PSScriptRoot
$rootDir = [System.IO.Path]::Combine($scriptDir, '..')

$inkscapePath = $Env:INKSCAPE_PATH
if (-not $inkscapePath) {
	Write-Host -ForegroundColor Red 'Inkscape path not set.'
	return
}

$logoPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo.svg')
$logoDestPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo128.png')
&"$inkscapePath" --export-type="png" --export-width=128 -o "$logoDestPath" "$logoPath"

. '.\buildVersionHelper.ps1'

# retrieve previous assembly file version
$dllOutputPath = [System.IO.Path]::Combine($rootDir, 'bin', 'Release', 'net8.0-windows', 'NameBasedGrid.dll') # any compiled file will do, no matter which target
$newFileVer = getFileVersion -path "$dllOutputPath" -incBuild $true
if (-not $newFileVer) {
	$newFileVer = "$version.0"
}
Write-Host -ForegroundColor Cyan "New build version: $newFileVer"

$slnDir = [System.IO.Path]::Combine($rootDir, 'src')
$slnPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid.sln')
$pjPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid', 'NameBasedGrid.csproj')

&dotnet build -p "Version=$version" -p "FileVersion=$newFileVer" -c Release "$slnPath"

$nuspecTemplatePath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'NameBasedGrid.template.nuspec')
$nuspecPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'NameBasedGrid.nuspec')

$nuspecDoc = New-Object -TypeName System.Xml.XmlDocument
$nuspecDoc.Load($nuspecTemplatePath)

#$versionEl = $nuspecDoc.DocumentElement.SelectSingleNode('/package/metadata/version')
#$versionEl.InnerText = $version

$nuspecDoc.Save($nuspecPath)

&dotnet pack "$pjPath" --no-build --no-restore -o "$([System.IO.Path]::Combine($rootDir, 'pubinfo'))" -p "Version=$version"
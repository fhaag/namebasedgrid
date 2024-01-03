Param(
  [Parameter(Mandatory=$true)][String]$version
)

$scriptDir = $PSScriptRoot
$rootDir = [System.IO.Path]::Combine($scriptDir, '..')

$inkscapePath = $Env:INKSCAPE_PATH
if (-not $inkscapePath) {
	Write-Output 'Inkscape path not set.'
	return
}

$logoPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo.svg')
$logoDestPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'logo128.png')
&"$inkscapePath" --export-type="png" --export-width=128 -o "$logoDestPath" "$logoPath"

$slnDir = [System.IO.Path]::Combine($rootDir, 'src')
$slnPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid.sln')
$pjPath = [System.IO.Path]::Combine($slnDir, 'NameBasedGrid', 'NameBasedGrid.csproj')

&dotnet build "$slnPath" -c Release

$nuspecTemplatePath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'NameBasedGrid.template.nuspec')
$nuspecPath = [System.IO.Path]::Combine($rootDir, 'pubinfo', 'NameBasedGrid.nuspec')

$nuspecDoc = New-Object -TypeName System.Xml.XmlDocument
$nuspecDoc.Load($nuspecTemplatePath)

$versionEl = $nuspecDoc.DocumentElement.SelectSingleNode('/package/metadata/version')
$versionEl.InnerText = $version

$nuspecDoc.Save($nuspecPath)

&dotnet pack "$pjPath" -o "$([System.IO.Path]::Combine($rootDir, 'pubinfo'))"
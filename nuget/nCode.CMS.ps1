$bin = "..\bin"
$src = "..\src"
$pkg = "nCode.CMS"

If (-Not (Test-Path $pkg)) { Write-Error "Missing nuget working directory: $pkg"; Exit; }

msbuild "$src\$pkg\$pkg.csproj" /t:Rebuild /p:Configuration=Release /p:Platform="AnyCPU"

If (Test-Path $pkg\lib) { rm $pkg\lib -Recurse }
md $pkg\lib\net45 | Out-Null

If (Test-Path $pkg\content) { rm $pkg\content -Recurse }
md $pkg\content | Out-Null


$packages = [xml](gc "$src\$pkg\packages.config");
$nuspec = [xml](gc "$pkg.nuspec.template");

$packages.selectNodes('//packages/package') | % {
    $dependency = $nuspec.CreateElement("dependency", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");
    $dependency.SetAttribute("id", $_.id);
    $dependency.SetAttribute("version", $_.version);
    $nuspec.package.metadata.dependencies.AppendChild($dependency);
} | Out-Null;

$nuspec.Save("$pwd\$pkg.nuspec");

cp $bin\$pkg.dll $pkg\lib\net45
cp $bin\$pkg.xml $pkg\lib\net45

cp "$src\$pkg\wwwroot\*.xdt" "$pkg\content\" -Recurse -Force

nuget pack "$pkg.nuspec" -BasePath "$pkg";

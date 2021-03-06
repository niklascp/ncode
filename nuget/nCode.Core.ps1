$bin = "..\bin"
$src = "..\src"
$pkg = "nCode.Core"

If (-Not (Test-Path $pkg)) { Write-Error "Missing nuget working directory: $pkg"; Exit; }

msbuild "$src\nCode\nCode.csproj" /t:Rebuild /p:Configuration=Release /p:Platform="AnyCPU"

If (Test-Path $pkg\lib) { rm $pkg\lib -Recurse }
md $pkg\lib\net45 | Out-Null
md "$pkg\lib\net45\da-DK" | Out-Null

If (Test-Path $pkg\content) { rm $pkg\content -Recurse }
md $pkg\content | Out-Null

#If (Test-Path $pkg\content\Config) { rm $pkg\content\Config -Recurse }
#md $pkg\content\Config | Out-Null


$packages = [xml](gc "$src\nCode\packages.config");
$nuspec = [xml](gc "$pkg.nuspec.template");

$packages.selectNodes('//packages/package') | % {
    $dependency = $nuspec.CreateElement("dependency", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd");
    $dependency.SetAttribute("id", $_.id);
    $dependency.SetAttribute("version", $_.version);
    $nuspec.package.metadata.dependencies.AppendChild($dependency);
} | Out-Null;

$nuspec.Save("$pwd\$pkg.nuspec");

cp $bin\nCode.dll $pkg\lib\net45
cp $bin\nCode.xml $pkg\lib\net45
cp "$bin\da-DK\nCode.resources.dll" "$pkg\lib\net45\da-DK"

# cp "$src\nCode\wwwroot\Config\Log.config" "$pkg\content\Config\Log.config" -Recurse -Force
cp "$src\nCode\wwwroot\*.xdt" "$pkg\content\" -Recurse -Force

nuget pack "$pkg.nuspec" -BasePath "$pkg";

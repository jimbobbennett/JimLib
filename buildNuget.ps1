Param
(
    [Parameter(Mandatory=$True)][string]$version,
    [string]$copyTo
) 

& 'C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe' .\JimLib.sln /property:Configuration=Release /Target:Rebuild
& 'nuget.exe' pack .\JimBobBennett.JimLib.nuspec -Properties Configuration=Release -Version $version -Symbols

if ($copyTo)
{
    cp .\JimBobBennett.JimLib.$version.nupkg $copyTo
    cp .\JimBobBennett.JimLib.$version.symbols.nupkg $copyTo
}
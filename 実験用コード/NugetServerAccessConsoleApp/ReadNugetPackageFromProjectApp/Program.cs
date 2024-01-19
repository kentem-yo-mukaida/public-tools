// See https://aka.ms/new-console-template for more information
using ReadNugetPackageFromProjectApp;

Console.WriteLine("Hello, World!");

// ---------------------------------------------
const string csprojNET2Name = "ClassLibrary.NET2";
const string csprojNET48Name = "ClassLibrary.NET48";
const string csprojNET8Name = "ClassLibrary.NET8";
// ---------------------------------------------

// デバッグビルド時
var slnDirectoryPath = Path.GetDirectoryName(
    Path.GetDirectoryName(
        Path.GetDirectoryName(
            Path.GetDirectoryName(
                (Environment.CurrentDirectory)
                )
            )
        )
    )!;

var csprojNET2FileName = Path.Combine(slnDirectoryPath, csprojNET2Name, $"{csprojNET2Name}.csproj");
var csprojNET48FileName = Path.Combine(slnDirectoryPath, csprojNET48Name, $"{csprojNET48Name}.csproj");
var csprojNET8FileName = Path.Combine(slnDirectoryPath, csprojNET8Name, $"{csprojNET8Name}.csproj");

Console.WriteLine("--- .NET Standard 2.0 ---");
{
    var nugetPackages = CsprojReader.GetNugetPackages(csprojNET2FileName).ToArray();
    foreach (var nugetPackage in nugetPackages)
        Console.WriteLine($"{nugetPackage.Name} : {nugetPackage.Version}");
}
Console.WriteLine("");

Console.WriteLine("--- .NET Framework 4.8 ---");
{
    var nugetPackages = CsprojReader.GetNugetPackages(csprojNET48FileName).ToArray();
    foreach (var nugetPackage in nugetPackages)
        Console.WriteLine($"{nugetPackage.Name} : {nugetPackage.Version}");
}
Console.WriteLine("");

Console.WriteLine("--- .NET 8 ---");
{
    var nugetPackages = CsprojReader.GetNugetPackages(csprojNET8FileName).ToArray();
    foreach (var nugetPackage in nugetPackages)
        Console.WriteLine($"{nugetPackage.Name} : {nugetPackage.Version}");
}
Console.WriteLine("");


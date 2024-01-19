// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NugetServerAccessConsoleApp;
using System.Collections.Immutable;
using System.Security.AccessControl;
using System.Threading;

// ---------------------------------------------
const string packageId = "Newtonsoft.Json";
// ---------------------------------------------

Console.WriteLine($"{packageId} パッケージの情報を取得します。");

ILogger logger = NullLogger.Instance;
var accesser = new NugetAccesser("https://api.nuget.org/v3/index.json", logger);

var versions = await accesser.GetVersionsAsync(packageId);

// バージョンリスト
Console.WriteLine("--- バージョンリスト ---");
foreach (NuGetVersion version in versions)
{
    Console.WriteLine($"Found version {version}");
}
Console.WriteLine("");

// パッケージダウンロード
Console.WriteLine("--- パッケージダウンロード ---");
//var packageVersion = versions.Last();
var packageVersion = new NuGetVersion("12.0.3");
var nuspecReader = await accesser.GetNuspecReaderAsync(packageId, packageVersion);

Console.WriteLine($"Tags: {nuspecReader.GetTags()}");
Console.WriteLine($"Description: {nuspecReader.GetDescription()}");
Console.WriteLine($"GetAuthors: {nuspecReader.GetAuthors()}");
Console.WriteLine($"GetContentFiles: {nuspecReader.GetContentFiles()}");
Console.WriteLine($"GetCopyright: {nuspecReader.GetCopyright()}");
var dependencyGroups = nuspecReader.GetDependencyGroups();
var frameworkAssemblyGroups = nuspecReader.GetFrameworkAssemblyGroups();
var frameworkRefGroups = nuspecReader.GetFrameworkRefGroups();
Console.WriteLine($"GetLanguage: {nuspecReader.GetLanguage()}");
Console.WriteLine($"GetLicenseMetadata: {nuspecReader.GetLicenseMetadata()?.License}");
Console.WriteLine($"GetLicenseUrl: {nuspecReader.GetLicenseUrl()}");
Console.WriteLine($"GetOwners: {nuspecReader.GetOwners()}");
Console.WriteLine($"GetProjectUrl: {nuspecReader.GetProjectUrl()}");
Console.WriteLine($"GetReadme: {nuspecReader.GetReadme()}");
//Console.WriteLine($"GetReferenceGroups: {nuspecReader.GetReferenceGroups()}");
//Console.WriteLine($"Description: {nuspecReader.GetRepositoryMetadata()}");
//Console.WriteLine($"Description: {nuspecReader.GetDescription()}");
Console.WriteLine("");

// メタデータ
Console.WriteLine("--- メタデータ ---");
var metadatas = await accesser.GetMetadataAsync(packageId);
foreach (var metadata in metadatas)
{
    Console.WriteLine($"Identity.Id: {metadata.Identity.Id}");
    Console.WriteLine($"Identity.Version: {(metadata.Identity.HasVersion ? metadata.Identity.Version.OriginalVersion : "")}");
    Console.WriteLine($"Summary: {metadata.Summary}");

    // 脆弱性
    Console.WriteLine("--- 脆弱性 ---");
    var vulnerabilities = metadata.Vulnerabilities ?? Array.Empty<PackageVulnerabilityMetadata>();
    if (vulnerabilities.Any())
    {
        foreach (var vulnerability in metadata.Vulnerabilities ?? Array.Empty<PackageVulnerabilityMetadata>())
        {
            Console.WriteLine($"AdvisoryUrl: {vulnerability.AdvisoryUrl}");
            Console.WriteLine($"Severity: {GetVulnerabilitySeverityString(vulnerability.Severity)}");
        }
    }
    else
    {
        Console.WriteLine($"なし。");
    }
    Console.WriteLine("");
}
Console.WriteLine("");

string GetVulnerabilitySeverityString(int severity)
{
    // https://learn.microsoft.com/ja-jp/nuget/api/vulnerability-info#vulnerability-page
    return severity switch { 0 => "低", 1 => "中", 2 => "高", 3 => "クリティカル", _ => throw new NotImplementedException() };
}

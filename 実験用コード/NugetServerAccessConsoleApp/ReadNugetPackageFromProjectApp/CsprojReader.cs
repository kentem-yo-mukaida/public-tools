using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReadNugetPackageFromProjectApp;

using PackageInfo = (string Name, string Version);

public static class CsprojReader
{
    private const string PackagesConfigName = "packages.config";

    public static IEnumerable<PackageInfo> GetNugetPackages(string csprojFileName)
    {
        var packagesConfigFileName = Path.Combine(Path.GetDirectoryName(csprojFileName)!, PackagesConfigName);
        if (File.Exists(packagesConfigFileName))
            return GetNugetPackagesFromPackagesConfig(packagesConfigFileName);

        var document = new XmlDocument();
        document.Load(csprojFileName);

        var packageInfos = new List<PackageInfo>();
        var projectElement = document["Project"];
        if (projectElement is not null)
        {
            if (projectElement["ItemGroup"] is { } itemGroupElement)
            {
                foreach (var item in itemGroupElement.OfType<XmlElement>())
                {
                    if (item.Name == "PackageReference")
                        packageInfos.Add((item.GetAttribute("Include"), item.GetAttribute("Version")));
                }
            }
        }
        return packageInfos;
    }

    private static IEnumerable<PackageInfo>
        GetNugetPackagesFromPackagesConfig(string packagesConfigFileName)
    {
        var document = new XmlDocument();
        document.Load(packagesConfigFileName);

        var packagesElement = document["packages"];
        if (packagesElement is not null)
        {
            foreach (var item in packagesElement.OfType<XmlElement>())
            {
                if (item.Name == "package")
                    yield return (item.GetAttribute("id"), item.GetAttribute("version"));
            }
        }
    }
}
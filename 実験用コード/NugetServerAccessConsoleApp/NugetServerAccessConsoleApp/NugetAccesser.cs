using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NugetServerAccessConsoleApp;

internal class NugetAccesser(string source, ILogger logger)
{
    private readonly SourceRepository _repository = Repository.Factory.GetCoreV3(source);

    private readonly ILogger _logger = logger;
    private readonly SourceCacheContext _cahge = new();

    public async Task<IEnumerable<NuGetVersion>> GetVersionsAsync(string packageId)
    {
        var resource = await _repository.GetResourceAsync<FindPackageByIdResource>();
        return await resource.GetAllVersionsAsync(packageId, _cahge, _logger, CancellationToken.None);
    }

    public async Task<NuspecReader> GetNuspecReaderAsync(string packageId, NuGetVersion version)
    {
        var resource = await _repository.GetResourceAsync<FindPackageByIdResource>();
        using var packageStream = new MemoryStream();

        await resource.CopyNupkgToStreamAsync(
            packageId,
            version,
            packageStream,
            _cahge,
            _logger,
            CancellationToken.None);

        Console.WriteLine($"Downloaded package {packageId} {version}");

        using var packageReader = new PackageArchiveReader(packageStream);
        return await packageReader.GetNuspecReaderAsync(CancellationToken.None);
    }

    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId)
    {
        var resource = await _repository.GetResourceAsync<PackageMetadataResource>();
        return await resource.GetMetadataAsync(
            packageId,
            includePrerelease: true,
            includeUnlisted: false,
            _cahge,
            _logger,
            CancellationToken.None);
    }
}

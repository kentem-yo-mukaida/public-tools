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
        var pack = await GetRequestPackAsync<FindPackageByIdResource>();
        return await pack.Resource.GetAllVersionsAsync(packageId, _cahge, _logger, pack.CancellationToken);
    }

    public async Task<NuspecReader> GetNuspecReaderAsync(string packageId, NuGetVersion version)
    {
        var pack = await GetRequestPackAsync<FindPackageByIdResource>();
        using var packageStream = new MemoryStream();

        await pack.Resource.CopyNupkgToStreamAsync(
            packageId,
            version,
            packageStream,
            _cahge,
            _logger,
            pack.CancellationToken);

        Console.WriteLine($"Downloaded package {packageId} {version}");

        using var packageReader = new PackageArchiveReader(packageStream);
        return await packageReader.GetNuspecReaderAsync(pack.CancellationToken);
    }

    public async Task<IEnumerable<IPackageSearchMetadata>> GetMetadataAsync(string packageId)
    {
        var pack = await GetRequestPackAsync<PackageMetadataResource>();
        return await pack.Resource.GetMetadataAsync(
            packageId,
            includePrerelease: true,
            includeUnlisted: false,
            _cahge,
            _logger,
            pack.CancellationToken);
    }

    private async Task<(T Resource, CancellationToken CancellationToken)>
        GetRequestPackAsync<T>()
        where T : class, INuGetResource
    {
        return (
            await _repository.GetResourceAsync<T>(),
            CancellationToken.None
            );
    }
}

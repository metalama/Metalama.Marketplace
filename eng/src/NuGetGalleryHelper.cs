// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Newtonsoft.Json.Linq;
using PostSharp.Engineering.BuildTools.Build;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BuildMetalamaMarketplace;

public static class NuGetGalleryHelper
{
    public static async Task<int?> GetDownloadCountsAsync(string packageName, BuildContext context)
    {
        using var client = new HttpClient();
        var apis = JObject.Parse(await client.GetStringAsync("https://api.nuget.org/v3/index.json"));
        
        var searchQueryServiceEndpoint = apis.GetValue("resources", StringComparison.Ordinal)?.FirstOrDefault(r => r.Value<string>("@type") == "SearchQueryService")?.Value<string>("@id");

        if (searchQueryServiceEndpoint == null)
        {
            return null;
        }
            
        // Fetch the package details
        var requestUri = $"{searchQueryServiceEndpoint}?q={packageName}&prerelease=true&semVerLevel=2.0.0";
        var packages = JObject.Parse(await client.GetStringAsync(requestUri));
        var package = packages.GetValue("data", StringComparison.Ordinal)?.FirstOrDefault(d => d.Value<string>("id") == packageName);

        if (package == null)
        {
            context.Console.WriteWarning( $"Cannot find the package '{packageName}' in the response of '{requestUri}'." );
            return null;
        }
            
        var downloads = package.Value<int>("totalDownloads");

        return downloads;
    }
}
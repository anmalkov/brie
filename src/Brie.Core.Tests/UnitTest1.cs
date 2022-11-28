using Brie.Core.Repositories;

namespace Brie.Core.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var directoryName = "Security Domain";

        var httpClient = new HttpClient();
        var repository = new GitHubApiRepository(httpClient);

        var directory = await repository.GetContentAsync("anmalkov", "brief", directoryName);

        Assert.NotNull(directory);
        Assert.Equal(directoryName, directory.Name);
        Assert.NotNull(directory.Directories);
        Assert.True(directory.Directories.Count() > 0);
    }
}
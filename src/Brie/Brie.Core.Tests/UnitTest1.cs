using Brie.Core.Repositories;

namespace Brie.Core.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var httpClient = new HttpClient();
        var repository = new GitHubRepository(httpClient);

        var directory = await repository.GetDirectoryAsync();
    }
}
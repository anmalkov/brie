using Brie.Core.Models;
using Brie.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Services;

public class PlansService : IPlansService
{
    private readonly IGitHubRepository _gitHubRepository;
    private readonly IPlansRepository _plansRepository;

    public PlansService(IGitHubRepository gitHubRepository, IPlansRepository plansRepository)
    {
        _gitHubRepository = gitHubRepository;
        _plansRepository = plansRepository;
    }

    public async Task<IEnumerable<Plan>?> GetAllAsync()
    {
        return await _plansRepository.GetAllAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public interface IReportsRepository
{
    Task<string?> GetAsync(string id);
    Task CreateAsync(string id, string projectName, string content);
}

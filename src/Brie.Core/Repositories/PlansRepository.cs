using Brie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Brie.Core.Repositories;

public class PlansRepository : RepositoryBase<Plan>, IPlansRepository
{
    private const string RepositoriesDirectoryName = "data";
    private const string RepositoryFilename = "plans.json";

    public PlansRepository() : base(RepositoryFilename) { }
}

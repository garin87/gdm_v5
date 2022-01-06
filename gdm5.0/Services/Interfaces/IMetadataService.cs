using gdm5._0.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IMetadataService
    {
        dynamic GetMetadataTypes();
        dynamic MergeMetadataWithViewRules(Dictionary<string, object> dbMetadata,
                                     Dictionary<string, Dictionary<string, Property>> viewRules);
        dynamic ParseViewRulesFile(string JsonViewRulesFile);
    }
}

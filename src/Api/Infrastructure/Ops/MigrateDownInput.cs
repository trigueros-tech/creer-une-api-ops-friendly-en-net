using System.Collections.Generic;
using Oakton;
using Oakton.AspNetCore;

namespace Api.Infrastructure.Ops
{
    /// <summary>
    /// Hériter de la classe NetCoreInput permet d'instancier une webapp
    /// (et permet donc d'accéder à sa configuration et ses services)
    /// </summary>
    public class MigrateDownInput : NetCoreInput
    {
        [Description("The target version")] public long Version { get; set; }
        
        [Description("The tags to include in the migration")]
        [FlagAlias("tag", 't')]
        public IEnumerable<string> TagsFlag { get; set; }
    }
}
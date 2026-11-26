using System;
using System.Collections.Generic;
using System.Text;
using TM.Common.Utils.Interfaces;

namespace TM.Common.Utils
{
    public class SeederManager(IEnumerable<ISeeder> seeders) : ISeederManager
    {
        public void Run(CancellationToken cancellationToken)
        {
            foreach (var seeder in seeders)
                seeder.Run(cancellationToken);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            foreach (var seeder in seeders)
                await seeder.RunAsync(cancellationToken);
        }
    }
}

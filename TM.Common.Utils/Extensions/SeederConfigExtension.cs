using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TM.Common.Utils.Interfaces;

namespace TM.Common.Utils.Extensions
{
    public static class SeederConfigExtension
    {
        public static void AddTMSeederManager(this IServiceCollection services) => 
            services.AddScoped<ISeederManager, SeederManager>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TM.Common.Utils.Interfaces
{
    public interface ISeederManager
    {
        void Run(CancellationToken cancellationToken);
        Task RunAsync(CancellationToken cancellationToken);
    }
}

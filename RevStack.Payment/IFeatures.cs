using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevStack.Payment
{
    public interface IFeatures
    {
        bool Purchase { get; }
        bool Capture { get; }
        bool Credit { get; }
        bool Void { get; }
    }
}

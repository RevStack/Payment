using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevStack.Payment
{
    public interface IGatewayApiRequest
    {
        IGatewayResponse Send(GatewayRequest request);
    }
}

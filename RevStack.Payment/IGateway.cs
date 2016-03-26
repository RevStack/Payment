using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RevStack.Payment
{
    public interface IGateway
    {
        GatewayAuth Auth {get; set;}
        ServiceMode Mode { get; set; }
        GatewayRequest CreateRequest();
        IGatewayResponse Send(GatewayRequest request);
    }
}
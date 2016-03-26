using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevStack.Payment.Providers.AuthorizeDotNet;

namespace RevStack.Payment
{
    public static class GatewayManager
    {
        public static ReadOnlyCollection<GatewayInfo> Gateways()
        {
            int index = 0;
            List<GatewayInfo> list = new List<GatewayInfo>();
            ReadOnlyCollection<GatewayType> types = GatewayTypes();
            foreach (GatewayType gatewayType in types)
            {
                GatewayInfo info = new GatewayInfo();
                info.Id = index;
                info.GatewayType = gatewayType;

                switch (gatewayType)
                {
                    case GatewayType.AuthorizeDotNet:
                        info.Features = new AuthorizeDotNetFeatures();
                        break;
                }
                list.Add(info);
            }

            return list.AsReadOnly();
        }

        public static ReadOnlyCollection<GatewayType> GatewayTypes()
        {
            return Array.AsReadOnly((GatewayType[])Enum.GetValues(typeof(GatewayType)));
        }
    }
}

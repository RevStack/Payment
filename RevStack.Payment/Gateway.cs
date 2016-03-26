using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;
using RevStack.Payment.Providers.AuthorizeDotNet;


namespace RevStack.Payment
{
    public class Gateway : IGateway
    {
        #region Private members and constructors

        public GatewayType GatewayType { get; set; }
        public GatewayAuth Auth { get; set; }
        public ServiceMode Mode { get; set; }
        private GatewayRequest Request { get; set; }
        private IFeatures Features { get; set; }
        public Dictionary<string, string> Post { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="gatewayType">Type of the gateway.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public Gateway(GatewayType gatewayType, string username, string password, string signature)
            : this(gatewayType, new GatewayAuth(username, password, signature), ServiceMode.Test)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="gatewayInfo">The gateway info.</param>
        public Gateway(GatewayInfo gatewayInfo, string username, string password, string signature, ServiceMode mode)
            : this(gatewayInfo.GatewayType, new GatewayAuth(username, password, signature), mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gateway"/> class.
        /// </summary>
        /// <param name="gatewayType">Type of the gateway.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="isTestMode">if set to <c>true</c> [is test mode].</param>
        public Gateway(GatewayType gatewayType, GatewayAuth auth, ServiceMode mode)
        {
            GatewayType = gatewayType;
            Auth = auth;
            Mode = mode;
            bool isTestMode = mode == ServiceMode.Test ? true : false;

            switch (gatewayType)
            {
                case GatewayType.AuthorizeDotNet:
                    Request = new AuthorizeDotNetRequest(Auth.Username, Auth.Password, isTestMode);
                    Features = new AuthorizeDotNetFeatures();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Sends this instance.
        /// </summary>
        /// <returns></returns>
        public IGatewayResponse Send()
        {
            return Send(Request);
        }

        public GatewayRequest CreateRequest() 
        {
            return Request;
        }

        /// <summary>
        /// Sends the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public IGatewayResponse Send(GatewayRequest request)
        {
            IGatewayResponse gatewayResponse = null;
            switch (GatewayType)
            {
                case GatewayType.AuthorizeDotNet:
                    gatewayResponse = new AuthorizeDotNetApiRequest().Send(request);
                    break;
            }

            return gatewayResponse;
        }
    }
}
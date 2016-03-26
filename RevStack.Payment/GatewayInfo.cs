namespace RevStack.Payment
{
    public class GatewayInfo
    {
        /// <summary>
        /// Gets or sets the gateway id as in a local unique identifier for multi-gateways environments.
        /// </summary>
        /// <value>
        /// The gateway id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the payment gateway.
        /// </summary>
        /// <value>
        /// The type of the payment gateway.
        /// </value>
        public GatewayType GatewayType { get; set; }

        /// <summary>
        /// Gets or sets the type of the features.
        /// </summary>
        /// <value>
        /// Features.
        /// </value>
        public IFeatures Features { get; set; }
    }
}
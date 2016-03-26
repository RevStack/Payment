using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevStack.Payment
{
    public class GatewayAuth
    {
        public GatewayAuth() { }

        public GatewayAuth(string username, string password, string signature) 
        {
            Username = username;
            Password = password;
            Signature = signature;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Signature.
        /// </summary>
        /// <value>
        /// The Signature.
        /// </value>
        public string Signature { get; set; }
    }
}

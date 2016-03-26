using System;

namespace RevStack.Payment.Providers.AuthorizeDotNet
{
    public class AuthorizeDotNetFeatures : IFeatures
    {
        public bool Purchase
        {
            get { return true; }
        }

        public bool Capture
        {
            get { return true; }
        }

        public bool Credit
        {
            get { return true; }
        }

        public bool Void
        {
            get { return true; }
        }
    }
}

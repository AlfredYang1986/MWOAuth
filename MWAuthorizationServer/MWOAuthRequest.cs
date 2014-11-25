using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWAuthorizationServer
{
    class RequestPhrase
    {
        MWOAuthRequest phraseRequest()
        {
            return null;
        }
    }

    abstract class MWOAuthRequest
    {
        abstract public string response_type { get; }
        abstract public MWOAuthResponse response { get; }
    }

    class MWOAuthResourceOwnerPasswordRequest : MWOAuthRequest
    {
        override public string response_type { get { return @"Password"; } }
        override public MWOAuthResponse response { get { return MWOAuthResponseFactory.ResponseInstance(MWOAuthResponseFactory.response_type.json); } }
        public string username { get; set; }
        public string password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWAuthorizationServer
{
    class MWOAuthResponseFactory
    {
        public enum response_type 
        {
            urlencoded,
            json,
        };

        public static MWOAuthResponse ResponseInstance(response_type type)
        {
            MWOAuthResponse re = null;
            switch (type)
            {
                case response_type.urlencoded:
                    re = new MWUrlEncodedResponse();
                    break;
                case response_type.json:
                    re = new MWJsonResponse();
                    break;
            }
            return re;
        }

    }

    abstract class MWOAuthResponse
    {
        abstract public string response_type { get; }
        abstract public string response_body { get; }
    }

    class MWUrlEncodedResponse : MWOAuthResponse
    {
        override public string response_type { get { return @"application/x-www-form-urlencoded; charset=utf-8"; } }
        override public string response_body { get { return "Alfred Yang's Demo"; } }
    }

    class MWJsonResponse : MWOAuthResponse
    {
        override public string response_type { get { return @"application/json; charset=utf-8"; } }
        override public string response_body { get { return "Alfred Yang's Demo"; } }
    }
}

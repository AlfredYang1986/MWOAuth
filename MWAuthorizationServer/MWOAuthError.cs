using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MWAuthorizationServer
{
    class MWOAuthError
    {
        public enum error
        {
            no_error,

            /************************************************************************/
            /* Authorization Endpoint Error                                         */
            /************************************************************************/
            invalid_request,
            unauthorized_client,
            access_denied,
            invalid_scope,

            server_error,
            temporarily_unavailable,
        }

        static KeyValuePair<error, string>[] kv = 
        {
            /************************************************************************/
            /* Authorization Endpoint Error                                         */
            /************************************************************************/
            new KeyValuePair<error, string>(error.invalid_request, @"invalid_request"),
            new KeyValuePair<error, string>(error.unauthorized_client, @"unauthorized_client"),
            new KeyValuePair<error, string>(error.access_denied, @"access_denied"),
            new KeyValuePair<error, string>(error.invalid_scope, @"invalid_scope"),

            new KeyValuePair<error, string>(error.server_error, @"server_error"),
            new KeyValuePair<error, string>(error.temporarily_unavailable, @"temporarily_unavailable"),
        };

        public string error_message(error e)
        {
            return (from re in kv
                   where re.Key == e
                   select re).First().Value;
        }

        public static Stream RequestError(error e, string state, string redirect_uri)
        {
            return null;
        }
    }
}

/**
 * Authorization Server
 * responsible for user login, Enterprise Login and Third party Authorization 
 * Created by Alfred Yang
 * 14 April, 2014
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MWAuthorizationServer
{
    [ServiceContract]
    public interface IMWAuthorizationServer
    {
        /************************************************************************/
        /* Access Token Endpoint                                                */
        /* For all authorization users                                          */
        /* all the return tokens imply date and user indentify                  */
        /* For refreshing the token with refresh_token grant_type               */
        /************************************************************************/

        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "access_token?grant_type={grant_type}&code={code}&redirect_uri={redirect_uri}&client_id={client_id}&username={username}&password={password}&scope={scope}&refresh_token={refresh_token}"
            )]
        Stream mw_access_token(string grant_type, string code = "", string redirect_uri = "", string client_id = "", string username = "", string password = "", string scope = "", string refresh_token = "");

        [OperationContract]
        [WebInvoke(Method = "POST"
            , UriTemplate = "FacebookLogin/"
            , RequestFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Wrapped
            , ResponseFormat = WebMessageFormat.Json)]
        string mw_fb_login(FacebookMapper fbMapper);

        /************************************************************************/
        /* Authorization Endpoint                                               */
        /* For Third party login, and resource owner authrization               */
        /* Actually, this is a two step process                                 */
        /*      first: redirect a HTML and let the resource owner authoritaeted */
        /*      second: response the client base on resource owner's input      */
        /*                                                                      */ 
        /* return                                                               */
        /*      step one may return a HTML form to let resource owner to input  */
        /*      the username and password                                       */
        /*                                                                      */
        /*      step two may return authrization_code if success                */
        /*      or error code otherwise                                         */
        /*                                                                      */
        /* error code:                                                          */
        /*      invalid_request: The request is missing a required parameter    */
        /*      unauthorized_client:                                            */
        /*      access_denied:                                                  */
        /*      invalid_scope:                                                  */
        /*      server_error:                                                   */
        /*      temporarily_unavailable:                                        */
        /************************************************************************/
        
        [OperationContract]
        [WebInvoke(Method="GET"
            , UriTemplate = "Authorization?response_type={response_type}&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&state={state}")]
        Stream mw_authorization(string response_type = "", string client_id = "", string redirect_uri = "", string scope="", string state=""); // for step one

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AuthorizationResponse?username={username}&password={password}&redirect_uri={redirect_uri}&response_type={response_type}&client_id={client_id}")]
        Stream mw_authorization_response(string username, string password, string redirect_uri, string response_type, string client_id = ""); // for step two

        /************************************************************************/
        /* For Creating A Client Identifier and Client secret                   */
        /************************************************************************/
        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "CreateClient/{name}/{expire}"
            , ResponseFormat=WebMessageFormat.Xml)]
        ClientApplication CareteClient(string name, string expire);

        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "UpdateClient/{client_id}/{expire}"
            , ResponseFormat=WebMessageFormat.Xml)]
        ClientApplication UpdateClient(string client_id, string expire);

        /************************************************************************/
        /* For Creating A User Identifer based on the username and password     */
        /* return user identifier                                               */
        /************************************************************************/
        
        /// <summary>
        ///     Creates new user after validating token, forwards user id and other details to the dispatch server
        /// </summary>
        /// <param name="user">UserInfo Json</param>
        /// <returns>Access Token</returns>
        /// <remarks>
        ///     Ayush edited web invoke to post
        /// </remarks>
        [OperationContract]
        [WebInvoke(Method = "POST"
        , UriTemplate = "CreateUser/"
        , RequestFormat = WebMessageFormat.Json
        , BodyStyle = WebMessageBodyStyle.Wrapped
        , ResponseFormat = WebMessageFormat.Xml)]
        string CreateUser(UserInfo user);
        
        /// <summary>
        ///     Updates user password, after validating token
        /// </summary>
        /// <param name="request">ChangePassRequest Json</param>
        /// <returns>Access token</returns>
        /// <remarks>
        ///     Ayush edited web invoke to post
        /// </remarks>
        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "UpdateUserPassword?chptoken={chptoken}&password={password}"
            , ResponseFormat = WebMessageFormat.Json)]
        string UpdateUserPassword(string chptoken, string password);
        //[WebInvoke(Method = "POST"
        //    , UriTemplate = "UpdateUserPassword"
        //    , RequestFormat = WebMessageFormat.Json
        //    , BodyStyle = WebMessageBodyStyle.Wrapped
        //    , ResponseFormat = WebMessageFormat.Json)]
        //string UpdateUserPassword(ChangePassRequest request);
        
        /// <summary>
        ///     For validating an Access Token                                       
        /// </summary>
        /// <param name="token">Access token string</param>
        /// <returns>Validity boolean value</returns>
        
        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "ValidateToken?token={token}"
            , ResponseFormat = WebMessageFormat.Xml)]
        Boolean ValidateToken(string token);

       //----------------------------------------------------------//
        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "GetUserId?token={token}"
            , ResponseFormat = WebMessageFormat.Xml)]
        string GetUserId(string token);
       //----------------------------------------------------------//

        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "CheckUserEmail?email={email}"
            , ResponseFormat = WebMessageFormat.Json)]
        string CheckUserEmail(string email);

        [OperationContract]
        [WebInvoke(Method = "GET"
            , UriTemplate = "ValidateChpToken?chptoken={chptoken}"
            , ResponseFormat = WebMessageFormat.Json)]
        string ValidateChpToken(string chptoken);
    }

    [DataContract]
    public class ClientApplication
    {
        private string _client_id;
        private string _client_secret;

        [DataMember]
        public string client_id 
        {
            get{ return _client_id; }
            set{ _client_id = value; }
        }

        [DataMember]
        public string client_secret
        {
            get{ return _client_secret; }
            set{ _client_secret = value; }
        }
    }

}

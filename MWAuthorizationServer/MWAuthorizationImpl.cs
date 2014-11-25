/**
 * Authorization Server Implementation
 * responsible for user login, Enterprise Login and Third party Authorization 
 * Created by Alfred Yang
 * 14 April, 2014
 */
/*
 * Slight little change. When I was accessing the database entities like "Client", it was throwing errors so I changed them to "Clients" instead
 * Ayush
 * 17 April, 2014
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.ServiceModel.Web;
using System.Net;
using System.Web.Script.Serialization;
using System.Web;
using MWRemoteAPICall;
using Newtonsoft.Json;

namespace MWAuthorizationServer
{
    public class MWAuthorizationImpl : IMWAuthorizationServer
    {
        /*
         * Read the OAuth2.0 and this does look like an entry point for refresh token. 
         * Adding the entry point to access_token, from where the client can ask for a new access token
         * e.g. access_token?grant_type=refresh_token&client_id={client_id}&refresh_token={refresh_token}
         * for above the client has to be validated and can be done with the client secret as well.
         */

        #region access token for authorizated user
        public Stream mw_access_token(string grant_type, string code = "", 
                                      string redirect_uri = "", string client_id = "", 
                                      string username = "", string password = "", 
                                      string scope = "", string refresh_token = "")
        {
            /************************************************************************/
            /* read the arguments in respone_type                                   */
            /*  3. grant_type is password (Resource owner Password Grant)           */
            /*  4. respone_type is client_credentials(CLient Credentials Grant)     */
            /*  5. grant_type is refresh_token                                      */
            /************************************************************************/
            string user_id = string.Empty;

            #region auth code
            if (grant_type == @"authorization_code")
            {
                // code is user_id
                user_id = code;

                if (code == @"" || redirect_uri == @"" || client_id == @"")
                    return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, "Alfred_Yang", redirect_uri);

                using (var db = new MWOAuthDBEntities())
                {
                    var query = from c in db.Clients
                                where c.client_id == client_id
                                select c.client_id;

                    if (query.Count() == 0)
                        return MWOAuthError.RequestError(MWOAuthError.error.access_denied, "Alfred_Yang", redirect_uri);

                    client_id = query.FirstOrDefault();
                }
            }
            #endregion

            #region password
            else if (grant_type == @"password")
            {
                if (username == @"" || password == @"")
                    return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, "Alfred_Yang", redirect_uri);

                using (var db = new MWOAuthDBEntities())
                {
                    var query = from u in db.Users
                                where u.password == password && u.username == username
                                select u.user_id;

                    if (query.Count() == 0)
                        return MWOAuthError.RequestError(MWOAuthError.error.access_denied, "Alfred_Yang", redirect_uri);

                    user_id = query.FirstOrDefault();
                }
                if (client_id == null || client_id == @"")
                    client_id = @"tipLBmIm0UhXzFNTYSfq7gtrhpHN6fUD"; // change for our project
///////////////////////////////////////////////////////////////////////////////////////////////////////
                //verify access-token expired or not 
                using (var db = new MWOAuthDBEntities())
                {
                    var result = from at in db.AccessTokens
                                               where at.client_id == client_id &&
                                                     at.user_id == user_id
                                               select at;
                    string current_accessToken = result.FirstOrDefault().access_token.ToString();
                    DateTime time = MWHashHelper.getDateOfToken(current_accessToken);

                    var s = new JavaScriptSerializer();
                    string jsonClient = "";
                    //if((DateTime.Now-time).TotalMilliseconds<3600000)
                    //{
                    //    AccessToken token = new AccessToken();
                    //    //token.user_id = result.FirstOrDefault().user_id;
                    //    //token.expire_to = result.FirstOrDefault().expire_to;
                    //    token.access_token = result.FirstOrDefault().access_token;
                    //    //token.client_id = result.FirstOrDefault().client_id;
                    //    //token.refresh_token = result.FirstOrDefault().refresh_token;
                    //    jsonClient = s.Serialize(token);
                    //}  
                    //else 
                       jsonClient = s.Serialize( UpdateAccessToken(result, client_id, user_id, username, password));

                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
                    return new MemoryStream(Encoding.UTF8.GetBytes(jsonClient));
                }
//////////////////////////////////////////////////////////////////////////////////////////////////////

            }
            #endregion

            #region client credentials - for unauthorized users
            else if (grant_type == @"client_credentials")
            {
                // for unauthorizated users
                user_id = @"12345";
                if (client_id == @"")
                    client_id = @"tipLBmIm0UhXzFNTYSfq7gtrhpHN6fUD"; // change for our project
            }
            #endregion

            #region refresh token
            else if (grant_type == @"refresh_token")
            {
                //for refreshing the access token, in case of expiry
                //check if client id and refersh token are provided 
                if (client_id == @"" || refresh_token == @"")
                    return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, "Alfred_Yang", redirect_uri);
                //validate the client id with refresh token
                using (var db = new MWOAuthDBEntities())
                {
                    refresh_token = HttpUtility.UrlDecode(refresh_token);
                    var query = from c in db.AccessTokens
                                where c.client_id == client_id && c.refresh_token == refresh_token
                                select c;

                    if (query.Count() == 0)
                        return MWOAuthError.RequestError(MWOAuthError.error.access_denied, "Alfred_Yang", redirect_uri);

                    client_id = query.FirstOrDefault().client_id;
                    user_id = query.FirstOrDefault().user_id;

                    var user = from u in db.Users
                               where u.user_id == user_id
                               select u;
                    //Uncomment the below to lines of code in Release
                    //username = user.FirstOrDefault().username;
                    //password = user.FirstOrDefault().password;
                }
            }
            #endregion

            else
                return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, "Alfred_Yang", redirect_uri);

            #region create token
            //AccessToken token = new AccessToken();
            //using (var db = new MWOAuthDBContainer())
            //{
            //    var query = from t in db.AccessToken
            //                where t.client_id == client_id && t.user_id == user_id
            //                select t;

            //    byte[] identifier = MWHashHelper.createIdentifierKey(username, password);
            //    identifier = MWHashHelper.addTimeStamp(identifier);
            //    if (query.Count() == 0)
            //    {
            //        //token = new AccessToken();
            //        token.user_id = user_id;
            //        token.client_id = client_id;
            //        token.expire_to = 3600; // should be change later
            //        token.access_token = MWHashHelper.ByteToString(identifier);
            //        /*
            //         * Edited by Ayush
            //         * Creating Refresh Token if not already created
            //         * refresh_token = time + access_token
            //         */
            //        //create Refresh Token (may be not for unknown clients!)
            //        byte[] refreshTokenIdentifier = MWHashHelper.createIdentifierKey(token.access_token, string.Empty);
            //        refreshTokenIdentifier = MWHashHelper.addTimeStamp(refreshTokenIdentifier);
            //        token.refresh_token = MWHashHelper.ByteToString(refreshTokenIdentifier);
            //        db.AccessToken.Add(token);
            //        db.SaveChanges();

            //    }
            //    else
            //    {
            //        token.refresh_token = query.FirstOrDefault().refresh_token;
            //        token.user_id = query.FirstOrDefault().user_id;
            //        token.client_id = query.FirstOrDefault().client_id;
            //        token.expire_to = query.FirstOrDefault().expire_to;

            //        if (grant_type == @"refresh_token")
            //        {
            //            if (refresh_token.Equals(token.refresh_token))
            //            {
            //                //generate new access token as the "refresh" Authentication called it
            //                // TODO: judge the token expire or not, if expire refresh ...Optional at this stage
            //                token.access_token = MWHashHelper.ByteToString(identifier);
            //            }
            //            else
            //            {
            //                token.access_token = query.FirstOrDefault().access_token;
            //                //token.access_token = MWHashHelper.ByteToString(identifier);
            //            }
            //        }
            //        else
            //        {
            //            //token.access_token = query.FirstOrDefault().access_token;
            //            token.access_token = MWHashHelper.ByteToString(identifier);
            //        }

            //        //token.access_token = MWHashHelper.ByteToString(identifier);
            //        db.AccessToken.Add(token);
            //        db.SaveChanges();
            //    }


            //}
            #endregion

            //if (token != null)
            //{
            //    var s = new JavaScriptSerializer();
            //    string jsonClient = s.Serialize(token);
            //   // string jsonClient = s.Serialize(token.user_id);
            //    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            //    return new MemoryStream(Encoding.UTF8.GetBytes(jsonClient));
            //}

            return null;
        }
        #endregion

        public AccessToken UpdateAccessToken(IQueryable<AccessToken> currentTokenInfo, string client_id, string user_id,
                                      string username, string password)
        {
            AccessToken token = new AccessToken();
            //AccessToken newToken = new AccessToken();
            using (var db = new MWOAuthDBEntities())
            {
                byte[] identifier = MWHashHelper.createIdentifierKey(username, password);
                identifier = MWHashHelper.addTimeStamp(identifier);

                //if (currentTokenInfo != null)
                //{
                    token.refresh_token = currentTokenInfo.FirstOrDefault().refresh_token;
                    token.user_id = currentTokenInfo.FirstOrDefault().user_id;
                    token.expire_to = currentTokenInfo.FirstOrDefault().expire_to;
                    token.client_id = currentTokenInfo.FirstOrDefault().client_id;
                    token.access_token = currentTokenInfo.FirstOrDefault().access_token;

                    var userToken = from t in db.AccessTokens
                                   where t.access_token == token.access_token
                                   select t;
                    userToken.FirstOrDefault().access_token = MWHashHelper.ByteToString(identifier);
                    db.SaveChanges();
                    token.access_token = MWHashHelper.ByteToString(identifier);
                    //token = new AccessToken();
                    //newToken.user_id = user_id;
                    //newToken.client_id = client_id;
                    //newToken.expire_to = 3600; // should be change later
                    //newToken.access_token = MWHashHelper.ByteToString(identifier);
                    
                    /*
                     * Edited by Ayush
                     * Creating Refresh Token if not already created
                     * refresh_token = time + access_token
                     */
                    //create Refresh Token (may be not for unknown clients!)
                    //byte[] refreshTokenIdentifier = MWHashHelper.createIdentifierKey(token.access_token, string.Empty);
                    //refreshTokenIdentifier = MWHashHelper.addTimeStamp(refreshTokenIdentifier);
                    //newToken.refresh_token = MWHashHelper.ByteToString(refreshTokenIdentifier);

                    //db.AccessToken.Add(newToken);
                    
                    //db.SaveChanges();

                }
                //else
                //{
                //   token.access_token = MWHashHelper.ByteToString(identifier);

                    //token.refresh_token = query.FirstOrDefault().refresh_token;
                    //token.user_id = query.FirstOrDefault().user_id;
                    //token.client_id = query.FirstOrDefault().client_id;
                    //token.expire_to = query.FirstOrDefault().expire_to;

                    //if (grant_type == @"refresh_token")
                    //{
                    //    if (refresh_token.Equals(token.refresh_token))
                    //    {
                    //        //generate new access token as the "refresh" Authentication called it
                    //        // TODO: judge the token expire or not, if expire refresh ...Optional at this stage
                    //        token.access_token = MWHashHelper.ByteToString(identifier);
                    //    }
                    //    else
                    //    {
                    //        token.access_token = query.FirstOrDefault().access_token;
                    //        //token.access_token = MWHashHelper.ByteToString(identifier);
                    //    }
                    //}
                    //else
                    //{
                    //    //token.access_token = query.FirstOrDefault().access_token;
                    //    token.access_token = MWHashHelper.ByteToString(identifier);
                    //}

                    //token.access_token = MWHashHelper.ByteToString(identifier);
                    //db.AccessToken.Add(token);
                    //db.SaveChanges();
                //}
                //newToken.access_token = token.access_token;
                //newToken.user_id = token.user_id;
                return token;
    }
        

        #region authorization step one, return is HTML (user Login)
        public Stream mw_authorization(string response_type, string client_id, string redirect_uri = "", string scope = "", string state = "")
        {
            /************************************************************************/
            /* read the arguments in respone_type                                   */
            /*  1. respone_type is authorization code (Grant)                       */
            /*  2. respone_type is token (Implicit Grant)                           */
            /************************************************************************/
            if (response_type == "" && client_id == "")
                return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, state, redirect_uri);



            if (response_type.ToLower() != @"code" && response_type.ToLower() != @"token")
                return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, state, redirect_uri);

            using (var db = new MWOAuthDBEntities())
            {
                var client_query = from c in db.Clients
                                   where c.client_id == client_id
                                   select c;

                if (client_query.Count() == 0)
                    return MWOAuthError.RequestError(MWOAuthError.error.unauthorized_client, state, redirect_uri);

                // TODO: scope authorization logic ...
                // TODO: state authorization logic ...
            }

            // form page for user to input username and password
            string html = string.Empty;

            /*
             * Changing location for Ayush
             */
            //using (StreamReader streamReader = new StreamReader(@"C:\Users\yy\Desktop\MWAuthorizationServer\authorizationserver\trunk\MWAuthorizationServer\ThirdPartyLogin.html", Encoding.UTF8))
            using (StreamReader streamReader = new StreamReader(@"\\ntapprdfs01n02.rmit.internal\sh4\S3391854\Configuration\Desktop\authorizationserver\trunk\MWAuthorizationServer\ThirdPartyLogin.html", Encoding.UTF8))
            {
                html = streamReader.ReadToEnd();
                html = html.Replace("{redirect_uri}", redirect_uri);
                html = html.Replace("{response_type}", response_type);
                html = html.Replace("{client_id}", client_id);
                //Ayush edited part
                //html = html.Replace("{refresh_token}", refresh_token);
            }

            if (html == string.Empty)
                return MWOAuthError.RequestError(MWOAuthError.error.server_error, state, redirect_uri);

            byte[] resultBytes = Encoding.UTF8.GetBytes(html);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return new MemoryStream(resultBytes);
        }
        #endregion

        #region authorization step two, return is Authorization Code
        public Stream mw_authorization_response(string username, string password, string redirect_uri, string response_type, string client_id = ""
            //, string refresh_token = ""
            )
        {
            /************************************************************************/
            /* authorization code is user id                                        */
            /************************************************************************/
            string authorization_code = string.Empty;
            using (var db = new MWOAuthDBEntities())
            {
                var user_query = from u in db.Users
                                 where u.username == username && u.password == password
                                 select u.user_id;

                if (user_query.Count() == 0)
                    return MWOAuthError.RequestError(MWOAuthError.error.server_error, "Alfred_Yang", redirect_uri);

                authorization_code = user_query.FirstOrDefault();
            }

            if (response_type == @"code")
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
                WebOperationContext.Current.OutgoingResponse.Location = redirect_uri + string.Format("?code={0}&state=xyz", authorization_code);
            }
            else if (response_type == @"token")
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
                client_id = HttpUtility.UrlEncode(client_id);
                WebOperationContext.Current.OutgoingResponse.Location = string.Format("access_token?grant_type=authorization_code&code={0}&redirect_uri={1}&client_id={2}&scope={3}", authorization_code, redirect_uri, client_id, "");

            }
            //Add refresh token query here
            //else if (response_type == @"refresh")
            //{
            //    WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
            //    client_id = HttpUtility.UrlEncode(client_id);
            //    WebOperationContext.Current.OutgoingResponse.Location = string.Format("access_token?grant_type=authorization_code&code={0}&redirect_uri={1}&client_id={2}&scope={3}&refresh_token={4}", authorization_code, redirect_uri, client_id, "", refresh_token);

            //}
            else
                return MWOAuthError.RequestError(MWOAuthError.error.invalid_request, "Alfred_Yang", redirect_uri);

            return null;
        }
        #endregion

        #region Create Client
        public ClientApplication CareteClient(string name, string str_expire)
        {
            DateTime expire = DateTime.Parse(str_expire);
            // client identifier
            byte[] identifier = MWHashHelper.createIdentifierKey(name, "Alfred_Yang");
            identifier = MWHashHelper.addTimeStamp(identifier);
            string client_id = MWHashHelper.ByteToString(identifier);

            // secret
            byte[] secret = MWHashHelper.addRandomByte(null);
            byte[] expire_time = BitConverter.GetBytes(expire.ToBinary());
            secret = secret.Concat(expire_time).ToArray();
            secret = MWHashHelper.addRandomByte(secret);
            string client_secret = MWHashHelper.ByteToString(secret);

            using (var db = new MWOAuthDBEntities())
            {
                Client c = new Client();
                c.client_id = client_id;
                c.client_seret = client_secret;
                c.scope = "mw_all";

                db.Clients.Add(c);
                db.SaveChanges();
            }

            ClientApplication re = new ClientApplication();
            re.client_id = client_id;
            re.client_secret = client_secret;
            return re;
        }
        #endregion

        #region Update Client
        public ClientApplication UpdateClient(string client_id, string str_expire)
        {
            DateTime expire = DateTime.Parse(str_expire);
            using (var db = new MWOAuthDBEntities())
            {
                var query = (from c in db.Clients
                             where c.client_id == client_id
                             select c).First();

                byte[] secret = MWHashHelper.addRandomByte(null);
                byte[] expire_time = BitConverter.GetBytes(expire.ToBinary());
                secret = secret.Concat(expire_time).ToArray();
                secret = MWHashHelper.addRandomByte(secret);
                string client_secret = MWHashHelper.ByteToString(secret);
                query.client_seret = client_secret;
                db.SaveChanges();

                ClientApplication re = new ClientApplication();
                re.client_id = client_id;
                re.client_secret = client_secret;
                return re;
            }
        }
        #endregion

        #region Add userInfo: username ,password, user_id to MWOAthDB
        public User AddUser_OAuth(UserInfo user)
        {
            using (var db = new MWOAuthDBEntities())
            {
                User u = new User();
                u.username = user.email;
                u.password = user.password;
                byte[] identifier = MWHashHelper.createIdentifierKey(user.email, user.password);
                u.user_id = MWHashHelper.ByteToString(identifier);
                db.Users.Add(u);
                db.SaveChanges();
                return u;
            }
            
        }
        #endregion

        #region call dispatch service
        public void Call_dispatch(User u, UserInfo user)
        {
            using(var remoteCall =  MWRemoteAPIFactory.Instance())
            {
                dynamic pramate = new EndPointParameters();
                pramate.userID = u.user_id;
                pramate.email = user.email;
                pramate.DateTimeStr = user.birthday.ToString();
                pramate.gender = user.gender;
                pramate.location = user.location;

                remoteCall.invokes("dispatch", "request",
                                    ParameterFactory.CreateRequestParamters(
                                    ParameterFactory.CreateAuthParamters( user.Token,
                                    ParameterFactory.CreateMethodParamters("CreateUser", pramate as EndPointParameters)))
                                    );
            }
 
        }

        #endregion


        #region Add access_token info in to Access_token table
        public AccessToken Addtoken_OAuth(User u, UserInfo user)
        {
            using (var db = new MWOAuthDBEntities())
            {
                AccessToken newUser_token = new AccessToken();

                byte[] identifier = MWHashHelper.createIdentifierKey(user.email, user.password);
                newUser_token.access_token = MWHashHelper.ByteToString(MWHashHelper.addTimeStamp(identifier));
                newUser_token.expire_to = 3600;
                newUser_token.refresh_token = null;
                newUser_token.client_id = user.client_id;
                newUser_token.user_id = u.user_id;

                db.AccessTokens.Add(newUser_token);
                db.SaveChanges();
                return newUser_token;
            }
        }
        #endregion

        #region Create User
        public string CreateUser(UserInfo user )
        {
            using (var db = new MWOAuthDBEntities())
            {
                // 1. first check whether user email existing
                //    if  email exist return json ----""
                //    else create userid
                //2. go to dispacth service
                //3. update access_token table in AuthoDB

               var user_exist = (from user_set in db.Users
                                where user_set.username.Equals(user.email)   
                                select user_set.username).FirstOrDefault()  ;
    
                if(user_exist == null)
                {

                    User u = AddUser_OAuth(user);
                    Call_dispatch(u, user);
                    AccessToken newUser_token = Addtoken_OAuth(u, user);
                    return new JavaScriptSerializer().Serialize(newUser_token.access_token);

                }
                else
                {
                    return new JavaScriptSerializer().Serialize("user name already exist!");
                }

               
            }
        }
        #endregion

        #region Update User
        public string UpdateUserPassword(ChangePassRequest request)
        {
            request.Token = HttpUtility.UrlDecode(request.Token);
            using (var db = new MWOAuthDBEntities())
            {
                var query = (from at in db.AccessTokens
                             where at.access_token == request.Token
                             select at.User).FirstOrDefault();
                if (query != null)
                {
                    query.password = request.Password;
                    db.SaveChanges();
                }
                return request.Token;
            }
        }
        #endregion

        /************************************************************************/
        /* print the log for the each api call                                  */
        /* use token to generate user_id or client_id                           */
        /************************************************************************/
        private void ApiQueryLogs(string token, string method)
        {
            // TODO: ...
        }

        #region Validate Token
        public Boolean ValidateToken(string token)
        {
            Boolean validity = false;
            using (var db = new MWOAuthDBEntities())
            {
                //Get row from DB with the token
                var query = (
                    from at in db.AccessTokens
                    where at.access_token == token
                    select at).FirstOrDefault();

                //check if there was a token like that
                if (query != null)
                {
                    //Check if the token is expired
                    if (query.expire_to != -1)
                    {
                        //get the time the token was granted
                        DateTime grantTime = MWHashHelper.getDateOfToken(token);
                        grantTime = grantTime.AddSeconds(query.expire_to);
                        //Compare the grantTime + expire_to time to see if expired
                        if (DateTime.Compare(DateTime.UtcNow, grantTime) <= 0)
                            validity = true;
                    }
                    else
                        validity = true;
                }

                //add facebook access token status check
            }
            return validity;
        }
        #endregion
        #region Get userId By validate Token
        public string GetUserId(string token)
        {
            using (var db = new MWOAuthDBEntities())
            {
                token = HttpUtility.UrlDecode(token);
                if (ValidateToken(token))
                {
                    var query = from at in db.AccessTokens
                                where at.access_token == token
                                select at;
                   // return new JavaScriptSerializer().Serialize(query);
                    return query.FirstOrDefault().user_id;
                }
                else
                    return null;

            }
        }
        #endregion

        #region Facebook token mapper
        public string mw_fb_login(FacebookMapper fbMapper)
        {
            using (var db = new MWOAuthDBEntities())
            {
                // 1. check the thirdPartyToken.userid is existing or not
                var query_exist = from ex in db.ThirdPartyTokens
                                  where ex.third_party_user_id == fbMapper.uId
                                  select ex;

                if (query_exist.Count() == 0)
                {
                    // 3. if not create user id
                    // 3.1. new user
                    var newUser = new User();
                    newUser.user_id = "FB-" + fbMapper.uId;
                    newUser.username = "FB-" + fbMapper.uId;
                    newUser.password = "FB-" + fbMapper.uId;

                    // 3.2. new access token
                    var newToken = new AccessToken();
                    byte[] token_byte = MWHashHelper.createIdentifierKey(fbMapper.fbToken, "Facebook");
                    token_byte = MWHashHelper.addTimeStamp(token_byte);
                    newToken.access_token = MWHashHelper.ByteToString(token_byte);
                    newToken.client_id = @"yS2JzRYo0Ui9wO5KHvA3V+kwWapVxdUw";
                    newToken.expire_to = 3600;
                    db.AccessTokens.Add(newToken);

                    newUser.AccessTokens.Add(newToken);
                    db.Users.Add(newUser);

                    // 3.3. new third token
                    var newThirdToken = new ThirdPartyToken();
                    newThirdToken.AccessToken = newToken;
                    newThirdToken.third_party_token = fbMapper.fbToken;
                    newThirdToken.third_party_user_id = fbMapper.uId;
                    newThirdToken.token_provider = "FB";
                    db.ThirdPartyTokens.Add(newThirdToken);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex);                   	
                    }
                    return newToken.access_token;

                }
                else
                {
                    // 2. if it exist, change the third token, and refresh access_token
                    // here is an assumption that new user have no account
                    var user_exist = query_exist.FirstOrDefault();
                    byte[] token_byte = MWHashHelper.createIdentifierKey(fbMapper.fbToken, "Facebook");
                    token_byte = MWHashHelper.addTimeStamp(token_byte);
                    user_exist.AccessToken.access_token = MWHashHelper.ByteToString(token_byte);
                    db.SaveChanges();
                    return user_exist.AccessToken.access_token;
                }
            }
        }
        #endregion

        public string CheckUserEmail(string email)
        {
            using (var db = new MWOAuthDBEntities())
            {
                var query = from u in db.Users
                            where u.username == email
                            select u.user_id;

                if (query.Count() > 0)
                {
                    var user_id = query.FirstOrDefault();
                    byte[] chptoken_byte = MWHashHelper.createIdentifierKey(user_id, "ChangePassword");
                    chptoken_byte = MWHashHelper.addTimeStamp(chptoken_byte);
                    string chptoken = MWHashHelper.ByteToString(chptoken_byte);

                    var queryOldToken = from chp in db.ChangePasswordTokenSets
                                        where chp.user_id == user_id
                                        select chp;

                    if (queryOldToken.Count() > 0)
                    {
                        var oldToken = queryOldToken.FirstOrDefault();
                        oldToken.ChpToken = chptoken;
                        oldToken.expired = 3600;
                    }
                    else
                    {
                        var chp = new ChangePasswordTokenSet();
                        chp.user_id = user_id;
                        chp.ChpToken = chptoken;
                        db.ChangePasswordTokenSets.Add(chp);
                    }
                    db.SaveChanges();
                    return chptoken;
                }
                else
                    return "false";
            }
        }

        public string ValidateChpToken(string chptoken)
        {
            using (var db = new MWOAuthDBEntities())
            {
                string reVal = "false";

                var query = from t in db.ChangePasswordTokenSets
                            where t.ChpToken == chptoken
                            select t;

                if (query.Count() > 0)
                {
                    var tmp = query.FirstOrDefault();
                    var begining = MWHashHelper.getDateOfToken(chptoken);
                    var expire = tmp.expired;

                    if (begining.AddMilliseconds(tmp.expired) < DateTime.Now)
                        reVal = "true";
                }
                return reVal;
            }
        }

        public string UpdateUserPassword(string chptoken, string password)
        {
            using (var db = new MWOAuthDBEntities())
            {
                string reVal = "false";

                var query = from t in db.ChangePasswordTokenSets
                            where t.ChpToken == chptoken
                            select t;

                if (query.Count() > 0)
                {
                    var tmp = query.FirstOrDefault();
                    var begining = MWHashHelper.getDateOfToken(chptoken);
                    var expire = tmp.expired;

                    if (begining.AddMilliseconds(tmp.expired) < DateTime.Now)
                        reVal = "true";
                
                    if (reVal == "true")
                    {
                        var queryUser = (from u in db.Users
                                         where u.user_id == tmp.user_id
                                         select u).FirstOrDefault();

                        queryUser.password = password;
                        db.SaveChanges();
                    }
                }
                return reVal;
            }

        }
    }
}

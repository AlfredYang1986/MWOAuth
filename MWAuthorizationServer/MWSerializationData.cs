using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Globalization;

namespace MWAuthorizationServer
{
    class MWSerializationData
    {
    }
    [DataContract]
    public class FacebookMapper
    {
        [DataMember]
        public string uId { get; set; }
        [DataMember]
        public string fbToken { get; set; }
    }

    [DataContract]
    public class ChangePassRequest
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

    [DataContract]
    public class AuthorizationCheck
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public MWRequestPhraseJSON RequestString { get; set; }
    }
    [DataContract]
    public class MWRequestPhraseJSON
    {
        [DataMember]
        public string MessageName { get; set; }
        [DataMember]
        public string Parameters;

    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string client_id { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public bool enableSSNLogin { get; set; }
        [DataMember]
        public string email { get; set; }
        //[DataMember]
        public Nullable<System.DateTime> birthday 
        { 
            get 
            {
                //CultureInfo provider = new CultureInfo("fr-FR");//CultureInfo.InvariantCulture;
                //string format = "d";
                DateTime dt = Convert.ToDateTime(DateTimeStr);
                return dt;//DateTime.ParseExact(DateTimeStr, format, provider); 
            } 
        }

        [DataMember]
        public string DateTimeStr { get; set; }
        [DataMember]
        public string gender { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Nullable<int> credit { get; set; }
        [DataMember]
        public Nullable<int> experience { get; set; }
        [DataMember]
        public string occupation { get; set; }
        [DataMember]
        public string nationality { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public Nullable<int> profile_picture { get; set; }
    }

}

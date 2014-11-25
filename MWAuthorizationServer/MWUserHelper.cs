using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWAuthorizationServer
{
    class MWUserHelper
    {
    }
    public class UserJson
    {
        public string user_id { get; set; }
        public bool enable_SSN_login { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> birthday { get { return DateTime.Parse(DateTimeStr); } }
        public string DateTimeStr { get; set; }
        public string gender { get; set; }
        public string name { get; set; }
        public Nullable<int> credit { get; set; }
        public Nullable<int> experience { get; set; }
        public string occupation { get; set; }
        public string nationality { get; set; }
        public string location { get; set; }
        public Nullable<int> profile_picture { get; set; }
    }
    public class DispatchUserInfo
    {
        public string userID { get; set; }

        public bool enSSNLogin { get; set; }

        public string email { get; set; }
        public Nullable<System.DateTime> birthday { get { return DateTime.Parse(DateTimeStr); } }
        public string DateTimeStr { get; set; }

        public string gender { get; set; }

        public string name { get; set; }

        public Nullable<int> credit { get; set; }

        public Nullable<int> experience { get; set; }
        public string occupation { get; set; }

        public string nationality { get; set; }

        public string location { get; set; }

        public Nullable<int> profile_picture { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWAuthorizationServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChangePasswordToken
    {
        public int ChpToken_Id { get; set; }
        public int expired { get; set; }
        public string user_id { get; set; }
        public string ChpToken { get; set; }
    
        public virtual User User { get; set; }
    }
}
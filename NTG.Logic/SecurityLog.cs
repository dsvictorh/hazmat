//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NTG.Logic.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SecurityLog : BaseModel<SecurityLog>
    {
        public int Id { get; set; }
        public System.DateTimeOffset Date { get; set; }
        public string Action { get; set; }
        public string IPAddress { get; set; }
        public string Browser { get; set; }
        public Nullable<int> UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserType { get; set; }
    }
}

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
    
    public partial class SiteLog : BaseModel<SiteLog>
    {
        public int Id { get; set; }
        public System.DateTimeOffset Date { get; set; }
        public string IPAddress { get; set; }
        public string Page { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool Important { get; set; }
        public int AdminId { get; set; }
    
        public virtual Admin Admin { get; set; }
    }
}

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
    
    public partial class ModuleSimpleCards : BaseModel<ModuleSimpleCards>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModuleSimpleCards()
        {
            this.ModuleSimpleCardsCards = new HashSet<ModuleSimpleCardsCard>();
        }
    
        public int Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModuleSimpleCardsCard> ModuleSimpleCardsCards { get; set; }
    }
}

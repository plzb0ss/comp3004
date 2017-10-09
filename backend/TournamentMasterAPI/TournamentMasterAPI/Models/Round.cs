//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TournamentMasterAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Round
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Round()
        {
            this.Pairings = new HashSet<Pairing>();
        }
    
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int RoundNumber { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pairing> Pairings { get; set; }
        public virtual Tournament Tournament { get; set; }
    }
}

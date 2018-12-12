namespace KiddyDesktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblToy")]
    public partial class tblToy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblToy()
        {
            tblFeedbacks = new HashSet<tblFeedback>();
            tblOrderDetails = new HashSet<tblOrderDetail>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public string desciption { get; set; }

        public double? price { get; set; }

        public int? quantity { get; set; }

        public bool? isActived { get; set; }

        [StringLength(50)]
        public string createdBy { get; set; }

        [StringLength(30)]
        public string category { get; set; }

        public virtual tblEmployee tblEmployee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFeedback> tblFeedbacks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOrderDetail> tblOrderDetails { get; set; }
    }
}

namespace KiddyAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOrder")]
    public partial class tblOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblOrder()
        {
            tblOrderDetails = new HashSet<tblOrderDetail>();
        }

        public int id { get; set; }

        [StringLength(50)]
        public string cusID { get; set; }
        
        public DateTime date { get; set; }

        [StringLength(50)]
        public string emlID { get; set; }

        [StringLength(50)]
        public string payment { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        public int status { get; set; }

        public virtual tblCustomer tblCustomer { get; set; }

        public virtual tblEmployee tblEmployee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOrderDetail> tblOrderDetails { get; set; }
    }
}

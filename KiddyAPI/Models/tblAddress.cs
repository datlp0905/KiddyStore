namespace KiddyAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblAddress")]
    public partial class tblAddress
    {
        public int id { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        [StringLength(50)]
        public string cusID { get; set; }

        public bool? isActived { get; set; }

        public virtual tblCustomer tblCustomer { get; set; }
    }
}

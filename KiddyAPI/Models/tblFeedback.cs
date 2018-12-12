namespace KiddyAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblFeedback")]
    public partial class tblFeedback
    {
        public int id { get; set; }

        [StringLength(50)]
        public string cusID { get; set; }

        public int? toyID { get; set; }

        public string content { get; set; }

        public int? status { get; set; }

        [StringLength(50)]
        public string confirmedBy { get; set; }

        public virtual tblCustomer tblCustomer { get; set; }

        public virtual tblEmployee tblEmployee { get; set; }

        public virtual tblToy tblToy { get; set; }
    }
}

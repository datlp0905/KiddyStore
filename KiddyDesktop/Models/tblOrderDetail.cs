namespace KiddyDesktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOrderDetail")]
    public partial class tblOrderDetail
    {
        public int id { get; set; }

        public int? orderID { get; set; }

        public int? toyID { get; set; }

        public int? quantity { get; set; }

        public virtual tblOrder tblOrder { get; set; }

        public virtual tblToy tblToy { get; set; }
    }
}

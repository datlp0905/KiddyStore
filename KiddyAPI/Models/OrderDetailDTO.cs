using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiddyAPI.Models
{
    public class OrderDetailDTO
    {
        public int id { get; set; }

        public int? orderID { get; set; }

        public int? toyID { get; set; }

        public int? quantity { get; set; }

        public double? price { get; set; }

        public string name { get; set; }

        public bool? isFeedback { get; set; }
    }
}
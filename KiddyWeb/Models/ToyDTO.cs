using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiddyWeb.Models
{
    public class ToyDTO
    {
        public int? id { get; set; }
        public string name { get; set; }
        public double? price { get; set; }
        public byte[] image { get; set; }
        public string category { get; set; }
        public string description { get; set; }
        public int? quantity { get; set; }
        public bool? isActived { get; set; }
        public string createdBy { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiddyAPI.Models
{
    public class CustomerDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool isActived { get; set; }
    }
}
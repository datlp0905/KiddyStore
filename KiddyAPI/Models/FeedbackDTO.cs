﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiddyAPI.Models
{
    public class FeedbackDTO
    {
        public int id { get; set; }
        public string content { get; set; }
        public string cusName { get; set; }
        public string cusID { get; set; }
        public int? toyID { get; set; }
        public int status { get; set; }
    }
}
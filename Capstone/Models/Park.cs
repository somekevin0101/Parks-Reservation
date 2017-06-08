﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    class Park
    {
        public int ParkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Area { get; set; }
        public DateTime EstablishDate { get; set; }
        public int Visitors { get; set; }
    }
}
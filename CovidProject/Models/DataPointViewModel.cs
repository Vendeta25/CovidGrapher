using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidProject.Models
{
    public class DataPointViewModel
    {
        public string Province { get; set; }
        public int Confirmed { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public string Date { get; set; }
    }
}

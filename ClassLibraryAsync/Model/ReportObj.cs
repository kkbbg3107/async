using ClassLibraryAsync.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Model
{
    public class ReportObj 
    {        
        public int dtno { get; set; }
        public int ftno { get; set; }
        public Assign all { get; set; }
        public string keyMap { get; set; }
        public string assign { get; set; }                   
    }

    public class Assign
    {
        public string AssignID { get; set; }
        public string AssignDate { get; set; }
        public int DTOrder { get; set; }
        public int MTPeriod { get; set; }
    }
}

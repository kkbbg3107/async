using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Model
{
    public class Result
    {
        public bool isCompleted { get; set; }
        public bool isFaulted { get; set; }
        public string signature { get; set; }
        public string exception { get; set; }
        public string result { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmBeachScrapper
{
    public class pbICases
    {
        public string CaseNumber { get; set; }
        public string Courts { get; set; }
        public string CaseType { get; set; }
        public DateTime FileDate { get; set; }
        public string PrimaryParty { get; set; }
        public string SecondaryParty { get; set; }
        public string CaseStatus { get; set; }
    }


    public class PalmCaseInfoClass
    {
        public PalmCaseInfoClass()
        {
            ListaCasos = new List<pbICases>(); 
        }

        public DateTime FileDate { get; set; }
        public int NumberOfCases { get; set; }
        public string County { get; set; }

        public List<pbICases> ListaCasos { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
    public class SprintDetails
    {
        public string SprintName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal capacity { get; set; }
        public double planned { get; set; }
        public double remaining { get; set; }
    }
}

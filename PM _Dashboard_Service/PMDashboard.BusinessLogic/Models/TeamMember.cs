using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
   public class TeamMember
    {
        public string Name { get; set; }
        public Dictionary<Nullable<DateTime>, Nullable<double>> HoursBurnt { get; set; }
    }
}

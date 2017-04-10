using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PM__Dashboard_Service.Models
{
    class TeamMember
    {
        public string Name { get; set; }
        public Dictionary<Nullable<DateTime>, Nullable<double>> HoursBurnt { get; set; }      
    }
}
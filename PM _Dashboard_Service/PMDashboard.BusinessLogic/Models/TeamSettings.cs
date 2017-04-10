using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
   public  class TeamSettings
    {
        //Specify the json property name for each of the property
        [JsonProperty("backlogIteration")]
        BacklogIteration BacklogIteration { get; set; }
        [JsonProperty("bugsBehavior")]
        public string BugsBehavior { get; set; }
        [JsonProperty("workingDays")]
        public string[] WorkingDays { get; set; }
    }
    public class BacklogIteration : TeamSettings
    {
        //Specify the json property name for each of the property
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }

    }
}

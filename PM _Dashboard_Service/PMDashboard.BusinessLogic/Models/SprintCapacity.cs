using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
   public class SprintCapacity
    {
        public string Count { get; set; }
        public List<Value> Value { get; set; }
    }
    public class Value
    {
        //Specify the json property name for each of the property
        [JsonProperty("teamMember")]
        public ContributorDetails TeamMember { get; set; }
        [JsonProperty("activities")]
        public List<Activities> Activities { get; set; }
        [JsonProperty("daysOff")]
        public List<DaysOff> DaysOFF { get; set; }
        [JsonProperty("url")]
        public string DaysOffURL { get; set; }
    }
    public class ContributorDetails
    {
        //Specify the json property name for each of the property
        [JsonProperty("id")]
        public string ContributorID { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonProperty("imageURL")]
        public string ImageURL { get; set; }
    }
    public class Activities
    {
        //Specify the json property name for each of the property
        [JsonProperty("capacityPerday")]
        public decimal CapacityPerDay { get; set; }
        [JsonProperty("name")]
        public string ActivityName { get; set; }
    }
    public class DaysOff
    {
        //Specify the json property name for each of the property
        [JsonProperty("start")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("end")]
        public DateTime? EndDate { get; set; }
    }
}

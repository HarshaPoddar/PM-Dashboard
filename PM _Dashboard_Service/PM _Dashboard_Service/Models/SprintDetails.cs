using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.Client;
using System.Net;

namespace PM__Dashboard_Service.Models
{
    class SprintDetails
    {
        public string SprintName { get; set; }
        public Nullable<DateTime> startDate { get; set; }
        public Nullable<DateTime> endDate { get; set; }
        public Nullable<decimal> capacity { get; set; }
        public Nullable<double>  planned { get; set; }
        public Nullable<double> remaining { get; set; }
    }

    public class TFSDetails
    {
        public Uri CollectionsURI { get; set; }
        public string ProjectURI { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string ProjectName { get; set; }
    
    public TfsTeamProjectCollection GetTeamProjectCollection()
    {

        var projectCollection = new TfsTeamProjectCollection(CollectionsURI, new NetworkCredential(UserName,PassWord));
        projectCollection.EnsureAuthenticated();
        return projectCollection;
    }
}
}
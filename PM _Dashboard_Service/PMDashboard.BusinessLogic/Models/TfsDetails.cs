using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
    public class TfSDetails
    {
        public Uri CollectionsURI { get; set; }
        public string ProjectURI { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string ProjectName { get; set; }
        public string Domain { get; set; }

        public TfsTeamProjectCollection GetTeamProjectCollection()
        {                      
            int indexOFdomain= UserName.IndexOf("/");
            Domain = UserName.Substring(0, indexOFdomain);
             UserName= UserName.Substring(indexOFdomain + 1); 
            var projectCollection = new TfsTeamProjectCollection(CollectionsURI, new NetworkCredential(UserName, PassWord,Domain));
            //Check for Authorization
            projectCollection.EnsureAuthenticated();
            return projectCollection;
        }
    }
}

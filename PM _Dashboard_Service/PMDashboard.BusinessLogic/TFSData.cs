using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.Client;
using PMDashboard.BusinessLogic.Constants;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace PMDashboard.BusinessLogic
{
    public class TFS : IPMDashboardBusinessLogic
    {

        private ICommonStructureService _commonStructureService;
        private TfsTeamProjectCollection _projectCollection;

        /// <summary>
        /// Get's the list of Projects for the given URI
        /// </summary>
        /// <param name="tfsDetails">User Credentials</param>
        /// <returns>A List of Projects</returns>
        public async Task<Dictionary<string, string>> GetProjectList(TfSDetails tfsDetails)
        {
            if (tfsDetails != null)
            {
                _projectCollection = tfsDetails.GetTeamProjectCollection();
                _commonStructureService = (ICommonStructureService)_projectCollection.GetService(typeof(ICommonStructureService));
                return _commonStructureService.ListAllProjects().ToDictionary(x => x.Uri, x => x.Name);

            }
            //CommomnStructureService defines methods to work with Areas and Iterations
            return _commonStructureService.ListAllProjects().ToDictionary(x => x.Uri, x => x.Name);

        }



        /// <summary>
        ///  Get's the Sprint Details for the selected Project
        /// </summary>
        /// <param name="tfsDetails">User Credentials</param>
        /// <returns>List of Sprint Details</returns>
        public async Task<List<SprintDetails>> GetSprintDetails(TfSDetails tfsDetails)
        {
            List<SprintDetails> sprintDetails = new List<SprintDetails>();
            double totalHoursplanned = 0;
            double totalHoursremaining = 0;
            //GetTemaProjectCollection intialises the TfsUri and authinticates the user
            _projectCollection = tfsDetails.GetTeamProjectCollection();

            //CommomnstructureService defines  methods to work with Areas and Iterations
            _commonStructureService = (ICommonStructureService)_projectCollection.GetService(typeof(ICommonStructureService));

            //GroupstructureService defines methods to acess TFS groups
            var groupSecurityservice = _projectCollection.GetService<IGroupSecurityService>();

            //Represents the Work Item Tracking client connection to a server that is running Team Foundation Server.
            var store = _projectCollection.GetService<WorkItemStore>();

            //Iterate Through the project list and acess the User Selected project

            foreach (Project project in store.Projects)
            {
                if (project.Name == tfsDetails.ProjectName)
                {
                    //Iterate through the  sprints and retrive the start and end dates of each sprint 
                    foreach (Node node in project.IterationRootNodes)
                    {
                        //Get the iteration id from the node uri
                        string iterationId = string.Empty;
                        iterationId = node.Uri.Segments[3];


                        //Through the nodeid acess the startdate,enddate and Sprint Name
                        var nodeID = _commonStructureService.GetNode(Convert.ToString(node.Uri));
                        var sprint = new SprintDetails() { SprintName = nodeID.Name, startDate = Convert.ToDateTime(nodeID.StartDate), endDate = Convert.ToDateTime(nodeID.FinishDate) };

                        //Call getcapacitymethod to get the sprint capacity
                         sprint.capacity = GetCapacity(tfsDetails, iterationId, sprint);
                        
                          //Query For tasks under the current sprint 
                          var queryFortasks = store.Query(TFSConstants.WorkItemQuery);

                        //Calculate the planned and Remaining hours foreach sprint
                        foreach (WorkItem task in queryFortasks)
                        {
                            //Calculate for each sprint
                            if (node.Path == task.IterationPath)
                            {
                                totalHoursremaining = totalHoursremaining + Convert.ToDouble(task.Fields[TFSConstants.RemainingWork].Value);
                                totalHoursplanned = totalHoursplanned + Convert.ToDouble(task.Fields[TFSConstants.OriginalEstimate].Value);
                                sprint.planned = totalHoursplanned;
                                sprint.remaining = totalHoursremaining;
                            }
                        }
                        sprintDetails.Add(sprint);
                    }
                    break;
                }
            }
            return sprintDetails;
        }

        /// <summary>
        /// Get's the Team Member Details.
        /// </summary>
        /// <param name="tfsDetails">User Credentials</param>
        /// <returns>List of Team Member Details</returns>
        public async Task<List<TeamMember>> TeamMemberDetails(TfSDetails tfsDetails)
        {

            //TeamSettings Configuration to acess current and Backlog iteration paths
            TeamSettingsConfigurationService teamSettingsConfig = _projectCollection.GetService<TeamSettingsConfigurationService>();
            var currentIterationpath = teamSettingsConfig
                                .GetTeamConfigurationsForUser(new[] { tfsDetails.ProjectURI })
                                .Select(c => c.TeamSettings.CurrentIterationPath)
                                .FirstOrDefault();


            //Represents the Work Item Tracking client connection to a server that is running Team Foundation Server.
            var store = _projectCollection.GetService<WorkItemStore>();
            var projectList = store.Projects;

            //GroupstructureService defines methods to acess tfs groups
            var groupSecurityservice = _projectCollection.GetService<IGroupSecurityService>();

            //List the groups under the project
            //Get the teamMemberNames
            var group = groupSecurityservice
                            .ListApplicationGroups(tfsDetails.ProjectURI)
                            .FirstOrDefault(Project => Project.DisplayName.Contains(TFSConstants.GroupName));

            Identity sids = groupSecurityservice.ReadIdentity(SearchFactor.Sid, group.Sid, QueryMembership.Expanded);
            TfsTeamService teamService = _projectCollection.GetService<TfsTeamService>();

            var team = teamService.QueryTeams(tfsDetails.ProjectURI);
            var Members = from t in team select t.Name;
            string teamName = Members.Single();
            TeamFoundationTeam teams = teamService.ReadTeam(tfsDetails.ProjectURI, teamName, null);
            List<TeamMember> teamMembersDetail = new List<TeamMember>();

            foreach (TeamFoundationIdentity i in teams.GetMembers(_projectCollection, MembershipQuery.Expanded))
            {
                if (i.DisplayName == tfsDetails.ProjectName + " Team")
                    continue;
                teamMembersDetail.Add(new TeamMember() { Name = i.DisplayName });
            }
          
            //Query for tasks under current sprint
            var queryFortasks = store.Query(TFSConstants.WorkItemQuery);

            //Loop through the project list and Select the user specified project
            foreach (Project project in projectList)
            {
                if (project.Name == tfsDetails.ProjectName)
                {
                    //Loop through the sprints and retrive the start and end dates
                    foreach (Node node in project.IterationRootNodes)
                    {
                        //Check for current Iteration
                        if (node.Path == currentIterationpath)
                        {
                            var nodeid = _commonStructureService.GetNode(Convert.ToString(node.Uri));
                            DateTime startDate = Convert.ToDateTime(nodeid.StartDate);
                            DateTime enDate = DateTime.Today;

                            //Loop to add the range of Sprint dates
                            foreach (TeamMember teamMemberdetail in teamMembersDetail)
                            {
                                teamMemberdetail.HoursBurnt = new Dictionary<DateTime?, double?>();
                                for (DateTime test = startDate.Date; test <= enDate; test = test.AddDays(1))
                                {
                                    teamMemberdetail.HoursBurnt.Add(test.Date, 0);
                                }
                            }

                            //Loop through the tasks for a given project and then through each revision
                            foreach (WorkItem task in queryFortasks)
                            {
                                foreach (Revision revision in task.Revisions)
                                {
                                    //Check which user has changed the state of a task
                                    var revisionOwner = revision.Fields["System.ChangedBy"].Value;
                                    var contributor = teamMembersDetail.Where(x => x.Name == revisionOwner.ToString()).FirstOrDefault();
                                    if (contributor != null)
                                    {
                                        DateTime reviseddate = Convert.ToDateTime(revision.Fields["System.ChangedDate"].Value);

                                        //Total number of hours burnt is the updatedValue-oldValue
                                        var hoursBurnt = Math.Abs(Convert.ToDouble(revision.Fields[TFSConstants.CompletedWork].Value) - Convert.ToDouble(revision.Fields[TFSConstants.CompletedWork].OriginalValue));

                                        //Update the Hours burnt for the given revised date.
                                        if (contributor.HoursBurnt.ContainsKey(reviseddate.Date))
                                        {
                                            if (contributor.HoursBurnt.Where(c => c.Key.Value.Date == reviseddate.Date).Select(c => c.Value != 0.0).Single())
                                                contributor.HoursBurnt[reviseddate.Date] = Convert.ToDouble(contributor.HoursBurnt.Where(c => c.Key.Value.Date == reviseddate.Date).Select(c => c.Value).Single()) + hoursBurnt;
                                            else
                                                contributor.HoursBurnt[reviseddate.Date] = Convert.ToDouble(hoursBurnt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
                
            }

            return teamMembersDetail;
        }


    /// <summary>
    /// Get Capacity Method Returns the capacity
    /// for current sprint.
    /// Capacity value is retained using a rest api.
    /// </summary>
    /// <param name="tfsDetails"></param>
    /// <param name="iterationId"></param>
    /// <returns></returns>
    public decimal GetCapacity(TfSDetails tfsDetails, string iterationId, SprintDetails sprint)
    {
            //Input the baseAddress
            var restClient = new RestClient(new Uri(Convert.ToString(tfsDetails.CollectionsURI)));
        //Make a Rest api request for capacity
        var request = new RestRequest(tfsDetails.ProjectName + "/_apis/work/teamsettings/iterations/" + iterationId + "/Capacities?api-version=2.0-preview.1", Method.GET);
        request.Credentials = new NetworkCredential(tfsDetails.UserName, tfsDetails.PassWord, tfsDetails.Domain);
            var res = restClient.FollowRedirects;
           var response = restClient.Execute(request);


            //Deserialse the response to an object
            var sprintCapacity = JsonConvert.DeserializeObject<SprintCapacity>(response.Content);


        //Make a Rest api request for working days
        var workingDaysoffRequest = new RestRequest(tfsDetails.ProjectName + "/_apis/work/teamsettings");
        workingDaysoffRequest.Credentials = new NetworkCredential(tfsDetails.UserName, tfsDetails.PassWord, tfsDetails.Domain);
        var workingDaysoffResponse = (RestResponse)restClient.Execute(workingDaysoffRequest);
        //Deserialse the response to an object
        var teamsettings = JsonConvert.DeserializeObject<TeamSettings>(workingDaysoffResponse.Content);

        //Check if the sprint has ended completed            
        var count = 0;
        if (Convert.ToDateTime(sprint.endDate).Date > DateTime.Now.Date)
        {
            //If not change the sprintenddate to today
            sprint.endDate = DateTime.Now.Date;
        }
        for (DateTime dateIndex = Convert.ToDateTime(sprint.startDate); dateIndex.Date < sprint.endDate; dateIndex = dateIndex.AddDays(1).Date)
        {
            if (teamsettings.WorkingDays.Contains(dateIndex.DayOfWeek.ToString().ToLower()))
            {
                count++;
            }
        }
        decimal teamCapacity = 0;
        decimal daysoff = 0;
        //Calculate the capacity for entire days of sprint
        foreach (var item in sprintCapacity.Value)
        {
            //Caluclate the daysoff value only if its not null
            if (item.DaysOFF.Count != 0)
            {
                daysoff = (decimal)(Convert.ToDateTime(item.DaysOFF.Select(f => f.EndDate.Value.Date).FirstOrDefault()) - Convert.ToDateTime(item.DaysOFF.Select(e => e.StartDate.Value.Date).FirstOrDefault())).TotalDays;

            }
            var capacity = item.Activities.Select(c => c.CapacityPerDay).FirstOrDefault();
            var teamMembersprintCapacity = (capacity * (count - daysoff));
            teamCapacity += teamMembersprintCapacity;
        }

        return teamCapacity;
    }

    #region The following methods are for further reference
    //ProjectList,SprintDetails and TeammemberDetails can be obtained using rest apis
    //Create the appropriate classes to deserailise the data
    public void GetListoFProjectsRestAPI(TfSDetails tfsDetails)
    {
        RestClient restClient = new RestClient(new Uri(Convert.ToString(tfsDetails.CollectionsURI)));
        //Make a Rest api request for capacity
        RestRequest projectCollectionrequest = new RestRequest("/_apis/work/projects?api-version=2.0-preview.1", Method.GET);
        projectCollectionrequest.Credentials = new NetworkCredential(tfsDetails.UserName, tfsDetails.PassWord, tfsDetails.Domain);
        RestResponse projectListresponse = (RestResponse)restClient.Execute(projectCollectionrequest);
        //Deserialse the response to an object
    }

    public void GetSprintDetailsRestAPI(TfSDetails tfsDetails)
    {
        RestClient restClient = new RestClient(new Uri(Convert.ToString(tfsDetails.CollectionsURI)));
        //Make a Rest api request for capacity
        RestRequest sprintDetailsrequest = new RestRequest(tfsDetails.ProjectName + "_apis/work/teamsettings/iterations?api-version=2.0-preview.1", Method.GET);
        sprintDetailsrequest.Credentials = new NetworkCredential(tfsDetails.UserName, tfsDetails.PassWord, tfsDetails.Domain);
        RestResponse sprintDetailsresponse = (RestResponse)restClient.Execute(sprintDetailsrequest);
        //Deserialse the response to an object
    }
    public void GetTeamMemberDetailsRestAPI(TfSDetails tfsDetails, string iterationid)
    {
        RestClient restClient = new RestClient(new Uri(Convert.ToString(tfsDetails.CollectionsURI)));
        //Make a Rest api request for capacity
        RestRequest teamMemberdetailsRequest = new RestRequest(tfsDetails.ProjectName + "_apis/work/teamsettings/iterations/" + iterationid + "Capacities?api-version=2.0-preview.1");
        teamMemberdetailsRequest.Credentials = new NetworkCredential(tfsDetails.UserName, tfsDetails.PassWord, tfsDetails.Domain);
        RestResponse teamMemberdetailsResponse = (RestResponse)restClient.Execute(teamMemberdetailsRequest);

    }
    #endregion
}
}









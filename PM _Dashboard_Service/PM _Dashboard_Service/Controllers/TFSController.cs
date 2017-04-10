using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Web.Http;
using System;
using Microsoft.TeamFoundation.Server;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using PM__Dashboard_Service.Models;
using System.IO;
using Microsoft.TeamFoundation.ProcessConfiguration.Client;
using Microsoft.TeamFoundation;

namespace PMDashboardService
{
    public class TFSController : ApiController
    {

        private const string groupName = "Contributors";

        /// <summary>
        ///GetProjectList Method returns all the Projects
        /// For the Given Credentials
        /// </summary>
        /// <param name="tfsDetails"></param>
        /// <returns></returns>

        [Route("api/GetProjectList")]
        public IHttpActionResult GetProjectList([FromUri]TFSDetails tfsDetails)
        {
            try
            {
                //GetTemaProjectCollection intialises the TfsUri and authinticates the user
                var projectCollection = tfsDetails.GetTeamProjectCollection();
                ICommonStructureService commonStructservice = (ICommonStructureService)projectCollection.GetService(typeof(ICommonStructureService));

                //CommomnstructureService defines methods to work with Areas and Iterations
                var projectList = commonStructservice.ListAllProjects();

                Dictionary<string, string> listOfProjectnames = projectList.ToDictionary(x => x.Uri, x => x.Name);
                return Json(listOfProjectnames);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }


        }

        /// <summary>
        ///  GetSprintDetails Method returns the plannedhours
        /// Remaining Hours and Capacity
        ///for current sprint. 
        /// </summary>
        /// <param name="tfsDetails"></param>
        /// <returns></returns>

        [Route("api/GetSprintDetails")]
        public IHttpActionResult GetSprintDetails([FromUri]TFSDetails tfsDetails)
        {

            try
            {
                List<SprintDetails> sprintDetails = new List<SprintDetails>();

                //GetTemaProjectCollection intialises the TfsUri and authinticates the user 
                var projectCollection = tfsDetails.GetTeamProjectCollection();

                //CommomnstructureService defines  methods to work with Areas and Iterations
                ICommonStructureService commonStructureservice = (ICommonStructureService)projectCollection.GetService(typeof(ICommonStructureService));

                //GroupstructureService defines methods to acess TFS groups
                IGroupSecurityService groupSecurityservice = projectCollection.GetService<IGroupSecurityService>();

                //Represents the Work Item Tracking client connection to a server that is running Team Foundation Server.
                WorkItemStore store = projectCollection.GetService<WorkItemStore>();

                //ProjectCollection is used to get the  list of projects under the tfs server.
                ProjectCollection projectList = store.Projects;


                //Iterate Through the project list and acess the User Selected project
                foreach (Project project in projectList)
                {
                    if (project.Name == tfsDetails.ProjectName)
                    {
                        //Iterate through the  sprints and retrive the start and end dates of each sprint 
                        foreach (Node node in project.IterationRootNodes)
                        {

                            SprintDetails sprint = new SprintDetails();

                            //Through the nodeid acess the startdate,enddate and Sprint Name
                            var nodeId = commonStructureservice.GetNode(Convert.ToString(node.Uri));
                            sprint.SprintName = node.Name;
                            sprint.startDate = nodeId.StartDate;
                            sprint.endDate = nodeId.FinishDate;

                            //Query For tasks under the current sprint 
                            var queryFortasks = store.Query("Select * FROM WorkItem Where [System.WorkItemType] = 'Task'");
                            double totalHoursplanned = 0;
                            double totalHoursremaining = 0;

                            //Calculate the planned and Remaining hours foreach sprint
                            foreach (WorkItem task in queryFortasks)
                            {
                                //Calculate for each sprint
                                if (node.Path == task.IterationPath)
                                {
                                    var estimatedTime = task.Fields["Microsoft.VSTS.Scheduling.OriginalEstimate"].Value;
                                    var hoursRemaining = task.Fields["Microsoft.VSTS.Scheduling.RemainingWork"].Value;
                                    totalHoursremaining = totalHoursremaining + Convert.ToDouble(hoursRemaining);
                                    totalHoursplanned = totalHoursplanned + Convert.ToDouble(estimatedTime);
                                    sprint.planned = totalHoursplanned;
                                    sprint.remaining = totalHoursremaining;
                                    sprint.capacity = 0;
                                }
                            }
                            sprintDetails.Add(sprint);
                        }
                        break;
                    }
                }

                return Ok(sprintDetails);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);

            }
        }


        /// <summary>
        /// TeaMemberDetails Method gets Hoursurnt on daily basis for each  teamMember
        /// Under the Curretn Sprint
        /// </summary>
        /// <param name="tfsDetails"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TeamMembers")]
        public IHttpActionResult TeamMemberDetails([FromUri]TFSDetails tfsDetails)
        {

            try
            {
                //GetTemaProjectCollection intialises the TfsUri and authinticates the 
                var projectCollection = tfsDetails.GetTeamProjectCollection();

                ////CommomnstructureService defines  methods to work with Areas and Iterations
                ICommonStructureService commonStructservice = (ICommonStructureService)projectCollection.GetService(typeof(ICommonStructureService));

                //GroupstructureService defines methods to acess tfs groups
                IGroupSecurityService groupSecurityservice = projectCollection.GetService<IGroupSecurityService>();

                //TeamSettings Configuration to acess current and Backlog iteration paths
                TeamSettingsConfigurationService teamSettingsconfig = projectCollection.GetService<TeamSettingsConfigurationService>();
                var teamSettings = teamSettingsconfig.GetTeamConfigurationsForUser(new[] { tfsDetails.ProjectURI });

                //Get the current Iteration Paths 
                var currentIterationpath = teamSettings.Select(c => c.TeamSettings.CurrentIterationPath).FirstOrDefault();
                List<TeamMember> teamMembersdetail = new List<TeamMember>();

                //Represents the Work Item Tracking client connection to a server that is running Team Foundation Server.
                WorkItemStore store = projectCollection.GetService<WorkItemStore>();
                ProjectCollection projectList = store.Projects;

                //List the groups under the project
                //Get the teamMemberNames
                var groupList = groupSecurityservice.ListApplicationGroups(tfsDetails.ProjectURI);
                var group = groupList.FirstOrDefault(Project => Project.DisplayName.Contains(groupName));
                Identity sids = groupSecurityservice.ReadIdentity(SearchFactor.Sid, group.Sid, QueryMembership.Expanded);

                // convert to a list
                List<Identity> contributors = groupSecurityservice.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.Expanded).ToList();
                List<string> teamMembers = contributors.Select(c => c.DisplayName).ToList();


                //Loop to add the team Member names
                foreach (string teammember in teamMembers)
                {
                    if (teammember == string.Format(tfsDetails.ProjectName + " Team"))
                        continue;
                    teamMembersdetail.Add(new TeamMember() { Name = teammember });
                }


                //Query for tasks under current sprint
                var queryFortasks = store.Query("Select * FROM WorkItem Where [System.WorkItemType] = 'Task'");


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
                                var nodeid = commonStructservice.GetNode(Convert.ToString(node.Uri));
                                DateTime startDate = Convert.ToDateTime(nodeid.StartDate);
                                DateTime enDate = DateTime.Today;

                                //Loop to add the range of Sprint dates
                                foreach (TeamMember teamMember in teamMembersdetail)
                                {
                                    teamMember.HoursBurnt = new Dictionary<DateTime?, double?>();
                                    for (DateTime test = startDate.Date; test <= enDate; test = test.AddDays(1))
                                    {
                                        teamMember.HoursBurnt.Add(test.Date, 0);
                                    }

                                }


                                //Loop through the tasks for a given project and then through each revision
                                foreach (WorkItem task in queryFortasks)
                                {

                                    foreach (Revision revision in task.Revisions)
                                    {
                                        //Check which user has changed the state of a task
                                        var revisionOwner = revision.Fields["System.ChangedBy"].Value;

                                        var teammember = teamMembersdetail.Where(x => x.Name == revisionOwner.ToString()).FirstOrDefault();
                                        if (teammember != null)
                                        {
                                            DateTime reviseddate = Convert.ToDateTime(revision.Fields["System.ChangedDate"].Value);
                                            var oldValue = revision.Fields["Microsoft.VSTS.Scheduling.CompletedWork"].OriginalValue;
                                            var updatedValue = revision.Fields["Microsoft.VSTS.Scheduling.CompletedWork"].Value;

                                            //Total number of hours burnt is the updatedValue-oldValue
                                            var hoursBurnt = System.Math.Abs(Convert.ToDouble(updatedValue) - Convert.ToDouble(oldValue));

                                            //Update the Hours burnt for the given revised date.
                                            if (teammember.HoursBurnt.ContainsKey(reviseddate.Date))
                                                teammember.HoursBurnt[reviseddate.Date] = Convert.ToDouble(hoursBurnt);
                                        }

                                    }

                                }
                            }
                        }
                        break;
                    }

                }
                return Ok(teamMembersdetail);
            }
            catch (TeamFoundationServerUnauthorizedException exception)
            {
                
               return Ok("Unauthorized");
                
                
            }

        }
    }
}










using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic
{
    public interface IPMDashboardBusinessLogic
    {
        Task< Dictionary<string, string> >GetProjectList(TfSDetails tfsDetails);
        Task <List<SprintDetails>> GetSprintDetails(TfSDetails tfsDetails);
        Task <List<TeamMember>> TeamMemberDetails(TfSDetails tfsDetails);
    }
}

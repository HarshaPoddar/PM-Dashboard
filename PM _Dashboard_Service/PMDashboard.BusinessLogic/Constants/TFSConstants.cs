using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDashboard.BusinessLogic.Constants
{
    public class TFSConstants
    {
        public const string GroupName = "Contributors";
        public const string WorkItemQuery = "Select * FROM WorkItem Where [System.WorkItemType] = 'Task'";
        public const string OriginalEstimate = "Microsoft.VSTS.Scheduling.OriginalEstimate";
        public const string RemainingWork = "Microsoft.VSTS.Scheduling.RemainingWork";
        public const string CompletedWork = "Microsoft.VSTS.Scheduling.CompletedWork";
        public const string ChangedDate = "System.ChangedDate";
    }
}

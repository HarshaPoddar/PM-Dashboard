using System;
using Microsoft.Office.Tools.Ribbon;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace PM_Dashboard
{


    public partial class ThisAddIn
    {
        private PM_DashboardControl pm_DashboardControl;
        private Microsoft.Office.Tools.CustomTaskPane pmDashboardValue;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            pm_DashboardControl = new PM_DashboardControl();
            pmDashboardValue = this.CustomTaskPanes.Add(pm_DashboardControl, "PM User Form");
            pmDashboardValue.VisibleChanged += new EventHandler(pmDashboardValue_VisibleChanged);
            //pmDashboardValue.Height = 200;
            pmDashboardValue.Width = 500;

        }

        private void pmDashboardValue_VisibleChanged(object sender, EventArgs e)
        {
            Globals.Ribbons.ProjectDetails.Launch.Checked =
                pmDashboardValue.Visible;
        }
        public Microsoft.Office.Tools.CustomTaskPane TaskPane
        {
            get
            {
                return pmDashboardValue;
            }
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}

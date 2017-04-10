using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace PM_Dashboard
{
    public partial class ProjectDetails
    {
        private void toggleButton1_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = ((RibbonToggleButton)sender).Checked;

        }
    }
}

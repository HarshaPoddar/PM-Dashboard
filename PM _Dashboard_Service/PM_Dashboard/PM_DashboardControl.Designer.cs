namespace PM_Dashboard
{
    partial class PM_DashboardControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.txtServerLink = new System.Windows.Forms.TextBox();
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.PassworderrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.userNameErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.projectListerrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.serverLinkErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.CredentailsValidation = new System.Windows.Forms.ErrorProvider(this.components);
            this.browseErrorprovider = new System.Windows.Forms.ErrorProvider(this.components);
            this.SelectProjectErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.GetProjectButton = new System.Windows.Forms.Button();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.UserNameLabel = new System.Windows.Forms.Label();
            this.ServerLinkLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Browse = new System.Windows.Forms.Button();
            this.SelectTBSFile = new System.Windows.Forms.Label();
            this.ViewDetails = new System.Windows.Forms.Button();
            this.SelectProjectLabel = new System.Windows.Forms.Label();
            this.cmbProjectList = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PassworderrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userNameErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectListerrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverLinkErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CredentailsValidation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.browseErrorprovider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectProjectErrorProvider)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // txtServerLink
            // 
            this.txtServerLink.Location = new System.Drawing.Point(128, 17);
            this.txtServerLink.Name = "txtServerLink";
            this.txtServerLink.Size = new System.Drawing.Size(205, 20);
            this.txtServerLink.TabIndex = 9;
            this.toolTip2.SetToolTip(this.txtServerLink, "Enter the TFS server link for example\"https://{instance}/DefaultCollection\"");
            this.txtServerLink.Validating += new System.ComponentModel.CancelEventHandler(this.ServerLinkTextbox_Validating);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(128, 64);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(205, 20);
            this.txtUserName.TabIndex = 10;
            this.toolTip3.SetToolTip(this.txtUserName, "Enter the Tfs User Name ");
            this.txtUserName.Validating += new System.ComponentModel.CancelEventHandler(this.UserNameTextbox_Validating);
            // 
            // txtPassword
            // 
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtPassword.Location = new System.Drawing.Point(128, 108);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(205, 20);
            this.txtPassword.TabIndex = 11;
            this.toolTip4.SetToolTip(this.txtPassword, "Enter the TFS password");
            this.txtPassword.Validating += new System.ComponentModel.CancelEventHandler(this.PasswordTextbox_Validating);
            // 
            // PassworderrorProvider
            // 
            this.PassworderrorProvider.BlinkRate = 0;
            this.PassworderrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.PassworderrorProvider.ContainerControl = this;
            // 
            // userNameErrorProvider
            // 
            this.userNameErrorProvider.BlinkRate = 0;
            this.userNameErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.userNameErrorProvider.ContainerControl = this;
            // 
            // projectListerrorProvider
            // 
            this.projectListerrorProvider.ContainerControl = this;
            // 
            // serverLinkErrorProvider
            // 
            this.serverLinkErrorProvider.BlinkRate = 0;
            this.serverLinkErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.serverLinkErrorProvider.ContainerControl = this;
            // 
            // CredentailsValidation
            // 
            this.CredentailsValidation.BlinkRate = 0;
            this.CredentailsValidation.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.CredentailsValidation.ContainerControl = this;
            // 
            // browseErrorprovider
            // 
            this.browseErrorprovider.ContainerControl = this;
            // 
            // SelectProjectErrorProvider
            // 
            this.SelectProjectErrorProvider.ContainerControl = this;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.GetProjectButton);
            this.panel1.Controls.Add(this.PasswordLabel);
            this.panel1.Controls.Add(this.txtPassword);
            this.panel1.Controls.Add(this.UserNameLabel);
            this.panel1.Controls.Add(this.ServerLinkLabel);
            this.panel1.Controls.Add(this.txtUserName);
            this.panel1.Controls.Add(this.txtServerLink);
            this.panel1.Location = new System.Drawing.Point(25, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 203);
            this.panel1.TabIndex = 15;
            // 
            // GetProjectButton
            // 
            this.GetProjectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.GetProjectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GetProjectButton.ForeColor = System.Drawing.Color.White;
            this.GetProjectButton.Location = new System.Drawing.Point(128, 163);
            this.GetProjectButton.Name = "GetProjectButton";
            this.GetProjectButton.Size = new System.Drawing.Size(111, 23);
            this.GetProjectButton.TabIndex = 12;
            this.GetProjectButton.Text = "GET PROJECT";
            this.GetProjectButton.UseVisualStyleBackColor = false;
            this.GetProjectButton.Click += new System.EventHandler(this.GetProjectsList);
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordLabel.Location = new System.Drawing.Point(19, 115);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(69, 15);
            this.PasswordLabel.TabIndex = 15;
            this.PasswordLabel.Text = "Password";
            // 
            // UserNameLabel
            // 
            this.UserNameLabel.AutoSize = true;
            this.UserNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserNameLabel.Location = new System.Drawing.Point(19, 67);
            this.UserNameLabel.Name = "UserNameLabel";
            this.UserNameLabel.Size = new System.Drawing.Size(79, 15);
            this.UserNameLabel.TabIndex = 14;
            this.UserNameLabel.Text = "User Name";
            // 
            // ServerLinkLabel
            // 
            this.ServerLinkLabel.AutoSize = true;
            this.ServerLinkLabel.BackColor = System.Drawing.SystemColors.Control;
            this.ServerLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerLinkLabel.Location = new System.Drawing.Point(19, 17);
            this.ServerLinkLabel.Name = "ServerLinkLabel";
            this.ServerLinkLabel.Size = new System.Drawing.Size(99, 13);
            this.ServerLinkLabel.TabIndex = 13;
            this.ServerLinkLabel.Text = "TFS Server Link";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Browse);
            this.panel2.Controls.Add(this.SelectTBSFile);
            this.panel2.Controls.Add(this.ViewDetails);
            this.panel2.Controls.Add(this.SelectProjectLabel);
            this.panel2.Controls.Add(this.cmbProjectList);
            this.panel2.Location = new System.Drawing.Point(25, 255);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(352, 195);
            this.panel2.TabIndex = 16;
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Browse.Location = new System.Drawing.Point(136, 74);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(79, 23);
            this.Browse.TabIndex = 16;
            this.Browse.Text = "BROWSE";
            this.Browse.UseVisualStyleBackColor = false;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // SelectTBSFile
            // 
            this.SelectTBSFile.AutoSize = true;
            this.SelectTBSFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectTBSFile.Location = new System.Drawing.Point(11, 79);
            this.SelectTBSFile.Name = "SelectTBSFile";
            this.SelectTBSFile.Size = new System.Drawing.Size(101, 13);
            this.SelectTBSFile.TabIndex = 19;
            this.SelectTBSFile.Text = "Import TBS Data";
            // 
            // ViewDetails
            // 
            this.ViewDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ViewDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ViewDetails.ForeColor = System.Drawing.Color.White;
            this.ViewDetails.Location = new System.Drawing.Point(136, 119);
            this.ViewDetails.Name = "ViewDetails";
            this.ViewDetails.Size = new System.Drawing.Size(103, 23);
            this.ViewDetails.TabIndex = 17;
            this.ViewDetails.Text = "VIEW DETAILS";
            this.ViewDetails.UseVisualStyleBackColor = false;
            this.ViewDetails.Click += new System.EventHandler(this.ViewDetails_Click);
            // 
            // SelectProjectLabel
            // 
            this.SelectProjectLabel.AutoSize = true;
            this.SelectProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectProjectLabel.Location = new System.Drawing.Point(11, 28);
            this.SelectProjectLabel.Name = "SelectProjectLabel";
            this.SelectProjectLabel.Size = new System.Drawing.Size(87, 13);
            this.SelectProjectLabel.TabIndex = 18;
            this.SelectProjectLabel.Text = "Select Project";
            // 
            // cmbProjectList
            // 
            this.cmbProjectList.FormattingEnabled = true;
            this.cmbProjectList.Location = new System.Drawing.Point(136, 20);
            this.cmbProjectList.Name = "cmbProjectList";
            this.cmbProjectList.Size = new System.Drawing.Size(205, 21);
            this.cmbProjectList.TabIndex = 15;
            this.cmbProjectList.Validating += new System.ComponentModel.CancelEventHandler(this.SelectProjectListbox_Validating);
            // 
            // PM_DashboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(500, 0);
            this.AutoScrollMinSize = new System.Drawing.Size(500, 0);
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "PM_DashboardControl";
            this.Size = new System.Drawing.Size(363, 436);
            this.Load += new System.EventHandler(this.PM_DashboardControl_Load);
            this.Leave += new System.EventHandler(this.PM_DashboardControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PassworderrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userNameErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectListerrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverLinkErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CredentailsValidation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.browseErrorprovider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SelectProjectErrorProvider)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ErrorProvider PassworderrorProvider;
        private System.Windows.Forms.ErrorProvider userNameErrorProvider;
        private System.Windows.Forms.ErrorProvider projectListerrorProvider;
        private System.Windows.Forms.ErrorProvider serverLinkErrorProvider;
        private System.Windows.Forms.ErrorProvider CredentailsValidation;
        private System.Windows.Forms.ErrorProvider browseErrorprovider;
        private System.Windows.Forms.ErrorProvider SelectProjectErrorProvider;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Label SelectTBSFile;
        private System.Windows.Forms.Button ViewDetails;
        private System.Windows.Forms.Label SelectProjectLabel;
        private System.Windows.Forms.ComboBox cmbProjectList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button GetProjectButton;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label UserNameLabel;
        private System.Windows.Forms.Label ServerLinkLabel;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtServerLink;
    }
}

namespace CSProject
{
    partial class MainForm
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
            //Ensures that any ESRI libraries that have been used are unloaded in the correct order. 
            //Failure to do this may result in random crashes on exit due to the operating system unloading 
            //the libraries in the incorrect order. 
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNewDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpenDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuExitApp = new System.Windows.Forms.ToolStripMenuItem();
            this.midataoperator = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.miSelectPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.咬合设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miStartSnap = new System.Windows.Forms.ToolStripMenuItem();
            this.miProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.miGeometry = new System.Windows.Forms.ToolStripMenuItem();
            this.miPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowTime = new System.Windows.Forms.ToolStripMenuItem();
            this.AreaStatistics = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.axMapControl1 = new ESRI.ArcGIS.Controls.AxMapControl();
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBarXY = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.midataoperator,
            this.咬合设置ToolStripMenuItem,
            this.miProject,
            this.AreaStatistics,
            this.miDeleteLayer});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(859, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNewDoc,
            this.menuOpenDoc,
            this.menuSaveDoc,
            this.menuSaveAs,
            this.menuSeparator,
            this.menuExitApp});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(39, 21);
            this.menuFile.Text = "File";
            // 
            // menuNewDoc
            // 
            this.menuNewDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuNewDoc.Image")));
            this.menuNewDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuNewDoc.Name = "menuNewDoc";
            this.menuNewDoc.Size = new System.Drawing.Size(180, 22);
            this.menuNewDoc.Text = "New Document";
            this.menuNewDoc.Click += new System.EventHandler(this.menuNewDoc_Click);
            // 
            // menuOpenDoc
            // 
            this.menuOpenDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuOpenDoc.Image")));
            this.menuOpenDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuOpenDoc.Name = "menuOpenDoc";
            this.menuOpenDoc.Size = new System.Drawing.Size(180, 22);
            this.menuOpenDoc.Text = "Open Document...";
            this.menuOpenDoc.Click += new System.EventHandler(this.menuOpenDoc_Click);
            // 
            // menuSaveDoc
            // 
            this.menuSaveDoc.Image = ((System.Drawing.Image)(resources.GetObject("menuSaveDoc.Image")));
            this.menuSaveDoc.ImageTransparentColor = System.Drawing.Color.White;
            this.menuSaveDoc.Name = "menuSaveDoc";
            this.menuSaveDoc.Size = new System.Drawing.Size(180, 22);
            this.menuSaveDoc.Text = "SaveDocument";
            this.menuSaveDoc.Click += new System.EventHandler(this.menuSaveDoc_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(180, 22);
            this.menuSaveAs.Text = "Save As...";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // menuSeparator
            // 
            this.menuSeparator.Name = "menuSeparator";
            this.menuSeparator.Size = new System.Drawing.Size(177, 6);
            // 
            // menuExitApp
            // 
            this.menuExitApp.Name = "menuExitApp";
            this.menuExitApp.Size = new System.Drawing.Size(180, 22);
            this.menuExitApp.Text = "Exit";
            this.menuExitApp.Click += new System.EventHandler(this.menuExitApp_Click);
            // 
            // midataoperator
            // 
            this.midataoperator.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddPolygon,
            this.miSelectPolygon,
            this.miEditPolygon});
            this.midataoperator.Name = "midataoperator";
            this.midataoperator.Size = new System.Drawing.Size(68, 21);
            this.midataoperator.Text = "数据操作";
            // 
            // miAddPolygon
            // 
            this.miAddPolygon.Name = "miAddPolygon";
            this.miAddPolygon.Size = new System.Drawing.Size(148, 22);
            this.miAddPolygon.Text = "添加polygon";
            this.miAddPolygon.Click += new System.EventHandler(this.miPolygonToolStripMenuItem_Click);
            // 
            // miSelectPolygon
            // 
            this.miSelectPolygon.Name = "miSelectPolygon";
            this.miSelectPolygon.Size = new System.Drawing.Size(148, 22);
            this.miSelectPolygon.Text = "选择polygon";
            this.miSelectPolygon.Click += new System.EventHandler(this.miSelectPolygon_Click);
            // 
            // miEditPolygon
            // 
            this.miEditPolygon.Name = "miEditPolygon";
            this.miEditPolygon.Size = new System.Drawing.Size(148, 22);
            this.miEditPolygon.Text = "编辑polygon";
            this.miEditPolygon.Click += new System.EventHandler(this.miEditPolygon_Click);
            // 
            // 咬合设置ToolStripMenuItem
            // 
            this.咬合设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miStartSnap});
            this.咬合设置ToolStripMenuItem.Name = "咬合设置ToolStripMenuItem";
            this.咬合设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.咬合设置ToolStripMenuItem.Text = "咬合设置";
            // 
            // miStartSnap
            // 
            this.miStartSnap.Name = "miStartSnap";
            this.miStartSnap.Size = new System.Drawing.Size(124, 22);
            this.miStartSnap.Text = "开始咬合";
            this.miStartSnap.Click += new System.EventHandler(this.miStartSnap_Click);
            // 
            // miProject
            // 
            this.miProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPoint,
            this.miGeometry,
            this.miPolygon,
            this.miShowTime});
            this.miProject.Name = "miProject";
            this.miProject.Size = new System.Drawing.Size(84, 21);
            this.miProject.Text = "Project函数";
            // 
            // miPoint
            // 
            this.miPoint.Name = "miPoint";
            this.miPoint.Size = new System.Drawing.Size(157, 22);
            this.miPoint.Text = "显示一个点";
            this.miPoint.Click += new System.EventHandler(this.miPoint_Click);
            // 
            // miGeometry
            // 
            this.miGeometry.Name = "miGeometry";
            this.miGeometry.Size = new System.Drawing.Size(157, 22);
            this.miGeometry.Text = "显示Geometry";
            this.miGeometry.Click += new System.EventHandler(this.miGeometry_Click);
            // 
            // miPolygon
            // 
            this.miPolygon.Name = "miPolygon";
            this.miPolygon.Size = new System.Drawing.Size(157, 22);
            this.miPolygon.Text = "显示面";
            this.miPolygon.Click += new System.EventHandler(this.miPolygon_Click);
            // 
            // miShowTime
            // 
            this.miShowTime.Name = "miShowTime";
            this.miShowTime.Size = new System.Drawing.Size(157, 22);
            this.miShowTime.Text = "ShowTime";
            this.miShowTime.Click += new System.EventHandler(this.miShowTime_Click);
            // 
            // AreaStatistics
            // 
            this.AreaStatistics.Name = "AreaStatistics";
            this.AreaStatistics.Size = new System.Drawing.Size(68, 21);
            this.AreaStatistics.Text = "面积统计";
            this.AreaStatistics.Click += new System.EventHandler(this.AreaStatistics_Click);
            // 
            // miDeleteLayer
            // 
            this.miDeleteLayer.Name = "miDeleteLayer";
            this.miDeleteLayer.Size = new System.Drawing.Size(143, 21);
            this.miDeleteLayer.Text = "删除Clip_polygon图层";
            this.miDeleteLayer.Click += new System.EventHandler(this.miDeleteLayer_Click);
            // 
            // axMapControl1
            // 
            this.axMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMapControl1.Location = new System.Drawing.Point(191, 53);
            this.axMapControl1.Name = "axMapControl1";
            this.axMapControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl1.OcxState")));
            this.axMapControl1.Size = new System.Drawing.Size(668, 466);
            this.axMapControl1.TabIndex = 2;
            this.axMapControl1.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.axMapControl1_OnMouseDown);
            this.axMapControl1.OnMouseUp += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseUpEventHandler(this.axMapControl1_OnMouseUp);
            this.axMapControl1.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.axMapControl1_OnMouseMove);
            this.axMapControl1.OnDoubleClick += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnDoubleClickEventHandler(this.axMapControl1_OnDoubleClick);
            this.axMapControl1.OnKeyDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnKeyDownEventHandler(this.axMapControl1_OnKeyDown);
            this.axMapControl1.OnMapReplaced += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMapReplacedEventHandler(this.axMapControl1_OnMapReplaced);
            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl1.Location = new System.Drawing.Point(0, 25);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(859, 28);
            this.axToolbarControl1.TabIndex = 3;
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.axTOCControl1.Location = new System.Drawing.Point(3, 53);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(188, 466);
            this.axTOCControl1.TabIndex = 4;
            this.axTOCControl1.OnMouseDown += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseDownEventHandler(this.axTOCControl1_OnMouseDown);
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(455, 251);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 5;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 53);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 488);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarXY});
            this.statusStrip1.Location = new System.Drawing.Point(3, 519);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(856, 22);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusBar1";
            // 
            // statusBarXY
            // 
            this.statusBarXY.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.statusBarXY.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBarXY.Name = "statusBarXY";
            this.statusBarXY.Size = new System.Drawing.Size(57, 17);
            this.statusBarXY.Text = "Test 123";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 541);
            this.Controls.Add(this.axLicenseControl1);
            this.Controls.Add(this.axMapControl1);
            this.Controls.Add(this.axTOCControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ArcEngine Controls Application";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNewDoc;
        private System.Windows.Forms.ToolStripMenuItem menuOpenDoc;
        private System.Windows.Forms.ToolStripMenuItem menuSaveDoc;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem menuExitApp;
        private System.Windows.Forms.ToolStripSeparator menuSeparator;
        public ESRI.ArcGIS.Controls.AxMapControl axMapControl1;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBarXY;
        private System.Windows.Forms.ToolStripMenuItem midataoperator;
        private System.Windows.Forms.ToolStripMenuItem miAddPolygon;
        private System.Windows.Forms.ToolStripMenuItem miSelectPolygon;
        private System.Windows.Forms.ToolStripMenuItem miEditPolygon;
        private System.Windows.Forms.ToolStripMenuItem 咬合设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miStartSnap;
        private System.Windows.Forms.ToolStripMenuItem miProject;
        private System.Windows.Forms.ToolStripMenuItem miPoint;
        private System.Windows.Forms.ToolStripMenuItem miGeometry;
        private System.Windows.Forms.ToolStripMenuItem miPolygon;
        private System.Windows.Forms.ToolStripMenuItem miShowTime;
        private System.Windows.Forms.ToolStripMenuItem AreaStatistics;
        private System.Windows.Forms.ToolStripMenuItem miDeleteLayer;
    }
}


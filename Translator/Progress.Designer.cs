namespace Translator
{
  partial class Progress
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Progress));
      this.trayNotify = new System.Windows.Forms.NotifyIcon(this.components);
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.logOutput = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // trayNotify
      // 
      this.trayNotify.Icon = ((System.Drawing.Icon)(resources.GetObject("trayNotify.Icon")));
      this.trayNotify.Text = "...";
      this.trayNotify.Visible = true;
      this.trayNotify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayNotify_MouseDoubleClick);
      // 
      // progressBar1
      // 
      this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar1.Location = new System.Drawing.Point(13, 143);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(489, 23);
      this.progressBar1.TabIndex = 0;
      // 
      // logOutput
      // 
      this.logOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logOutput.Location = new System.Drawing.Point(12, 12);
      this.logOutput.Multiline = true;
      this.logOutput.Name = "logOutput";
      this.logOutput.ReadOnly = true;
      this.logOutput.Size = new System.Drawing.Size(490, 125);
      this.logOutput.TabIndex = 1;
      // 
      // Progress
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(514, 178);
      this.Controls.Add(this.logOutput);
      this.Controls.Add(this.progressBar1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "Progress";
      this.Text = "Extracting file information...";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Progress_FormClosing);
      this.Load += new System.EventHandler(this.Progress_Load);
      this.Resize += new System.EventHandler(this.Progress_Resize);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NotifyIcon trayNotify;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.TextBox logOutput;
  }
}


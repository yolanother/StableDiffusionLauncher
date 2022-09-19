namespace StableDiffusionLauncher
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
            this.richTextBoxConsole = new ConsoleControl.ConsoleControl();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // richTextBoxConsole
            // 
            this.richTextBoxConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxConsole.IsInputEnabled = true;
            this.richTextBoxConsole.Location = new System.Drawing.Point(-1, 1);
            this.richTextBoxConsole.Name = "richTextBoxConsole";
            this.richTextBoxConsole.SendKeyboardCommandsToProcess = false;
            this.richTextBoxConsole.ShowDiagnostics = false;
            this.richTextBoxConsole.Size = new System.Drawing.Size(802, 448);
            this.richTextBoxConsole.TabIndex = 0;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBoxConsole);
            this.Name = "MainForm";
            this.Text = "Stable Diffusion Service";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private ConsoleControl.ConsoleControl richTextBoxConsole;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}


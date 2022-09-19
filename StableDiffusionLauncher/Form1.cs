using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StableDiffusionLauncher
{
    public partial class MainForm : Form
    {
        private Process currentProcess;

        public MainForm()
        {
            InitializeComponent();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Runs a process
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments)
        {
            // Create the process start info
            var processStartInfo = new ProcessStartInfo(fileName, arguments);

            // Set the options
            processStartInfo.UseShellExecute = false;
            processStartInfo.ErrorDialog = false;
            processStartInfo.CreateNoWindow = true;

            // Specify redirection
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;

            // Create the process
            currentProcess = new Process();
            currentProcess.EnableRaisingEvents = true;
            currentProcess.StartInfo = processStartInfo;
            currentProcess.Exited += new EventHandler(currentProcess_Exited);

            // Try to start the process
            try
            {
                bool processStarted = currentProcess.Start();
            }
            catch (Exception e)
            {
                // We failed to start the process. Write the output (if we have diagnostics on).
                if (ShowDiagnostics)
                    WriteOutput("Failed: " + e.ToString() + Environment.NewLine, Color.Red);
                return;
            }

            // Store name and arguments
            currentProcessFileName = fileName;
            currentProcessArguments = arguments;
            // Create the readers and writers
            inputWriter = currentProcess.StandardInput;
            outputReader = TextReader.Synchronized(currentProcess.StandardOutput);
            errorReader = TextReader.Synchronized(currentProcess.StandardError);

            // Run the output and error workers
            outputWorker.RunWorkerAsync();
            errorWorker.RunWorkerAsync();

            // If we enable input, make the control not read only
            if (IsInputEnabled)
                richTextBoxConsole.ReadOnly = false;
        }
}

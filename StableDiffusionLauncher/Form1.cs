using JsonConfig.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StableDiffusionLauncher
{
    public partial class MainForm : Form
    {
        private Process currentProcess;
        private Config config = new Config();
        private JsonSettingsDialog jsonSettingsDialog;
        private int pythonPid = -1;

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
            Application.ApplicationExit += Application_ApplicationExit;
            richTextBoxConsole.OnConsoleOutput += RichTextBoxConsole_OnConsoleOutput;

            richTextBoxConsole.ProcessInterface.OnProcessExit += ProcessInterface_OnProcessExit;
            jsonSettingsDialog = new JsonSettingsDialog();
            config.Load();
            jsonSettingsDialog.Settings.SetConfig(config);
        }

        void Status(string text)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke((MethodInvoker)delegate { Status(text); });
                return;
            }

            toolStripStatusLabel.Text = text;
            toolStripStatusLabelPythonPID.Text = "Python Process: " + (pythonPid >= 0 ? "" + pythonPid : "Unknown");
        }

        private void ProcessInterface_OnProcessExit(object sender, ConsoleControlAPI.ProcessEventArgs args)
        {
            Status("Stable diffusion has stopped.");
            Task.Delay(60000).ContinueWith(t => richTextBoxConsole.Invoke((MethodInvoker)delegate {
            StartSimpleDiffusion();
            }));
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            richTextBoxConsole.ProcessInterface.OnProcessExit -= ProcessInterface_OnProcessExit;
            StopProcess();

        }

        private void RichTextBoxConsole_OnConsoleOutput(object sender, ConsoleControl.ConsoleEventArgs args)
        {
            var content = args.Content;
            if (content.Length >= 64) content = content.Substring(0, 62);
            notifyIcon.Text = content;
            Status(args.Content);
            if (args.Content.Contains("HTTPError"))
            {
                RestartSimpleDiffusion();
            }

            var m = Regex.Match(args.Content, @"\*\* SERVICE PID: (\d+) \*\*");
            if(m.Success)
            {
                pythonPid = int.Parse(m.Groups[1].Value);
                Status("Found process id for python!");
            }
        }

        private void restartSimpleDiffusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxConsole.ProcessInterface.OnProcessExit += ProcessInterface_OnProcessExit;
            RestartSimpleDiffusion();
        }

        private void RestartSimpleDiffusion()
        {
            StopProcess();
            StartSimpleDiffusion();
        }

        void StopProcess()
        {
            if (richTextBoxConsole.ProcessInterface.Process == null) return;

            if (pythonPid != -1) KillProcessAndChildren(pythonPid);
            pythonPid = -1;

            try
            {
                var id = richTextBoxConsole.ProcessInterface.Process.Id;
                Status($"Stopping simple diffusion [pid: {id}...");
                KillProcessAndChildren(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void StartSimpleDiffusion()
        {
            CleanProcesses();
            Status("Starting simple diffusion...");
            try
            {
                richTextBoxConsole.StartProcess(config.StableDiffusionBinary, config.Arguments);
            }
            catch (Exception e)
            {
                Status(e.Message);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jsonSettingsDialog.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxConsole.StopProcess();
            Application.Exit();
        }

        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        private static void KillProcessAndChildren(int pid)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/pid {pid} /f /t",
                CreateNoWindow = true
            }).WaitForExit();
        }

        private void stopSimpleDiffusionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxConsole.ProcessInterface.OnProcessExit -= ProcessInterface_OnProcessExit;
            StopProcess();
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            ((ToolStripMenuItem)sender).Checked = TopMost;
        }

        public void CleanProcesses()
        {
            foreach(var process in FindProcesses())
            {
                var childProcesses = GetChildProcesses(process);
                foreach(var childProcess in childProcesses)
                {
                    Status(childProcess.ProcessName);
                }
            }
            foreach(var process in Process.GetProcessesByName("Python"))
            {
                Status(process.ProcessName);
            }
        }

        public static Process[] FindProcesses()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var processes = Process.GetProcessesByName(processName);
            return processes;
        }

        public static IList<Process> GetChildProcesses(Process process) => new ManagementObjectSearcher(
                $"Select * From Win32_Process Where ParentProcessID={process.Id}")
            .Get()
            .Cast<ManagementObject>()
            .Select(mo =>
                Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])))
            .ToList();
    }
}

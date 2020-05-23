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
using Microsoft.Win32;

namespace CondorMapMover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string condorPath = (string)Registry.GetValue("HKEY_CLASSES_ROOT\\cndr2\\DefaultIcon", "", "Condor not found");
            if (condorPath != null) condorPath = condorPath.Replace("\\Condor.exe", "");
            condorpathEl.Text = condorPath;
            newpathEl.Text = Directory.GetCurrentDirectory();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (condorpathFile.ShowDialog() == DialogResult.OK)
            {
                condorpathEl.Text = condorpathFile.SelectedPath;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (newpathFile.ShowDialog() == DialogResult.OK)
            {
                newpathEl.Text = newpathFile.SelectedPath;
            }
        }

        private void condorpathEl_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string mapPath = condorpathEl.Text + "\\landscapes\\";
            if (!Directory.Exists(mapPath)) return;

            string[] subdirectoryEntries = Directory.GetDirectories(mapPath);
            foreach (string subdirectory in subdirectoryEntries)
            {
                string map = subdirectory.Replace(mapPath, "");
                listBox1.Items.Add(map);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void log(string text)
        {
            logEl.AppendText(text + "\r\n");
        }

        private void go_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                log("error: no maps found in this location");
                return;
            }

            if (!Directory.Exists(newpathEl.Text))
            {
                log("error: target directory not found");
                return;
            }

            if (listBox1.SelectedItems.Count == 0)
            {
                log("select at least one map");
                return;
            }

            foreach (string map in listBox1.SelectedItems) moveMap(map);

            log("done");
        }

        private void moveMap(string map)
        {
            log("copy map " + map + "please wait...");       

            string sourceDirectory = condorpathEl.Text + @"\landscapes\" + map;
            string destinationDirectory = newpathEl.Text + @"\" + map;
            try
            {
                DirectoryCopy(sourceDirectory, destinationDirectory, true);
                log("finished copying " + map);
}
            catch (Exception e)
            {
                log(e.Message);
            }

            log("delete source map");
            try
            {
                Directory.Delete(sourceDirectory, true);
                log("deleted " + sourceDirectory);
            }
            catch (Exception e)
            {
                log(e.Message);
            }

            log("creating mklink");
            makeMklink(sourceDirectory, destinationDirectory);
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                log("copy file " + file);
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void makeMklink(string source, string dest)
        {
            var psi = new ProcessStartInfo("cmd.exe", " /C mklink /j \"" + source + "\" \"" + dest + "\"");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi).WaitForExit();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.tobiaswust.de");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/TobiasWust/Condor-MapMover/");
        }
    }
}

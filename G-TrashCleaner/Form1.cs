using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace G_TrashCleaner
{
    public partial class Form1 : Form
    {
        // DWM API
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableComposition(bool enable);

        // Get all files in the temp directory
        private string[] files = new[]
        {
            Path.GetTempPath(),                                                     // User Temp Folder
            @"C:\Windows\Temp",                                                     // Windows Temp Folder
            @"C:\Windows\SoftwareDistribution\Download",                            // Windows Update Cache
            @"C:\Windows\Prefetch",                                                 // Windows Prefetch
            @"C:\Users\Public\Documents\Microsoft Shared\Windows Live",             // Windows Live files
            @"C:\ProgramData\Microsoft\Windows\WER",                                // Windows Error Reporting
            @"C:\Users\<YourUsername>\AppData\Local\Microsoft\Windows\INetCache",   // Internet Cache
       
            // Application temp/cache
            @"C:\Users\<YourUsername>\AppData\Local\Temp",                          // User-specific Temp
            @"C:\Users\<YourUsername>\AppData\Local\Microsoft\Windows\Temporary Internet Files", // Old IE cache
            @"C:\Users\<YourUsername>\AppData\Local\Microsoft\Windows\WebCache",    // Edge/IE WebCache
            @"C:\Users\<YourUsername>\AppData\Local\Microsoft\Windows\Explorer",    // Explorer thumbnail cache
            @"C:\Users\<YourUsername>\AppData\Local\CrashDumps",                    // Application crash dumps
            @"C:\Users\<YourUsername>\AppData\Local\Packages",                      // UWP app caches (subfolders)
            @"C:\Users\<YourUsername>\AppData\Roaming\Microsoft\Windows\Recent",    // Recent files
            @"C:\Users\<YourUsername>\AppData\Roaming\Microsoft\Windows\Cookies",   // Cookies
            @"C:\Users\<YourUsername>\AppData\Local\Google\Chrome\User Data\Default\Cache", // Chrome cache
            @"C:\Users\<YourUsername>\AppData\Local\Mozilla\Firefox\Profiles",      // Firefox profiles (cache inside)
            @"C:\$Recycle.Bin",                                                     // Recycle Bin (system-wide)
            @"C:\Windows\Logs",                                                     // Windows logs
            @"C:\Windows\System32\LogFiles",                                        // System log files
            @"C:\Windows\System32\spool\PRINTERS",                                  // Print spooler files
            @"C:\Windows\Debug",                                                    // Debug logs
            @"C:\Windows\Minidump"                                                  // Crash dumps
        };
        public Form1()
        { 
            InitializeComponent();
            // Enable DWM Composition
            DwmEnableComposition(true);
        }

        public void Scantmp()
        {
            // Get the path to the temp directory
            string tempPath = Path.GetTempPath();

            // Check if the path is not null or empty
            if (!string.IsNullOrEmpty(tempPath))
            {
                try
                {  // Get all files in the temp directory
                    files = Directory.GetFiles(tempPath); 
                    checkedListBox1.Items.Clear();  
                    foreach (string file in files)
                    {
                       checkedListBox1.Items.Add(file);
                    } 

                    foreach(string files in Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch")))
                    {
                        checkedListBox1.Items.Add(files);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Access to the Prefetch directory is denied. Please run the application as administrator to access it.");
                } 
                catch (Exception ex)
                {
                    Console.WriteLine("An Error occurred: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Temp path could not be determined.");
            }

        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the selected files?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Collect items to remove
                List<string> filesToRemove = new List<string>();

                foreach (string fl in checkedListBox1.CheckedItems)
                {
                    try
                    {
                        File.Delete(fl);
                        filesToRemove.Add(fl); // Collect the file for removal
                    }
                    catch (Exception ex)
                    {
                       Console.WriteLine($"Failed to delete: {fl}; Error Message: {ex.Message}", "G-WipeOut Fail to Delete");
                    }
                }

                // Remove files from CheckedListBox after iteration
                foreach (string fl in filesToRemove)
                {
                    checkedListBox1.Items.Remove(fl);
                }

                // Optionally, refresh the list after deletion
                Scantmp();
            }

            //string tmpfile = "";
            //try
            //{
            //    foreach (string fl in files)
            //    {
            //        tmpfile = fl;
            //        File.Delete(fl);
            //    }
            //}
            //catch( Exception ex)
            //{ Console.WriteLine($" fail to delete : {tmpfile}; \nError Message :{ex.Message}" ); }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            Scantmp();
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true); // Select all items
            }
        }
    }
}

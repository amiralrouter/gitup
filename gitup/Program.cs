using System; 
using System.IO; 
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal; 
using System.Windows.Forms;

namespace gitup {
    class App { 
        string getCompanyName() {
            return ((AssemblyCompanyAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0]).Company;
        }

        string getAppName() {
            return ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0]).Title;
        }

        string getAppDirectory() { 
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + this.getCompanyName() + "\\" + this.getAppName();
        }

        public string getCurrentPath() {
            return Assembly.GetExecutingAssembly().Location;
        }
        
        string getMustBePath() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\" + this.getCompanyName() + "\\" + this.getAppName() + "\\" + this.getAppName() + ".exe";
        }

        public bool isInstalled() { 
            return this.getCurrentPath().Equals(this.getMustBePath());
        } 

        public void Install() { 
            Console.WriteLine("Checing if installed...");
            if (!Directory.Exists(this.getAppDirectory())) {
                Console.WriteLine("Creating directory...");
                Directory.CreateDirectory(this.getAppDirectory());
            } 
            
            Console.WriteLine("Copying file...");
            File.Copy(this.getCurrentPath(), this.getMustBePath(), true);

            try {
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

                if (path == null) { 
                    Console.WriteLine("PATH is null");
                    return;
                }
                if (!path.Contains(this.getAppDirectory())) {
                    Console.WriteLine("Adding to PATH...");
                    path += ";" + this.getAppDirectory();
                    Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Machine);
                    Console.WriteLine("PATH updated");
                }
                else{
                    Console.WriteLine("Already in PATH...");
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
 

            Console.WriteLine("Checking if file is executable...");
            FileInfo fi = new FileInfo(this.getMustBePath());
            SecurityIdentifier userAccount = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSecurity fileAcl = new FileSecurity();
            fileAcl.AddAccessRule(new FileSystemAccessRule(userAccount, FileSystemRights.FullControl, AccessControlType.Allow));
            fi.SetAccessControl(fileAcl);
            Console.WriteLine("File is executable");

            Console.WriteLine("Installed successfully");
            Console.WriteLine("Call \"gitup\" command in your directory");

            MessageBox.Show("Installed successfully\nCall \"gitup\" command in your directory", "gitup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
  

    }
    
    internal class Program {
        static void Main(string[] args) {
            App app = new App();    

 
 

            if (!app.isInstalled()) {
                app.Install();
                return;
            }

  
            string commitText = "";
            foreach (string arg in args) 
                commitText += arg + " "; 
            commitText = commitText.Trim();

            // if arguments empty
            if (commitText.Length == 0) {
                commitText = "updated";
            }
  

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C git add * & git commit -m \"" + commitText + "\" & git push";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start(); 
            while (!process.HasExited) {
                Console.WriteLine(process.StandardOutput.ReadToEnd());
            } 



        }
    }
}

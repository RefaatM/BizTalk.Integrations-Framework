using System;
using System.IO;
using System.Reflection;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.Serialization;

namespace GT.BizTalk.SSO.AdminUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintWelcome();

            bool success = (args.Length > 1 && Process(args) == true);
            if (success == false)
            {
                PrintHelp();
            }

            Console.Read();
        }

        private static bool Process(string[] args)
        {
            string action = args[0].ToLower();
            switch (action)
            {
                case "/i": // import
                    return Import(args);

                case "/e": // export
                    // get application name and export file name
                    return Export(args);
            }
            return false;
        }

        private static bool Import(string[] args)
        {
            // validate arguments
            string suppressConfirm = string.Empty;
            string fileName = string.Empty;
            if (args.Length == 2)
            {
                fileName = args[1];
            }
            else if (args.Length == 3)
            {
                suppressConfirm = args[1];
                fileName = args[2];
            }
            else
            {
                Console.WriteLine("ERROR - Invalid number of arguments.");
                Console.WriteLine();
                return false;
            }

            // validate suppress confirm
            if (string.IsNullOrEmpty(suppressConfirm) == false && suppressConfirm.ToLower() != "/y")
            {
                Console.WriteLine("ERROR - Invalid option.");
                Console.WriteLine();
                return false;
            }

            // validate file name
            if (File.Exists(fileName) == false)
            {
                Console.WriteLine("ERROR - The specified SSO application configuration file does not exist.");
                Console.WriteLine();
                return false;
            }

            // import the SSO application configuration file
            try
            {
                // load the app configuration from the file
                SSOAppConfig appConfig = XmlSerializationUtil.LoadXml<SSOAppConfig>(fileName);
                // check if the application already exists
                if (SSOManager.ApplicationExists(appConfig.AppInfo.Name) == true)
                {
                    bool overrideApp = true;

                    // the application exists, ask the user for confirmation to override
                    // if confirmation was not suppressed
                    if (string.IsNullOrEmpty(suppressConfirm) == true)
                    {
                        Console.WriteLine("SSO application already exists. Override (Y/N)?");
                        int key = Console.Read();
                        char ch = Convert.ToChar(key);
                        overrideApp = (ch == 'y' || ch == 'Y');
                    }

                    if (overrideApp == true)
                    {
                        // update/recreate the application
                        SSOManager.UpdateApplication(appConfig, true);
                        Console.WriteLine("SSO application successfully imported.");
                    }
                }
                else
                {
                    // create a new application
                    SSOManager.CreateApplication(appConfig);
                    Console.WriteLine("SSO application successfully imported.");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - An error has occurred importing the SSO application.");
                Console.WriteLine("");
                Console.WriteLine(ex);
                return false;
            }
        }

        private static bool Export(string[] args)
        {
            // validate arguments
            if (args.Length < 3)
            {
                Console.WriteLine("ERROR - Invalid number of arguments.");
                Console.WriteLine();
                return false;
            }

            // get export parameters
            string appName = args[1];
            string fileName = args[2];

            // export the SSO application configuration file
            try
            {
                SSOManager.ExportApplication(appName, fileName);
                Console.WriteLine("SSO application successfully exported.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR - An error has occurred exporting the SSO application.");
                Console.WriteLine("");
                Console.WriteLine(ex);
                return false;
            }
        }

        private static void PrintWelcome()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version ver = assembly.GetName().Version;
            Console.WriteLine("HR - SSO Application Configuration utility - Version {0}", ver);
            Console.WriteLine();
            Console.WriteLine("GT.BizTalk.SSO.AdminUtil.exe -");
            Console.WriteLine("    Utility to import/export SSO configuration store applications.");
            Console.WriteLine();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    GT.BizTalk.SSO.AdminUtil.exe /i [/y] <config>.xml");
            Console.WriteLine("    GT.BizTalk.SSO.AdminUtil.exe /e <name> <config>.xml");
            Console.WriteLine();
            Console.WriteLine("    - OPTIONS -");
            Console.WriteLine("/i");
            Console.WriteLine("    Import the specified SSO application configuration file.");
            Console.WriteLine("    If the application exists, you will be prompted to confirm if you want to");
            Console.WriteLine("    override it unless the /y option is specified.");
            Console.WriteLine("/e");
            Console.WriteLine("    Export the specified SSO application into the specified configuration file.");
            Console.WriteLine("/y");
            Console.WriteLine("    Suppresses prompting to confirm you want to overwrite an existing SSO");
            Console.WriteLine("    application.");
            Console.WriteLine();
            Console.WriteLine("    - ARGUMENTS -");
            Console.WriteLine("<name>         Name of the SSO application.");
            Console.WriteLine("<config>.xml   Name of the SSO application configuration file.");
        }
    }
}

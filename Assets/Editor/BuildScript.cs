using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    [MenuItem("Build/PerformBuild")]
    public static void PerformBuild()
    {
        string[] scenes = { "Assets/Scenes/MainMenu.unity" };
        string buildPath = "Builds/SteamBuild/RaveSurvival.exe";

         BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed.");
        }
    }
}

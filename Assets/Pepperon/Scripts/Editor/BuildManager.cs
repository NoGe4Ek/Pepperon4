using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Pepperon.Scripts.Editor {
public static class BuildManager {
    [MenuItem("Build/Build All")]
    public static void BuildAll() {
        BuildLinuxServer();
        BuildWebClient();
    }

    [MenuItem("Build/Build Client (WebGL)")]
    private static void BuildWebClient() {
        BuildPlayerOptions buildPlayerOptions = new() {
            scenes = new[] { "Assets/Pepperon/Scenes/Start.unity", "Assets/Pepperon/Scenes/Match.unity" },
            locationPathName = "builds/web-client/build",
            target = BuildTarget.WebGL,
            subtarget = (int)StandaloneBuildSubtarget.Player,
        };
        EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", CodeOptimization.BuildTimes.ToString());
        
        Log("Building Server (Linux)...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        Log($"Build result: {summary.result}");
        Log($"Output path: {summary.outputPath}");
        Log($"Total size: {summary.totalSize / (1024f * 1024f):F2} MB");
        Log(summary.result != BuildResult.Succeeded ? "❌ Build failed" : "✅ Build succeeded");
    }

    [MenuItem("Build/Build Server (Linux)")]
    private static void BuildLinuxServer() {
        BuildPlayerOptions buildPlayerOptions = new() {
            scenes = new[] { "Assets/Pepperon/Scenes/Match.unity" },
            locationPathName = "builds/server/build/PepperonServer.x86_64",
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int)StandaloneBuildSubtarget.Server
        };
        // buildPlayerOptions.options = BuildOptions.CompressWithLz4;
        
        Log("Building Server (Linux)...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        Log($"Build result: {summary.result}");
        Log($"Output path: {summary.outputPath}");
        Log($"Total size: {summary.totalSize / (1024f * 1024f):F2} MB");
        Log(summary.result != BuildResult.Succeeded ? "❌ Build failed" : "✅ Build succeeded");
    }

    private static void Log(string message) {
        Console.WriteLine(message);
        Debug.Log(message);
    }
    
    private enum CodeOptimization
    {
        BuildTimes,
        RuntimeSpeed,
        RuntimeSpeedLTO,
        DiskSize,
        DiskSizeLTO,
    }
}
}
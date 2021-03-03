using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

namespace Antilatency {

    public class Build {
        static string[] GetScenes() {
            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
                if (EditorBuildSettings.scenes[i].enabled) {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                }
            }
            return scenes.ToArray();
        }

        static string ReadEnvironment(string variableName, string fallback) {
            return System.Environment.GetEnvironmentVariable(variableName) ?? fallback;
        }

        static string BuildName => ReadEnvironment("BUILD_FILE_NAME", Application.productName);
        public static string ProjectDirectory = Directory.GetCurrentDirectory();
        public static string AndroidReleaseFile = string.Format("{0}/Builds/Android/{1}.apk", ProjectDirectory, BuildName);
        public static string WindowsX64ReleaseFile = string.Format("{0}/Builds/Windows/x64/{1}.exe", ProjectDirectory, BuildName);

        [MenuItem("Antilatency/Build/WindowsX64Release")]
        static void WindowsX64Release() {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive);
            BuildReport report = BuildPipeline.BuildPlayer(GetScenes(), WindowsX64ReleaseFile, BuildTarget.StandaloneWindows64, BuildOptions.None);
            int code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;
            if (Application.isBatchMode) {
                EditorApplication.Exit(code);
            }
        }

        [MenuItem("Antilatency/Build/AndroidRelease")]
        static void AndroidRelease() {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality32Bit;
            BuildReport report = BuildPipeline.BuildPlayer(GetScenes(), AndroidReleaseFile, BuildTarget.Android, BuildOptions.None);
            int code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;

            if (Application.isBatchMode) {
                EditorApplication.Exit(code);
            }
        }
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Build
{
    /// <summary>
    /// Tool for making builds. Has public static methods that can be invoked
    /// via the command-line.
    /// </sumary>
    public static class BuildUtility
    {
        // Config file with a list of scenes to build.
        private static string BuildConfigFileName = "build-config.json";

        [MenuItem("Tools/Make build (Mac)")]
        public static void MakeBuildMacMenu()
        {
            var path = EditorUtility.SaveFilePanel(
                "Save build",
                Application.dataPath,
                "Build",
                string.Empty // Unity automatically adds .app extension
            );
            if (!string.IsNullOrEmpty(path))
            {
                MakeBuild(path, BuildTarget.StandaloneOSX);
            }
        }

        public static void MakeBuildMacCLI()
        {
            var outputPath = GetCommandLineArg("buildPath");
            MakeBuild(outputPath, BuildTarget.StandaloneOSX);
        }

        private static void MakeBuild(string outputPath, BuildTarget target)
        {
            var buildConfig = LoadBuildConfig();

            BuildPipeline.BuildPlayer(
                buildConfig.Scenes,
                outputPath,
                target,
                new BuildOptions()
            );
        }

        /// <summary>
        /// Load and validate the build configuration file.
        /// </summary>
        private static BuildConfig LoadBuildConfig()
        {
            var buildConfigFile = File.ReadAllText(
                Path.Combine(Application.dataPath, BuildConfigFileName)
            );
            var config = JsonUtility.FromJson<BuildConfig>(buildConfigFile);

            if (config.Scenes == null || config.Scenes.Length <= 0)
            {
                throw new Exception("No scenes specified in build config.");
            }

            return config;
        }

        /// <summary>
        /// Get the string value of a named command line argument passed to
        /// the Unity editor.
        /// </summary>
        private static string GetCommandLineArg(string argName)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (Regex.IsMatch(args[i], $"^-{argName}$") && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }
            throw new Exception($"Could not find command-line argument \"{argName}\"");
        }

        /// <sumary>
        /// Class to deserialise build config JSON settings file.
        /// </summary>
        private struct BuildConfig
        {
            public string[] Scenes;
        }
    }
}
//
//  MaxPostProcessBuildiOS.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Thomas So on 2/18/19.
//  Copyright © 2019 AppLovin. All rights reserved.
//

#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
#if UNITY_2017_1_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace AppLovinMax
{
    public static class MaxPostProcessBuildiOS
    {
#if UNITY_2019_3_OR_NEWER
        private const string TargetUnityIphone = "target 'Unity-iPhone' do";
        private const string TargetEnd = "end";

        /// <summary>
        /// This constant is dependent on JarResolver generating the Podfile. Jar Resolver Generates the Podfile at 40 and runs <c>pod install</c> at 50.
        /// I don't think these constants would ever get updated, but if they ever do, might cause our logic to break.
        /// See <c>BUILD_ORDER_GEN_PODFILE</c> constant under IOSResolver.cs: https://github.com/googlesamples/unity-jar-resolver/blob/master/source/IOSResolver/src/IOSResolver.cs#L396
        /// </summary>
        private const int BuildOrderUpdatePodfile = 41;
#endif

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                Debug.Log("[AppLovin MAX] Starting iOS post-process build script...");

                var projectPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
                var project = new PBXProject();
                project.ReadFromString(File.ReadAllText(projectPath));

                //
                // Add the -ObjC linker flag
                //
#if UNITY_2019_3_OR_NEWER
                var target = project.GetUnityMainTargetGuid();
#else
                var target = project.TargetGuidByName("Unity-iPhone");
#endif
                project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

                //
                // Add needed frameworks to Embedded Libraries
                //
                string moPubFrameworkFileGuid = project.FindFileGuidByProjectPath("Frameworks/MaxSdk/Plugins/iOS/MoPub/MoPubSDKFramework.framework");

                // The publisher might be using Unity 2018.3 which adds the frameworks directly into the `Frameworks/` directory.
                if (moPubFrameworkFileGuid == null) moPubFrameworkFileGuid = project.FindFileGuidByProjectPath("Frameworks/MoPubSDKFramework.framework");

                if (moPubFrameworkFileGuid != null)
                {
#if UNITY_2017_1_OR_NEWER
                    Debug.Log("[AppLovin MAX] Adding MoPubSDKFramework.framework to Embedded Binaries list in the Xcode project");
                    project.AddFileToEmbedFrameworks(target, moPubFrameworkFileGuid);
#else
                Debug.Log("[AppLovin Max] Failed to add MoPubSDKFramework.framework to Embedded Binaries. Please add it manually in your Xcode project.");
#endif

                    // Add `@executable_path/Frameworks` to Run Search Paths needed for embedded frameworks on older version of Unity.
                    // NOTE: Unity automatically adds it for newer versions but we don't exactly know the version in which they started doing it, so we will be adding it to all versions.
                    project.SetBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
                }

                //
                // Write modified Xcode project back to original location
                //
                File.WriteAllText(projectPath, project.WriteToString());

                Debug.Log("[AppLovin MAX] Finished iOS post-process build script...");
            }

            // Rename files masked to prevent Unity compilation such as MoPub's mraid.js
            string[] noCompileFiles = Directory.GetFiles(buildPath, "*.no_compile", SearchOption.AllDirectories);
            foreach (var noCompileFile in noCompileFiles)
            {
                Debug.Log("[AppLovin MAX] Removing .no_compile mask from file: " + noCompileFile);
                File.Move(noCompileFile, noCompileFile.Replace(".no_compile", ""));
            }
        }

#if UNITY_2019_3_OR_NEWER
        [PostProcessBuildAttribute(BuildOrderUpdatePodfile)]
        public static void OnPostProcessUpdatePodFile(BuildTarget buildTarget, string buildPath)
        {
            var podfilePath = Path.Combine(buildPath, "Podfile");

            // Check if the Podfile exists. If it doesn't, it could be that the publisher has disabled CocoaPods for Jar Resolver or Jar Resolver has updated the build order constants.
            if (!File.Exists(podfilePath)) return;

            var lines = File.ReadAllLines(podfilePath);

            // Return if target already exists.
            if (lines.Any(line => line.Contains(TargetUnityIphone))) return;

            var updatedPodfile = new List<string>();
            var unityIphoneTargetAdded = false;
            foreach (var line in lines)
            {
                // Add the Unity-iPhone target before the start of other targets. 
                if (!unityIphoneTargetAdded && line.Contains("target"))
                {
                    updatedPodfile.AddRange(new List<string> {TargetUnityIphone, TargetEnd, "", line});
                    unityIphoneTargetAdded = true;
                }
                // Add all other lines.
                else
                {
                    updatedPodfile.Add(line);
                }
            }

            try
            {
                File.WriteAllText(podfilePath, string.Join("\n", updatedPodfile.ToArray()) + "\n");
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to Add Unity-iPhone target to PodFile. Podfile file write failed.");
                Console.WriteLine(exception);
            }
        }
#endif
    }
}

#endif

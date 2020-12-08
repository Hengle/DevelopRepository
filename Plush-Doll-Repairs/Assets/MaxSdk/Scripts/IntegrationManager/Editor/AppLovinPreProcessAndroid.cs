//
//  AppLovinBuildPreProcessor.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 8/27/19.
//  Copyright © 2019 AppLovin. All rights reserved.
//

#if UNITY_ANDROID && !UNITY_2019_3_OR_NEWER

using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

/// <summary>
/// Adds the AppLovin Quality Service plugin to the gradle template file. See <see cref="AppLovinProcessGradleBuildFile"/> for more details.
/// </summary>
public class AppLovinPreProcessAndroid : AppLovinProcessGradleBuildFile,
#if UNITY_2018_1_OR_NEWER
    IPreprocessBuildWithReport
#else
    IPreprocessBuild
#endif
{
#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
        // We can only process gradle template file here. If it is not available, we will try again in post build on Unity IDEs newer than 2018_2 (see AppLovinPostProcessGradleProject).
        if (!AppLovinIntegrationManager.GradleTemplateEnabled) return;

        AddAppLovinQualityServicePlugin(AppLovinIntegrationManager.GradleTemplatePath);
    }

    public int callbackOrder
    {
        get { return int.MaxValue; }
    }
}

#endif

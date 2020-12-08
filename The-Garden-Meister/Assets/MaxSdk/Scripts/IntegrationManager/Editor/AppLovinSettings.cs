//
//  AppLovinSettings.cs
//  AppLovin MAX Unity Plugin
//
//  Created by Santosh Bagadi on 1/27/20.
//  Copyright © 2019 AppLovin. All rights reserved.
//

using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A <see cref="ScriptableObject"/> representing the AppLovin Settings that can be set in the Integration Manager Window.
///
/// The scriptable object asset is created with the name <c>AppLovinSettings.asset</c> and is placed under the directory <c>Assets/MaxSdk/Resources</c>.
/// </summary>
public class AppLovinSettings : ScriptableObject
{
    private static readonly string SettingsFile = Path.Combine("Assets/MaxSdk/Resources", "AppLovinSettings.asset");

    private static AppLovinSettings instance;

    [SerializeField] private string adMobAndroidAppId = string.Empty;
    [SerializeField] private string adMobIosAppId = string.Empty;

    /// <summary>
    /// An instance of AppLovin Setting.
    /// </summary>
    public static AppLovinSettings Instance
    {
        get
        {
            if (instance == null)
            {
                var settingsDir = Path.GetDirectoryName(SettingsFile);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                instance = AssetDatabase.LoadAssetAtPath<AppLovinSettings>(SettingsFile);
                if (instance != null) return instance;

                instance = CreateInstance<AppLovinSettings>();
                AssetDatabase.CreateAsset(instance, SettingsFile);
            }

            return instance;
        }
    }

    /// <summary>
    /// AdMob Android App ID.
    /// </summary>
    public string AdMobAndroidAppId
    {
        get { return Instance.adMobAndroidAppId; }
        set { Instance.adMobAndroidAppId = value; }
    }

    /// <summary>
    /// AdMob iOS App ID
    /// </summary>
    public string AdMobIosAppId
    {
        get { return Instance.adMobIosAppId; }
        set { Instance.adMobIosAppId = value; }
    }

    /// <summary>
    /// Saves the instance of the settings.
    /// </summary>
    public void SaveAsync()
    {
        EditorUtility.SetDirty(instance);
    }
}

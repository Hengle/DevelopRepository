using UnityEngine;
using System.Collections.Generic;

public abstract class MaxSdkBase
{
    /// <summary>
    /// This enum represents whether or not the consent dialog should be shown for this user.
    /// The state where no such determination could be made is represented by {@link ConsentDialogState#Unknown}.
    /// </summary>
    public enum ConsentDialogState
    {
        /// <summary>
        /// The consent dialog state could not be determined. This is likely due to SDK failing to initialize.
        /// </summary>
        Unknown,

        /// <summary>
        /// This user should be shown a consent dialog.
        /// </summary>
        Applies,

        /// <summary>
        /// This user should not be shown a consent dialog.
        /// </summary>
        DoesNotApply
    }

    public enum AdViewPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum BannerPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        Centered,
        CenterLeft,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public class SdkConfiguration
    {
        public ConsentDialogState ConsentDialogState;
    }

    public struct Reward
    {
        public string Label;
        public int Amount;


        public override string ToString()
        {
            return "Reward: " + Amount + " " + Label;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Label) && Amount > 0;
        }
    }

    public class AdInfo
    {    
        public string AdUnitIdentifier { get; }
        public string NetworkName { get; }

        public AdInfo(string adInfoString)
        {
            string adUnitIdentifier = "";
            string networkName = "";
            
            // NOTE: Unity Editor creates empty string
            IDictionary<string, string> adInfoObject = MaxSdkUtils.PropsStringToDict(adInfoString);
            adInfoObject.TryGetValue("adUnitId", out adUnitIdentifier);
            adInfoObject.TryGetValue("networkName", out networkName);
         
            AdUnitIdentifier = adUnitIdentifier;	
            NetworkName = networkName;
        }

        public override string ToString()
        {
            return "[AdInfo adUnitIdentifier: " + AdUnitIdentifier + ", networkName: " + NetworkName + "]";
        }
    }
    
    protected static void ValidateAdUnitIdentifier(string adUnitIdentifier, string debugPurpose)
    {
        if (string.IsNullOrEmpty(adUnitIdentifier))
        {
            Debug.LogError("[AppLovin MAX] No MAX Ads Ad Unit ID specified for: " + debugPurpose);
        }
    }

    // Allocate the MaxSdkCallbacks singleton, which receives all callback events from the native SDKs.
    protected static void InitCallbacks()
    {
        var type = typeof(MaxSdkCallbacks);
        var mgr = new GameObject("MaxSdkCallbacks", type)
            .GetComponent<MaxSdkCallbacks>(); // Its Awake() method sets Instance.
        if (MaxSdkCallbacks.Instance != mgr)
        {
            Debug.LogWarning("[AppLovin MAX] It looks like you have the " + type.Name +
                             " on a GameObject in your scene. Please remove the script from your scene.");
        }
    }

    /// <summary>
    /// Generates serialized Unity meta data to be passed to the SDK.
    /// </summary>
    /// <returns>Serialized Unity meta data.</returns>
    protected static string GenerateMetaData()
    {
        var metaData = new Dictionary<string, string>();
        metaData.Add("UnityVersion", Application.unityVersion);
        
        return MaxSdkUtils.DictToPropsString(metaData);
    }
}
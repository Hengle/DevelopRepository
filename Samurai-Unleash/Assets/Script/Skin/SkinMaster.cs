#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SkinMaster : MonoBehaviour {

    [MenuItem("Assets/Create/SkinMaster")]

    public static void CreateAsset() {
        SkinData item = ScriptableObject.CreateInstance<SkinData>();

        //アセットを保存するパス
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Skin/" + typeof(SkinData) + ".asset");

        AssetDatabase.CreateAsset(item, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = item;
    }
}
#endif
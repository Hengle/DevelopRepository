using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Plugins.Editor
{
    public class PostBuildProcess
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                ProcessForiOS(path);
            }
        }

        private static void ProcessForiOS(string path)
        {
            string pjPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pj = new PBXProject();
            pj.ReadFromString(File.ReadAllText(pjPath));

            string target = pj.TargetGuidByName("Unity-iPhone");

            //libz.tbdの追加
            pj.AddFileToBuild(target, pj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));

            //Other Linker Flags
            pj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            File.WriteAllText(pjPath, pj.WriteToString());
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    public class SetAssetBundleNameContextMenu
    {

        [MenuItem("Assets/SSC/Set AssetBundle Name to File Path", true)]
        private static bool ValidateSetAssetBundleNames()
        {
            return Selection.objects.Length > 0;
        }

        [MenuItem("Assets/SSC/Set AssetBundle Name to File Path")]
        public static void SetAssetBundleNames()
        {

            if (!EditorUtility.DisplayDialog(
                "Set AssetBundle Name to File Path",
                "You cannot undo this action.\n\nContinue ?",
                "Yes",
                "Cancel"
                ))
            {
                return;
            }

            // --------------

            string path = "";

            AssetImporter ai = null;

            foreach (var obj in Selection.objects)
            {

                path = AssetDatabase.GetAssetPath(obj);

                ai = AssetImporter.GetAtPath(path);

                if(ai)
                {

                    path = path.ToLower().Replace(' ', '_');
                    path = Path.ChangeExtension(path, ".unity3d");

                    ai.assetBundleName = path;
                }

            }

            EditorUtility.DisplayDialog("Set AssetBundle Name to File Path", "Done.", "OK");

            AssetDatabase.Refresh();

        }

    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    public class CreateScriptableObjectAsset
    {

        [MenuItem("Assets/SSC/Create ScriptableObject Asset", true)]
        private static bool ValidateCreateScriptableObjectAsset()
        {
            return Selection.objects.Length == 1;
        }

        [MenuItem("Assets/SSC/Create ScriptableObject Asset")]
        public static void createScriptableObjectAsset()
        {

            var selected = Selection.objects[0];

            string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(selected));

            ScriptableObject asset = ScriptableObject.CreateInstance(fileName);

            if (asset)
            {

                string path = EditorUtility.SaveFilePanelInProject("Save", fileName, "asset", "Please enter a file name to save the ScriptableObject to");

                if (!string.IsNullOrEmpty(path))
                {

                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = asset;

                }

            }

        }

    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SSC
{

    public class AssetBundleNameSetter
    {

        [MenuItem("Assets/SSC/Set AssetBundle Names", true)]
        private static bool ValidateSetAssetBundleNames()
        {

            //AssetDatabase.
            //Debug.Log("VALIDATE " + Selection.);
            
            if (Selection.objects.Length <= 0)
            {
                return false;
            }

            string path = "";

            foreach (UnityEngine.Object obj in Selection.objects)
            {

                path = AssetDatabase.GetAssetPath(obj);

                Debug.Log(path);

                if (Directory.Exists(path))
                {
                    return true;
                }

            }

            return false;

        }

        [MenuItem("Assets/SSC/Set AssetBundle Names")]
        public static void SetAssetBundleNames()
        {
            
        }

    }

}

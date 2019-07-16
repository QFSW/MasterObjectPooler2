using System.IO;
using UnityEditor;

namespace QFSW.MOP2.Editor
{
    public class ObjectPoolPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (Path.GetExtension(assetPath) == ".asset")
                {
                    ObjectPool pool = AssetDatabase.LoadAssetAtPath<ObjectPool>(assetPath);
                    if (pool)
                    {
                        pool.AutoFillName();
                    }
                }
            }
        }
    }
}

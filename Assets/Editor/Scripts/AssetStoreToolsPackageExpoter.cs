using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public static class AssetStoreToolsPackageExpoter
{
    private static string PackageName = "AssetStoreToolsPackage";

    [MenuItem("Tools/ExportAssetStoreTools")]
    private static void ExportAST()
    {
        string path = "Assets/AssetStoreTools"; //アセットフォルダのパス

        //フォルダ内のアセットを列挙
        var assetGUIDs = AssetDatabase.FindAssets(string.Empty, new string[] { path });
        var assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

        //例外とログの処理
        if (assetPaths.Count() <= 0)
        {
            Debug.LogWarning("エキスポートするアセットがありません。");
            return;
        }
        else
        {
            Debug.LogWarning($"以下のファイルをエキスポートしました。\n{string.Join('\n', assetPaths)}");
        }

        //プロジェクトディレクトリ直下のパスを取得
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string exportPath = Path.Combine(projectPath, $"{PackageName}.unitypackage");

        // パッケージをエクスポート
        AssetDatabase.ExportPackage(assetPaths, exportPath, ExportPackageOptions.Recurse);
    }
}

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
        string path = "Assets/AssetStoreTools"; //�A�Z�b�g�t�H���_�̃p�X

        //�t�H���_���̃A�Z�b�g���
        var assetGUIDs = AssetDatabase.FindAssets(string.Empty, new string[] { path });
        var assetPaths = assetGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToArray();

        //��O�ƃ��O�̏���
        if (assetPaths.Count() <= 0)
        {
            Debug.LogWarning("�G�L�X�|�[�g����A�Z�b�g������܂���B");
            return;
        }
        else
        {
            Debug.LogWarning($"�ȉ��̃t�@�C�����G�L�X�|�[�g���܂����B\n{string.Join('\n', assetPaths)}");
        }

        //�v���W�F�N�g�f�B���N�g�������̃p�X���擾
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string exportPath = Path.Combine(projectPath, $"{PackageName}.unitypackage");

        // �p�b�P�[�W���G�N�X�|�[�g
        AssetDatabase.ExportPackage(assetPaths, exportPath, ExportPackageOptions.Recurse);
    }
}

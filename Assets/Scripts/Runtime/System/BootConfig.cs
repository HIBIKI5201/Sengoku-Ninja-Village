using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    /// 初期化のコンフィグ
    /// </summary>
    [CreateAssetMenu(menuName = "SengokuNinjaVillage/BootConfig")]
    public class BootConfig : ScriptableObject
    {
        [SerializeField, Tooltip("初期化シーンを設定する")]
        private SceneListEnum _sceneKind = SceneListEnum.OutGame;
        public SceneListEnum InitializeSceneKind => _sceneKind;
        
        [SerializeField, Tooltip("初期化シーンの有効化")]
        private bool _initializeScene = true;
        public bool InitializeScene => _initializeScene;
    }
}

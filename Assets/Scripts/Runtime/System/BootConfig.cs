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
        private SceneKind _sceneKind = SceneKind.OutGame;
        public SceneKind InitializeSceneKind => _sceneKind;

        public enum SceneKind
        {
            InGame = SceneListEnum.InGame,
            OutGame = SceneListEnum.OutGame,
        }
    }
}

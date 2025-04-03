using SymphonyFrameWork.System;
using UnityEngine;

namespace SengokuNinjaVillage.System
{
    /// <summary>
    /// ゲームの初期化を行う
    /// </summary>
    public static class BootStrap
    {
        /// <summary>
        /// シーンロード前の初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            //システムシーンをロードする
            var task = SceneLoader.LoadScene(SceneListEnum.System.ToString());
        }
    }
}

using SymphonyFrameWork.System;
using System.Linq;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
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

            if (SceneLoader.GetExistScene(SceneListEnum.System.ToString(), out var scene))
            {
                var manager = scene.GetRootGameObjects()
                    .FirstOrDefault(go => go.GetComponent<SceneManager>());
            }

        }
    }
}

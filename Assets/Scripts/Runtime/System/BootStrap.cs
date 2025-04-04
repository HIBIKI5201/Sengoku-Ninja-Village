using SymphonyFrameWork.System;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    ///     ゲームの初期化を行う
    /// </summary>
    public static class BootStrap
    {
        /// <summary>
        ///     シーンロード前の初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Initialize()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneListEnum.System.ToString());

            //システムシーンのマネージャーを初期化
            if (SceneLoader.GetExistScene(SceneListEnum.System.ToString(), out var scene))
            {
                //マネージャーを取得
                SceneManager manager = null;
                foreach (var go in scene.GetRootGameObjects())
                    if (go.TryGetComponent(out SceneManager sm))
                    {
                        manager = sm;
                        break;
                    }

                if (manager)
                {
                    await manager.SceneAwake();
                }
                else
                {
                    Debug.LogWarning("System Scene not found");
                }
            }
        }
    }
}
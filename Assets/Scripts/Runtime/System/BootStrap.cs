using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void Initialize()
        {
            //システムシーンをロードし、それ以外はアンロード
            await SceneLoader.LoadScene(SceneListEnum.System.ToString(), mode: LoadSceneMode.Single);

            if (SceneLoader.GetExistScene(SceneListEnum.System.ToString(), out var scene))
            {
                //システムシーンのロードが終わるまで待機
                while (!scene.IsValid() || !scene.isLoaded)
                {
                    await Awaitable.NextFrameAsync();
                }

                //マネージャーを取得
                SceneDirector director = null;
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.TryGetComponent(out SceneDirector sd))
                    {
                        director = sd;
                        break;
                    }
                }

                //システムシーンを初期化
                if (director)
                {
                    _ = director.SceneAwake();
                }
                else
                {
                    Debug.LogWarning("System Scene not found");
                }
            }
        }
        
    }
}
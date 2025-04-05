using SymphonyFrameWork.System;
using System.Threading.Tasks;
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
            await SceneLoader.LoadScene(SceneListEnum.System.ToString());

            //システムシーンのマネージャーを初期化
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
using SymphonyFrameWork.System;
using System.Threading.Tasks;
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
            await SceneManager.LoadSceneAsync(SceneListEnum.System.ToString(), LoadSceneMode.Single);
            Scene scene = SceneManager.GetSceneByName(SceneListEnum.System.ToString());
            
            //システムシーンのマネージャーを初期化
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
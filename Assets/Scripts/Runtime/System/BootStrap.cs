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
        public const string ConfigPath = "SystemAsset/Boot Config";
        
        /// <summary>
        ///     シーンロード前の初期化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void Initialize()
        {
            //コンフィグをロードして初期化が必要か確認する
            var request = Resources.LoadAsync<BootConfig>(ConfigPath);
            await request;
            var config = request.asset as BootConfig;
            if (!config || !config.InitializeScene)
            {
                Debug.LogWarning("BootStrapの初期化シーンロードを実行しません");
                return;
            }
            
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
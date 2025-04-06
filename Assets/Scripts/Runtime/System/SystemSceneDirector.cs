using System;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    ///     システムシーンのマネジメントを行う
    /// </summary>
    public class SystemSceneDirector : SceneDirector
    {
        
        public override SceneListEnum[] RequiredScenes => Array.Empty<SceneListEnum>();
        
        [SerializeField] private string _configPath = "SystemAsset/Boot Config";

        public override async Task SceneAwake()
        {
            //コンフィグをロード
            var request = Resources.LoadAsync<BootConfig>(_configPath);
            await request;
            var config = request.asset as BootConfig;

            #region 設定されたシーンをロード

            if (!config)
            {
                Debug.LogWarning("Boot Configが見つかりません");
                return;
            }

            if (config.InitializeSceneKind == SceneListEnum.None)
            {
                Debug.LogWarning("InitializeSceneKindが設定されていません");
                return;
            }
            
            await ChangeScene(config.InitializeSceneKind);

            #endregion
        }

        /// <summary>
        ///     シーンをロードする
        /// </summary>
        /// <param name="sceneKind"></param>
        public async Task ChangeScene(SceneListEnum sceneKind)
        {
            //メインのシーンをロード
            await SceneLoader.LoadScene(sceneKind.ToString());
            SceneLoader.SetActiveScene(sceneKind.ToString());

            //ロードしたシーンからディレクターを取得
            SceneDirector director = null;
            if (SceneLoader.GetExistScene(sceneKind.ToString(), out var scene))
            {
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.TryGetComponent(out SceneDirector sceneDirector))
                    {
                        director = sceneDirector;
                        break;
                    }
                }
            }
            
            if (!director)
            {
                Debug.LogWarning("ロードされたシーンにはディレクターがありません");
                return;
            }

            if (director.RequiredScenes.Length <= 0)
            {
                return;
            }
                
            //必要なシーンをロード
            Task[] loadTasks = new Task[director.RequiredScenes.Length];
            for (int i = 0; i < director.RequiredScenes.Length; i++)
            {
                loadTasks[i] = SceneLoader.LoadScene(director.RequiredScenes[i].ToString());
            }
            await Task.WhenAll(loadTasks);
        }
    }
}
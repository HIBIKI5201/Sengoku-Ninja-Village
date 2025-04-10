using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SymphonyFrameWork.Debugger;
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
        
        public override async Task SceneAwake()
        {
            //locator登録
            ServiceLocator.SetInstance(this, ServiceLocator.LocateType.Singleton);
            
            //コンフィグをロード
            var request = Resources.LoadAsync<BootConfig>(BootStrap.ConfigPath);
            await request;
            var config = request.asset as BootConfig;
            
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
        }

        /// <summary>
        ///     シーンをロードする
        /// </summary>
        /// <param name="sceneKind"></param>
        public async Task ChangeScene(SceneListEnum sceneKind)
        {
            SymphonyDebugLog.AddText("以下のシーンをロード。");
            
            //メインのシーンをロード
            await LoadScene(sceneKind);
            if (SceneLoader.SetActiveScene(sceneKind.ToString()))
            {
                SymphonyDebugLog.AddText($"{sceneKind.ToString()}のロードが完了");
                SymphonyDebugLog.TextLog();
            }
            else
            {
                SymphonyDebugLog.AddText($"{sceneKind.ToString()}のロードに失敗");
                SymphonyDebugLog.TextLog(SymphonyDebugLog.LogKind.Warning);
            }
        }

        private static async Task LoadScene(SceneListEnum sceneKind)
        {
            await SceneLoader.LoadScene(sceneKind.ToString());
            SymphonyDebugLog.AddText(sceneKind.ToString());
            
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
                Debug.Log(director is null);
                return;
            }

            await director.SceneAwake();
            
            //必要なシーンをロード
            for (int i = 0; i < director.RequiredScenes.Length; i++)
            {
                var requiredScene = director.RequiredScenes[i];

                //対象シーンが未ロードだった場合はロードを開始する
                if (!SceneLoader.GetExistScene(requiredScene.ToString(), out _))
                {
                    await LoadScene(requiredScene);
                }
            }
        }
    }
}
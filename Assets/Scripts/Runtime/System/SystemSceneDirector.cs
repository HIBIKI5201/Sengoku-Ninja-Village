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

            if (config.InitializeSceneKind != SceneListEnum.None)
            {
                Debug.LogWarning("InitializeSceneKindが設定されていません");
                return;
            }

            await SceneLoader.LoadScene(config.InitializeSceneKind.ToString());
            SceneLoader.SetActiveScene(config.InitializeSceneKind.ToString());

            #endregion
        }
    }
}
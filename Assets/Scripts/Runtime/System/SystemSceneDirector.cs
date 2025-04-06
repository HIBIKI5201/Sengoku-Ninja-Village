using System;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.Serialization;

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

            //設定されたシーンをロード
            if (config)
            {
                await SceneLoader.LoadScene(config.InitializeSceneKind.ToString());
            }
        }
    }
}
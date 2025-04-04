using PlasticGui.WorkspaceWindow.PendingChanges;
using SymphonyFrameWork.System;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    /// システムシーンのマネジメントを行う
    /// </summary>
    public class SystemManager : SceneManager
    {
        private bool _isInitialize = false;

        public override async Task SceneAwake()
        {

            //初期化を実行
            try
            {
                await Awaitable.NextFrameAsync(destroyCancellationToken);

                await SceneStart();

                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log($"システムの初期化をキャンセル\n{ex}");
            }

            //初期化終了を記録
            _isInitialize = true;
        }

        private void Update()
        {
            if (!_isInitialize) return;
        }
    }
}

using PlasticGui.WorkspaceWindow.PendingChanges;
using SymphonyFrameWork.System;
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
        private ManagedComponent[] _components = default;

        private bool _isInitialize = false;

        public override async Task SceneAwake()
        {
            //システムシーンのManagedComponentを取得する
            if (SceneLoader.GetExistScene(SceneListEnum.System.ToString(), out var scene))
            {
                _components = scene.GetRootGameObjects()
                    .Select(go => go.GetComponent<ManagedComponent>())
                    .Where(co => co)
                    .ToArray();
            }

            //Awakeを実行する
            foreach (var c in _components)
            {
                c?.ManagedAwake();
            }

            await Awaitable.NextFrameAsync(destroyCancellationToken);

            await SceneStart();

            await Awaitable.NextFrameAsync(destroyCancellationToken);

            _isInitialize = true;
        }

        public override async Task SceneStart()
        {
            //Startを実行する
            foreach (var c in _components)
            {
                c?.ManagedStart();
            }
        }

        private void Update()
        {
            if (!_isInitialize) return;

            //Updateを実行する
            foreach (var c in _components)
            {
                c?.ManagedUpdate();
            }
        }
    }
}

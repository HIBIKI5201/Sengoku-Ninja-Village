using SymphonyFrameWork.System;
using System.Linq;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    /// システムシーンのマネジメントを行う
    /// </summary>
    public class SystemManager : SceneManager
    {
        private ManagedComponent[] _components = default;

        public override void SceneAwake()
        {
            //システムシーンのManagedComponentを取得する
            if (SceneLoader.GetExistScene(SceneListEnum.System.ToString(), out var scene))
            {
                _components = scene.GetRootGameObjects().Select(go => go.GetComponent<ManagedComponent>()).ToArray();
            }

            //Awakeを実行する
            foreach (var c in _components)
            {
                c.ManagedAwake();
            }
        }

        public override void SceneStart()
        {
            //Startを実行する
            foreach (var c in _components)
            {
                c.ManagedStart();
            }

            _components = null; //解放
        }
    }
}

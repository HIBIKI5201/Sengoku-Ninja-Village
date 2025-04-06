using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime
{
    public abstract class SceneDirector : MonoBehaviour
    {
        /// <summary>
        ///     シーンの初期化
        /// </summary>
        /// <returns></returns>
        public virtual Task SceneAwake() => Task.CompletedTask;
        
        public abstract SceneListEnum[] RequiredScenes { get; }
    }
}
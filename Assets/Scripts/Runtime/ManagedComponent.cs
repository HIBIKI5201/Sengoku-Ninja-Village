using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    /// シーンのシステムにマネジメントされるコンポーネント
    /// ルートゲームオブジェクトのみ実行される
    /// </summary>
    public class ManagedComponent : MonoBehaviour
    {
        /// <summary>
        /// シーンシステムによるAwake
        /// </summary>
        public virtual Task ManagedAwake() { return Task.CompletedTask; }

        /// <summary>
        /// シーンシステムによるUpdate
        /// </summary>
        public virtual Task ManagedStart() { return Task.CompletedTask; }

        /// <summary>
        /// シーンシステムによるUpdate
        /// </summary>
        public virtual void ManagedUpdate() { }
    }
}

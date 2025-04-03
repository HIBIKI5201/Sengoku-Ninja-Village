using UnityEngine;

namespace SengokuNinjaVillage.Runtime.System
{
    /// <summary>
    /// シーンのシステムにマネジメントされるコンポーネント
    /// </summary>
    public class ManagedComponent : MonoBehaviour
    {
        /// <summary>
        /// シーンシステムによるAwake
        /// </summary>
        public virtual void ManagedAwake() { }

        /// <summary>
        /// シーンシステムによるUpdate
        /// </summary>
        public virtual void ManagedStart() { }

        /// <summary>
        /// シーンシステムによるUpdate
        /// </summary>
        public virtual void ManagedUpdate() { }
    }
}

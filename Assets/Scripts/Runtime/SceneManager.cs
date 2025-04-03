using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime
{
    public abstract class SceneManager : MonoBehaviour
    {
        public abstract Task SceneAwake();

        public abstract Task SceneStart();

        public virtual void SceneUpdate() { }

    }
}

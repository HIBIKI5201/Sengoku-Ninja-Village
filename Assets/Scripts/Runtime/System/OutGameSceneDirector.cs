using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SengokuNinjaVillage.Runtime.System
{
    public class OutGameSceneDirector : SceneDirector
    {
        [SerializeField]private UnityEvent _initialized;
        
        public override SceneListEnum[] RequiredScenes =>Array.Empty<SceneListEnum>(); 
        public override Task SceneAwake()
        {
            _initialized?.Invoke();
            return Task.CompletedTask;
        }
    }
}

using SengokuNinjaVillage.Runtime.System;
using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage
{
    /// <summary>
    /// コンポーネントが実行されているか確認する
    /// </summary>
    public class DebugManagedComponent : ManagedComponent
    {
        public override Task ManagedAwake()
        {
            Debug.Log("ManagedAwake");
            return Task.CompletedTask;
        }
        public override Task ManagedStart()
        {
            Debug.Log("ManagedStart");
            return Task.CompletedTask;
        }

        public override void ManagedUpdate()
        {
            Debug.Log("ManagedUpdate");
        }
    }
}

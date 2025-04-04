﻿using System.Threading.Tasks;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime
{
    public abstract class SceneManager : MonoBehaviour
    {
        public virtual Task SceneAwake() { return Task.CompletedTask; }

        public virtual Task SceneStart() { return Task.CompletedTask; }

        public virtual void SceneUpdate() { }

    }
}

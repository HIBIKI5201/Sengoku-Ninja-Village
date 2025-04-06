using System;
using UnityEngine;

namespace SengokuNinjaVillage.Runtime.OutGame
{
    public class IngameSceneDirector : SceneDirector
    {
        public override SceneListEnum[] RequiredScenes => new [] { SceneListEnum.MapScene_1 };
    }
}

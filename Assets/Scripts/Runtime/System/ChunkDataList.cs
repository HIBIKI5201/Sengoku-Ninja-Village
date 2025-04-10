using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SengokuNinjaVillage.Runtime.System
{
    [CreateAssetMenu(fileName = "ChunkDataList", menuName = "GameData/ChunkDataList")]
    public class ChunkDataList : ScriptableObject
    {
        [SerializeField] private ChunkData[] _chunks;
        public ChunkData[] Chunks => _chunks;
    }

    [Serializable]
    public class ChunkData
    {
        [SerializeField] private Vector2 _chunkPos;
        [SerializeField] private SceneListEnum _sceneKind;
        
        public Vector2 ChunkPos => _chunkPos;
        public SceneListEnum SceneKind => _sceneKind;
    }
}

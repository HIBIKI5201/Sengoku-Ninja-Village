using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SengokuNinjaVillage.Runtime.System
{
    public class InGameSceneDirector : SceneDirector
    {
        //TODO:HERE TimeLineに変更
        [FormerlySerializedAs("_loadAnimation")] [Header("ロードアニメーション")] [SerializeField]
        private Animator _loadAnimator;

        [SerializeField] private string _loadAnimName;
        [SerializeField] private Animator _unLodeAnimation;

        /// <summary>チャンクのsize</summary>
        [Header("チャンク設定")] [SerializeField] private Vector2 _chunkSize = new(100, 100);

        /// <summary>loadするチャンクの距離(個数単位)</summary>
        [SerializeField] private Vector2 _loadChunkDistance = new(1, 1);

        [SerializeField] private ChunkDataList _chunkDataList;

        [Header("初期化"), SerializeField] private UnityEvent _initialized;


        private static Dictionary<Vector2, ChunkDirector> _chunks;


        /// <summary> テスト用　</summary>
        [SerializeField] private Vector3 _playerPos;

        public override SceneListEnum[] RequiredScenes => Array.Empty<SceneListEnum>();


        public override async Task SceneAwake()
        {
            Debug.Log("Loading Ingame Scene...");
            if (_chunks == null)
            {
                //Chunkのデータ読み込み
                LoadChunkData();
            }

            //playerの座標をチャンク上の座標に変更
            var centerChunk = new Vector2(_playerPos.x / _chunkSize.x, _playerPos.z / _chunkSize.y);
            
            await LoadChunk(centerChunk);
            //アニメーションなどを待機させる
            await WaitFor();
            //初期化処理
            _initialized?.Invoke();
            return;
            
            async Task WaitFor()
            {
                if (_loadAnimator == null) return;
                //Animator内に指定の名前のアニメーションがあるかどうかの確認
                var isAnimName =
                    _loadAnimator.runtimeAnimatorController.animationClips.Any(x => x.name == _loadAnimName);
                if (!isAnimName) return;


                // アニメーションを再生
                _loadAnimator.Play(_loadAnimName);

                //AnimatorStateInfo stateInfo = _loadAnimation.GetCurrentAnimatorStateInfo(0);

                // アニメーションが始まるのを待つ
                while (!_loadAnimator.GetCurrentAnimatorStateInfo(0).IsName(_loadAnimName))
                {
                    await Awaitable.EndOfFrameAsync();
                }

                // アニメーションが終わるまで待機
                while (_loadAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    await Awaitable.EndOfFrameAsync();
                }

                return;
            }
        }

        public void LoadChunkData()
        {
            _chunks = new();

            foreach (var chunk in _chunkDataList.Chunks)
            {
                _chunks.Add(chunk.ChunkPos, new ChunkDirector(chunk.SceneName, chunk.SceneKind));
            }
        }

        /// <summary>
        /// チャンクロード
        /// </summary>
        /// <param name="centerPos">現在のチャンク位置</param>
        private async Task LoadChunk(Vector2 centerPos)
        {
            //現在位置との距離が一定を超えているかつ、ロード済みのシーンをアンロード
            var unLode = _chunks.Where(chunk =>chunk.Value.IsLoaded &&
                       (chunk.Key.x > centerPos.x + _loadChunkDistance.x ||
                        chunk.Key.x < centerPos.x - _loadChunkDistance.x ||
                       chunk.Key.y > centerPos.y + _loadChunkDistance.y ||
                        chunk.Key.y < centerPos.y - _loadChunkDistance.y)
            );

            foreach (var chunk in unLode)
            {
                //Debug.Log($"{chunk.Value.Kind.ToString()} is unloaded");
                await chunk.Value.UnLoadChunk();
            }

            
            //現在地点の距離に応じて未ロードのシーンをロードする。
            for (var i = (int)centerPos.x - (int)_loadChunkDistance.x; i <= _loadChunkDistance.x + centerPos.x; i++)
            {
                for (var j = (int)centerPos.y - (int)_loadChunkDistance.y; j <= _loadChunkDistance.y + centerPos.y; j++)
                {
                    //Debug.Log(_chunks.TryGetValue(new Vector2(i,j), out var chunkt));
                    if (_chunks.TryGetValue(new Vector2(i, j), out var chunk) && !chunk.IsLoaded)
                    {
                        await chunk.LoadChunk();
                    }
                }
            }
        }

        /// <summary>
        /// デバッグ用のLoadメソッド
        /// </summary>
        [ContextMenu("Debug Load Chunk")]
        private void DebugLoadChunk()
        {
            Debug.Log("Loading Ingame Scene...");
            _ = LoadChunk(new Vector2(_playerPos.x / _chunkSize.x, _playerPos.z / _chunkSize.y));
        }

        private void OnDrawGizmos() //TODO:HERE  
        {
        }
    }

    public class ChunkDirector
    {
        private Scene _scene;
        private string _sceneName;
        private SceneListEnum _sceneKind;
        private bool _isLoaded;

        public Scene Scene => _scene;

        public bool IsLoaded
        {
            get => _isLoaded;
            set => _isLoaded = value;
        }

        public string SceneName => _sceneName;
        public SceneListEnum Kind => _sceneKind;

        public ChunkDirector()
        {
            return;
        }

        public ChunkDirector(string sceneName = null, SceneListEnum sceneKind = SceneListEnum.None)
        {
            _sceneName = sceneName;
            _sceneKind = sceneKind;
        }

        public async Task LoadChunk()
        {
            await SceneLoader.LoadScene(_sceneKind.ToString());
            _isLoaded = true;
        }

        public async Task UnLoadChunk()
        {
            await SceneLoader.UnloadScene(_sceneKind.ToString());
            _isLoaded = false;
        }
    }
}
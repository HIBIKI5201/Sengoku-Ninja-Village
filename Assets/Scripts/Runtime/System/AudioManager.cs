using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using System.Threading.Tasks;

namespace SengokuNinjaVillage.Runtime.System
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer _mixer;

        /// <summary>
        /// オーディオミキサーのグループ
        /// </summary>
        public enum AudioType
        {
            Master,
            BGM,
            SE,
            Voice
        }

        private Dictionary<AudioType, (AudioMixerGroup group, AudioSource source, float originalVolume)> _audioDict = new();

        [Serializable]
        private class AudioData
        {
            public float Volume = 1;
            public AudioClip Clip = default;
        }

        [SerializeField]
        private List<AudioData> _bgmList = new();
        private CancellationTokenSource _bgmChangeToken;

        [SerializeField]
        private List<AudioData> _soundEffectList = new();

        private void Awake()
        {
            AudioSourceInit();

            //BGMの音素材をループ化
            if (_audioDict.TryGetValue(AudioType.BGM, out var data))
            {
                data.source.loop = true;
            }
        }

        /// <summary>
        /// AudioSourceの初期化
        /// </summary>
        private void AudioSourceInit()
        {
            //Enumの名前を取得
            if (!_mixer)
            {
                Debug.LogWarning("オーディオミキサーがアサインされていません");
                return;
            }

            foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
            {
                string name = type.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                AudioMixerGroup group = _mixer.FindMatchingGroups(name).FirstOrDefault();

                if (group)
                {
                    //AudioTypeの数だけAudioSourceを作成
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = group;
                    source.playOnAwake = false;

                    //初期の音量を保存
                    if (_mixer.GetFloat($"{name}_Volume", out float value))
                    {
                        _audioDict.Add(type, (group, source, value));
                    }
                    else
                    {
                        Debug.LogWarning($"AudioMixerに{type}の音量が設定されていません");
                    }
                }
                else
                {
                    Debug.LogWarning($"AudioMixerに{type}のグループが設定されていません");
                }
            }
        }
        /// <summary>
        /// BGMの音を変更する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void VolumeSliderChanged(AudioType type, float value)
        {
            if (value < 0 || 1 < value)
            {
                Debug.LogWarning("音量の値が不正です");
                return;
            }

            //デジベルに変換し音量変更
            float db = value * (_audioDict[type].originalVolume + 80) - 80;

            _mixer.SetFloat(type.ToString(), db);
        }

        /// <summary>
        /// ミキサーグループを出す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AudioMixerGroup GetMixerGroup(AudioType type) => _audioDict[type].group;

        public async Task BGMFadeOut(float duration, CancellationToken token = default)
        {
            AudioSource source = _audioDict[AudioType.BGM].source;

            while (source.volume > 0)
            {
                source.volume -= 1 / (duration / 2) * Time.deltaTime;
                await Awaitable.NextFrameAsync(token);
            }

            source.Stop();
        }

        public async Task BGMFadeIn(float duration, CancellationToken token = default)
        {
            AudioSource source = _audioDict[AudioType.BGM].source;

            source.Play();

            while (source.volume < _audioDict[AudioType.BGM].originalVolume)
            {
                source.volume += 1 / (duration / 2) * Time.deltaTime;
                await Awaitable.NextFrameAsync(token);
            }
        }

        /// <summary>
        /// BGMを変更する
        /// </summary>
        public async void BGMChanged(int index, float duration)
        {
            if (_bgmList.Count <= index)
            {
                Debug.LogWarning("BGMのインデックスが不正です");
                return;
            }

            //前のBGMを止める
            if (_bgmChangeToken is { IsCancellationRequested: false })
            {
                _bgmChangeToken.Cancel();
            }

            //新しいトークンを作成
            _bgmChangeToken = new();
            var token = _bgmChangeToken.Token;

            AudioSource source = _audioDict[AudioType.BGM].source;
            var data = _bgmList[index];

            //BGMをフェードアウト
            try
            {
                await BGMFadeOut(duration, token);
            }
            finally
            {
                source.volume = 0;

                //BGMを変更
                source.clip = data.Clip;
            }

            //BGMをフェードイン
            try
            {
                await BGMFadeIn(duration, token);
            }
            finally
            {
                source.volume = data.Volume;
            }
        }

        public void PlaySoundEffect(int index)
        {
            if (_soundEffectList.Count <= index)
            {
                Debug.LogWarning("SEのインデックスが不正です");
                return;
            }
            AudioSource source = _audioDict[AudioType.SE].source;
            var data = _soundEffectList[index];

            source.volume = data.Volume;
            source.PlayOneShot(data.Clip, data.Volume);
        }
    }
}

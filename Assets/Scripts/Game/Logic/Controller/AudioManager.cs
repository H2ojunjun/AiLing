using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AiLing
{
	/// <summary>
	/// 声音管理器
	/// </summary>
	public class AudioManager : MonoSingleton<AudioManager>
	{
		Dictionary<AudioClip, AudioSource> _sourceDic = new Dictionary<AudioClip, AudioSource>();

		[LabelText("默认背景音乐")]
		public AudioClip defaultClip;
		//用于播放背景音乐的音源组件
		AudioSource _bkgMusicAudioSource = null;
		//音源资源池，专用于播放音效。
		AudioSource[] _soundAudioSourceArr;

		void Awake()
		{
			_bkgMusicAudioSource = gameObject.GetComponent<AudioSource>();
			//将背景音乐的音源组件设置为循环播放模式。
			_bkgMusicAudioSource.loop = true;

			//初始化音效数组
			_soundAudioSourceArr = new AudioSource[5];
			for (int i = 0; i < _soundAudioSourceArr.Length; i++)
			{
				GameObject go = new GameObject(i.ToString());
				go.transform.parent = transform;
				_soundAudioSourceArr[i] = go.AddComponent<AudioSource>();
			}
			if(defaultClip != null)
				PlayAudio(_bkgMusicAudioSource, defaultClip);
		}

		/// <summary>
		/// 播放指定音乐
		/// </summary>
		private void PlayAudio(AudioSource audioSource, AudioClip music)
		{
			audioSource.clip = music;
			audioSource.Play();
            if (!_sourceDic.ContainsKey(music))
            {
				_sourceDic.Add(music, audioSource);
            }
            else
            {
				_sourceDic[music] = audioSource;
            }
		}

        /// <summary>
        /// 播放枚举指定的音效
        /// </summary>
        /// <param name="music">Audio enum.</param>
        public void PlaySound(AudioClip music)
        {
            AudioSource audioSource = GetFreeAudioSource(_soundAudioSourceArr);
			PlayAudio(audioSource, music);
		}

        AudioSource GetFreeAudioSource(AudioSource[] pool)
		{
			for (int i = 0; i < pool.Length; i++)
			{
				if (pool[i].isPlaying == false)
					return pool[i];
			}
			DebugHelper.LogError("音源数组大小不足，导致需要提前终止音效的播放，请增加数组容量");
			pool[0].Stop();
			return pool[0];
		}

		public void StopAllAudioSourceInPool(AudioSource[] pool)
		{
			for (int i = 0; i < pool.Length; i++)
			{
				_soundAudioSourceArr[i].Stop();
			}
		}

		public void StopBkgMusic()
        {
			_bkgMusicAudioSource.Stop();
        }

		public void SetBkgMusic(AudioClip music)
        {
			PlayAudio(_bkgMusicAudioSource, music);
		}

		public void StopSpecificAudio(AudioClip music)
        {
			AudioSource asource = null;
			if (_sourceDic.TryGetValue(music, out asource))
			{
				asource.Stop();
			}
			else
				DebugHelper.Log("找不到正在播放 " + music.ToString() + " 的音源组件");
		}
	}
}



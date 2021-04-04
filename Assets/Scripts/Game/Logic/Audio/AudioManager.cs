using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
	/// <summary>
	/// 声音管理器
	/// </summary>
	public class AudioManager : MonoSingleton<AudioManager>
	{
		Dictionary<EMusicName, AudioSource> _audioSourceDic = new Dictionary<EMusicName, AudioSource>();
		const string AUDIO_PATH = "Audios/";
		//用于播放背景音乐的音源组件
		AudioSource _musicAudioSource = null;
		//音源资源池，专用于播放音效。
		AudioSource[] _soundAudioSourceArr;

		void Awake()
		{
			_musicAudioSource = gameObject.GetComponent<AudioSource>();
			//将背景音乐的音源组件设置为循环播放模式。
			_musicAudioSource.loop = true;

			//初始化音效数组
			_soundAudioSourceArr = new AudioSource[5];
			for (int i = 0; i < _soundAudioSourceArr.Length; i++)
			{
				GameObject go = new GameObject(i.ToString());
				go.transform.parent = transform;
				_soundAudioSourceArr[i] = go.AddComponent<AudioSource>();
			}
			PlayAudio(_musicAudioSource, EMusicName.BackGround);
		}

		/// <summary>
		/// 播放指定背景音乐
		/// </summary>
		private void PlayAudio(AudioSource audioSource, EMusicName music)
		{
			audioSource.clip = GetAudio(music);
			audioSource.Play();
			AudioSource asource = null;
			if (!_audioSourceDic.TryGetValue(music, out asource))
			{
				_audioSourceDic.Add(music, audioSource);
			}
			else
				_audioSourceDic[music] = audioSource;
		}

		/// <summary>
		/// 根据提供的声音枚举，加载对应的声音文件
		/// </summary>
		/// <returns>The aduio.</returns>
		/// <param name="audioEnum">Audio enum.</param>
		AudioClip GetAudio(EMusicName audioEnum)
		{
			AudioClip clip = Resources.Load(AUDIO_PATH + audioEnum.ToString()) as AudioClip;
			return clip;
		}

        /// <summary>
        /// 播放枚举指定的音效
        /// </summary>
        /// <param name="audioEnum">Audio enum.</param>
        public void PlaySound(EMusicName audioEnum)
        {
            AudioSource audioSource = GetFreeAudioSource(_soundAudioSourceArr);
			PlayAudio(audioSource, audioEnum);
		}

        AudioSource GetFreeAudioSource(AudioSource[] pool)
		{
			for (int i = 0; i < pool.Length; i++)
			{
				if (pool[i].isPlaying == false)
					return pool[i];
			}
			Debug.LogError("音源数组大小不足，导致需要提前终止音效的播放，请增加数组容量");
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

		public void StopSpecificAudio(EMusicName music)
        {
			AudioSource asource = null;
			if (_audioSourceDic.TryGetValue(music, out asource))
			{
				asource.Stop();
			}
			else
				Debug.Log("找不到正在播放 " + music.ToString() + " 的音源组件");
		}
	}
}



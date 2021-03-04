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
		/// <summary>
		/// 根据指定的次数来播放音效
		/// </summary>
		/// <returns>The sound coroutine.</returns>
		//public void PlaySoundBySpecificTimes(EMusicName audioEnum, int times)
		//{
		//	Objecr是所有类型的祖先，用它来定义数组，数组中的元素可以存放不同类型的数据
		//	Object[] objArray = new object[2];
		//	objArray [0] = audioEnum;
		//	objArray [1] = times;
		//	当用StratCoroutine的第一种参数列表(即传一个字符串)时，只能传一个参数，用这个数组便可以将两个参数传入只能传一个参数的函数
		//	StopAllCoroutines();
		//	StartCoroutine(PlaySoundCoroutine(audioEnum, times));
		//}
		//IEnumerator PlaySoundCoroutine(AudioEnum audioEnum, int times)
		//{
		//	AudioClip clip = GetAudio(audioEnum);
		//	for (int i = 0; i < times; i++)
		//	{
		//		AudioSource audioSource = GetFreeAudioSource();
		//		audioSource.clip = clip;
		//		audioSource.Play();
		//		yield return new WaitForSeconds(clip.length + 0.5f);
		//	}
		//}
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



using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {
	const string BGM_PATH = "Audio/Bgm/";
	const string SE_PATH = "Audio/Se/";

	// リソース名と一致させること。使用時はToLowerする前提
	public enum BgmName {
		Default,
	}
	
	// リソース名と一致させること。使用時はToLowerする前提
	public enum SeName {
		Throw,
		Slash,
		Drum,
		Shoji,
		Hit_Voice,
		Hit_Voice_2,
		Hit_Voice_3,
		Hit_Weapon,
		Break,
	}

	Dictionary<BgmName, AudioClip> bgmPool = new Dictionary<BgmName, AudioClip>();
	Dictionary<SeName, AudioClip> sePool = new Dictionary<SeName, AudioClip>();

	AudioSource bgmSource;
	AudioSource seSource;


	protected override void Awake () {
		base.Awake();

		bgmSource = gameObject.AddComponent<AudioSource>();
		seSource = gameObject.AddComponent<AudioSource>();

		//PlayerDataManager.Instance.OnChangeBGMEnabledListener += () => {
		//	if (PlayerDataManager.Instance.IsBGMEnabled) {
		//		PlayBgm(BgmName.Default);
		//	} else {
		//		StopBgm();
		//	}
		//};

		//PlayerDataManager.Instance.OnChangeSEEnabledListener += () => {
		//	if (!PlayerDataManager.Instance.IsSEEnabled) {
		//		StopSe();
		//	}
		//};
	}

	void Start () {
		LoadBgm(BgmName.Default);
		PlayBgm(BgmName.Default);
	}


	public void LoadBgm (BgmName key) {
		if (!bgmPool.ContainsKey(key)) {
			var path = BGM_PATH + key.Name().ToLower();
			var clip = Resources.Load<AudioClip>(path);

			Assert.IsNotNull(clip);
			bgmPool.Add(key, clip);
		}
	}

	public void LoadSe (SeName key) {
		if (!sePool.ContainsKey(key)) {
			var path = SE_PATH + key.Name().ToLower();
			var clip = Resources.Load<AudioClip>(path);

			Assert.IsNotNull(clip);
			sePool.Add(key, clip);
		}
	}

	public void PlayBgm (BgmName key, bool isLoop = true) {
		//if (!PlayerDataManager.Instance.IsBGMEnabled) return;

		Assert.IsTrue(bgmPool.ContainsKey(key));
		if (bgmSource.isPlaying) {
			return;
		}

		SettingBgm(bgmSource, key, isLoop);
		bgmSource.Play();
	}

	public void PlaySe (SeName key) {
		//if (!PlayerDataManager.Instance.IsSEEnabled) return;

		Assert.IsTrue(sePool.ContainsKey(key));

		seSource.PlayOneShot(sePool[key]);
	}

	public void PauseBgm () {
		bgmSource.Pause();
	}

	public void StopBgm () {
		bgmSource.Stop();
	}

	public void StopSe () {
		seSource.Stop();
	}

	public void CrossFadeBgm (BgmName nextKey, bool isLoop = true) {
		AudioSource temp = gameObject.AddComponent<AudioSource>();
		SettingBgm(temp, nextKey, isLoop);

		var fadeInBgm = temp;
		var fadeOutBgm = bgmSource;

		var seq = DOTween.Sequence();
		seq.OnStart(() => {
			fadeInBgm.volume = 0;
			fadeInBgm.Play();
			fadeInBgm.DOKill();
			fadeOutBgm.DOKill();
		});
		seq.Append(fadeInBgm.DOFade(1, 2.5f));
		seq.Join(fadeOutBgm.DOFade(0, 2.5f));
		seq.OnComplete(() => {
			bgmSource = fadeInBgm;
			Destroy(fadeOutBgm);
		});
	}

	void SettingBgm (AudioSource source, BgmName key, bool isLoop = false) {
		source.clip = bgmPool[key];
		source.loop = isLoop;
		source.playOnAwake = false;
		source.priority = 0; // BGMなので最優先
		source.volume = 0.3f;
	}

}

using UnityEngine;
using System.Collections;

public class SoundEffectPlayer : MonoBehaviour {

	AudioClip soundEffectWalk;
	AudioClip soundEffectJump;
	AudioClip soundEffectDie;
	AudioClip soundEffectWin;
	AudioClip backgroundMusic;

	AudioSource[] audioSources;
	AudioSource walkAudioSource;
	AudioSource backgroundAudioSource;

	void Start () {
	
		soundEffectWalk = Resources.Load<AudioClip>("Sounds/dk-a2600_walk");
		soundEffectJump = Resources.Load<AudioClip>("Sounds/dk-a2600_jump");
		soundEffectDie = Resources.Load<AudioClip>("Sounds/dk-a2600_die");
		soundEffectWin = Resources.Load<AudioClip>("Sounds/dk-a2600_victory");
		backgroundMusic = Resources.Load<AudioClip>("Sounds/Donkey_Kong_Background_Level_1");

		audioSources = new AudioSource[5];
		for (int i = 0; i < 5; i++) {
			audioSources[i] = gameObject.AddComponent<AudioSource>();
		}
		walkAudioSource = gameObject.AddComponent<AudioSource>();
		walkAudioSource.clip = soundEffectWalk;
		walkAudioSource.loop = true;
		walkAudioSource.playOnAwake = false;

		backgroundAudioSource = gameObject.AddComponent<AudioSource>();
		backgroundAudioSource.clip = backgroundMusic;
		backgroundAudioSource.loop = true;
		backgroundAudioSource.volume = 0.7f;
		backgroundAudioSource.Play();
	}
	
	void PlaySoundEffect(AudioClip clip) {
		
		foreach (AudioSource source in audioSources) {
			if (!source.isPlaying) {
				source.clip = clip;
				source.Play();
				return;
			}
		}
	}

	public void PlayWalkEffect(bool play) {
		if (play) {
			if (!walkAudioSource.isPlaying) walkAudioSource.Play();
		}
		else
			walkAudioSource.Pause();
	}
	public void PlayJumpEffect() {
		PlaySoundEffect(soundEffectJump);
	}
	public void PlayDieEffect() {
		PlaySoundEffect(soundEffectDie);
	}
	public void PlayWinEffect() {
		PlaySoundEffect(soundEffectWin);
	}
	public void StopBackgroundMusic() {
		backgroundAudioSource.Stop();
	}
}

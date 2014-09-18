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
	
		// Load all the sound files
		soundEffectWalk = Resources.Load<AudioClip>("Sounds/dk-a2600_walk");
		soundEffectJump = Resources.Load<AudioClip>("Sounds/dk-a2600_jump");
		soundEffectDie = Resources.Load<AudioClip>("Sounds/dk-a2600_die");
		soundEffectWin = Resources.Load<AudioClip>("Sounds/dk-a2600_victory");
		backgroundMusic = Resources.Load<AudioClip>("Sounds/Donkey_Kong_Background_Level_1");

		// Create a few Audio Sources (in case we want to play more than one sound at a time)
		audioSources = new AudioSource[5];
		for (int i = 0; i < 5; i++) {
			audioSources[i] = gameObject.AddComponent<AudioSource>();
		}

		// Create a dedicated Audio Source for the walk sound
		walkAudioSource = gameObject.AddComponent<AudioSource>();
		walkAudioSource.clip = soundEffectWalk;
		walkAudioSource.loop = true;
		walkAudioSource.playOnAwake = false;

		// Create a dedicated Audio Source for the background music
		backgroundAudioSource = gameObject.AddComponent<AudioSource>();
		backgroundAudioSource.clip = backgroundMusic;
		backgroundAudioSource.loop = true;
		backgroundAudioSource.volume = 0.7f;
		backgroundAudioSource.Play();
	}
	
	void PlaySoundEffect(AudioClip clip) {

		// Find an available Audio Source and use it to play the sound
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
		else {
			walkAudioSource.Pause();
		}
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

﻿using UnityEngine;
using System.Collections;

public class AudioPlay : MonoBehaviour {

	public GameObject[] audioGameObjects;

	//FIRST AUDIO SOURCE	--	audioGameObjects[0]
		public AudioClip[] audioClipsA_S1;
		public AudioClip[] audioClipsA_S2;
		public AudioClip[] audioClipsA_S3;
	//SECOND AUDIO SOURCE	--	audioGameObjects[1]
		public AudioClip[] audioClipsB_S1;
		public AudioClip[] audioClipsB_S2;
		public AudioClip[] audioClipsB_S3;
	//THIRD AUDIO SOURCE	--	audioGameObjects[2]
		public AudioClip[] audioClipsC_S1;
		public AudioClip[] audioClipsC_S2;
		public AudioClip[] audioClipsC_S3;

	private void Start()
	{
		VaryMusic = false;
	}

	private void Update ()
	{
		if(GameManager.Instance.PlayerScore >= 28 || !VaryMusic)
		{
			PlaySetOne();
		}
		else if(GameManager.Instance.PlayerScore >= 16)
		{
			PlaySetTwo();
		}
		else
		{
			PlaySetThree();
		}
	}

	private void PlaySetOne(){
		if(audioGameObjects[0].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[0].audio.clip = audioClipsA_S1[rand];
			audioGameObjects[0].audio.Play();
		}
		if(audioGameObjects[1].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[1].audio.clip = audioClipsB_S1[rand];
			audioGameObjects[1].audio.Play();
		}
		if(audioGameObjects[2].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[2].audio.clip = audioClipsC_S1[rand];
			audioGameObjects[2].audio.Play();
		}
	}

	private void PlaySetTwo(){
		if(audioGameObjects[0].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[0].audio.clip = audioClipsA_S2[rand];
			audioGameObjects[0].audio.Play();
		}
		if(audioGameObjects[1].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[1].audio.clip = audioClipsB_S2[rand];
			audioGameObjects[1].audio.Play();
		}
		if(audioGameObjects[2].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[2].audio.clip = audioClipsC_S2[rand];
			audioGameObjects[2].audio.Play();
		}
	}

	private void PlaySetThree(){
		if(audioGameObjects[0].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[0].audio.clip = audioClipsA_S3[rand];
			audioGameObjects[0].audio.Play();
		}
		if(audioGameObjects[1].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[1].audio.clip = audioClipsB_S3[rand];
			audioGameObjects[1].audio.Play();
		}
		if(audioGameObjects[2].audio.isPlaying==false){
			int rand = Random.Range(0,2);
			audioGameObjects[2].audio.clip = audioClipsC_S3[rand];
			audioGameObjects[2].audio.Play();
		}
	}

	public bool VaryMusic { get; set; }

}
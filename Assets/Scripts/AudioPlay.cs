using UnityEngine;
using System.Collections;

public class AudioPlay : MonoBehaviour
{
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
	//MUSIC FOR END CRYING SCENE
	public AudioClip endCryMusic;
	//MUSIC FOR END DANCING SCENE
	public AudioClip endDanceMusic;
	private bool _playingEndMusic;
	
	private void Start()
	{
		VaryMusic = false;
		_playingEndMusic = false;
	}
	
	private void Update()
	{
		if(Application.loadedLevelName == "end_cry" && !_playingEndMusic)
		{
			stopEverything();
			audio.clip = endCryMusic;
			audio.loop = true;
			audio.Play();
			_playingEndMusic = true;
		}
		else if(Application.loadedLevelName == "end_dance" && !_playingEndMusic)
		{
			stopEverything();
			audio.clip = endDanceMusic;
			audio.loop = true;
			audio.Play();
			_playingEndMusic = true;
		}
		else if(!_playingEndMusic)
		{
			if(GameManager.Instance.PlayerScore >= 28 || !VaryMusic)
			{
				audioGameObjects[0].audio.mute = false;
				audioGameObjects[1].audio.mute = false;
				audioGameObjects[2].audio.mute = false;
				audioGameObjects[3].audio.mute = true;
				audioGameObjects[4].audio.mute = true;
				audioGameObjects[5].audio.mute = true;
				audioGameObjects[6].audio.mute = true;
				audioGameObjects[7].audio.mute = true;
				audioGameObjects[8].audio.mute = true;
				
			}
			else if(GameManager.Instance.PlayerScore >= 8)
			{
				audioGameObjects[0].audio.mute = true;
				audioGameObjects[1].audio.mute = true;
				audioGameObjects[2].audio.mute = true;
				audioGameObjects[3].audio.mute = false;
				audioGameObjects[4].audio.mute = false;
				audioGameObjects[5].audio.mute = false;
				audioGameObjects[6].audio.mute = true;
				audioGameObjects[7].audio.mute = true;
				audioGameObjects[8].audio.mute = true;
			}
			else
			{
				audioGameObjects[0].audio.mute = true;
				audioGameObjects[1].audio.mute = true;
				audioGameObjects[2].audio.mute = true;
				audioGameObjects[3].audio.mute = true;
				audioGameObjects[4].audio.mute = true;
				audioGameObjects[5].audio.mute = true;
				audioGameObjects[6].audio.mute = false;
				audioGameObjects[7].audio.mute = false;
				audioGameObjects[8].audio.mute = false;
			}
			
			if(audioGameObjects[0].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[0].audio.clip = audioClipsA_S1[rand];
				audioGameObjects[0].audio.Play();
			}
			if(audioGameObjects[1].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[1].audio.clip = audioClipsB_S1[rand];
				audioGameObjects[1].audio.Play();
			}
			if(audioGameObjects[2].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[2].audio.clip = audioClipsC_S1[rand];
				audioGameObjects[2].audio.Play();
			}
			
			
			if(audioGameObjects[3].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[3].audio.clip = audioClipsA_S2[rand];
				audioGameObjects[3].audio.Play();
			}
			if(audioGameObjects[4].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[4].audio.clip = audioClipsB_S2[rand];
				audioGameObjects[4].audio.Play();
			}
			if(audioGameObjects[5].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[5].audio.clip = audioClipsC_S2[rand];
				audioGameObjects[5].audio.Play();
			}
			
			if(audioGameObjects[6].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[6].audio.clip = audioClipsA_S3[rand];
				audioGameObjects[6].audio.Play();
			}
			if(audioGameObjects[7].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[7].audio.clip = audioClipsB_S3[rand];
				audioGameObjects[7].audio.Play();
			}
			if(audioGameObjects[8].audio.isPlaying == false)
			{
				int rand = Random.Range(0, 2);
				audioGameObjects[8].audio.clip = audioClipsC_S3[rand];
				audioGameObjects[8].audio.Play();
			}
		}
	}

	public void Reset()
	{
		VaryMusic = false;
		_playingEndMusic = false;
	}

	public void stopEverything()
	{
		audioGameObjects[0].audio.mute = true;
		audioGameObjects[1].audio.mute = true;
		audioGameObjects[2].audio.mute = true;
		audioGameObjects[3].audio.mute = true;
		audioGameObjects[4].audio.mute = true;
		audioGameObjects[5].audio.mute = true;
		audioGameObjects[6].audio.mute = true;
		audioGameObjects[7].audio.mute = true;
		audioGameObjects[8].audio.mute = true;
		if(audio.isPlaying)
		{
			audio.Stop();
		}
	}
	
	public bool VaryMusic { get; set; }
}
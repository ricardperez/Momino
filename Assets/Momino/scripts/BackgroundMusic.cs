using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {
	
	private AudioSource audioSource;
	public AudioClip menuMusic;
	public AudioClip gameMusic;
	public AudioClip gameFastMusic;
	
	// Use this for initialization
	void Start () {
		this.audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void playMenuMusic()
	{
		this.audioSource.clip = this.menuMusic;
	}
	
	public void playGameMusic()
	{
		this.audioSource.clip = this.gameMusic;
		this.audioSource.Play();
	}
	
	public void playGameFastMusic()
	{
		this.audioSource.clip = this.gameFastMusic;
		this.audioSource.Play();
	}
}

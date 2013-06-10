using UnityEngine;
using System.Collections;

public class ClickSoundGC : MonoBehaviour {
	
	private bool alreadyEvaluated;
	
	// Use this for initialization
	void Start () {
		this.alreadyEvaluated = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.alreadyEvaluated)
		{
			this.audio.Play();
			this.alreadyEvaluated = true;
		} else
		{
			if (!this.audio.isPlaying)
			{
//				this.audio.Play();
				Destroy(this.gameObject);
			}
		}
	}
}

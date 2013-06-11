using UnityEngine;
using System.Collections;

public class FinishScript : MonoBehaviour
{
	private GUIStyle labelsStyle;
	public AudioClip successSound;
	public AudioClip failureSound;
	public AudioClip mouseClickSound;
	private GUIStyle buttonsStyle;
	// Use this for initialization
	void Start()
	{
		this.labelsStyle = new GUIStyle();
		labelsStyle.normal.textColor = Color.black;
		labelsStyle.alignment = TextAnchor.MiddleCenter;
		labelsStyle.fontSize = 30;
		
		
		AudioSource audioSource = Camera.mainCamera.GetComponent<AudioSource>();
		if (GameProperties.gameSuccess)
		{
			audioSource.clip = this.successSound;
		} else
		{
			audioSource.clip = this.failureSound;
		}
		audioSource.Play();
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	void OnGUI()
	{
		if (this.buttonsStyle == null)
		{
			this.buttonsStyle = new GUIStyle(GUI.skin.button);
			if (GameProperties.IsTactil())
			{
				this.buttonsStyle.fontSize = 20;
			}
		}
		GUI.Label(new Rect(((Screen.width - 300) / 2), 100, 300, 30), (GameProperties.gameSuccess ? "Congrats, you finished the level!" : "Ooops, you failed..."), this.labelsStyle);
		
		float buttonsHeight = (GameProperties.IsTactil() ? 80.0f : 50.0f);
		float buttonsWidth = (GameProperties.IsTactil() ? 180.0f : 150.0f);
		float buttonsSep = (GameProperties.IsTactil() ? 20.0f : 10.0f);
		int nButtons = 3;
		
		float currY = 70.0f + (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GameProperties.gameSuccess)
		{
			
			if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Next level", this.buttonsStyle))
			{
				this.audio.clip = this.mouseClickSound;
				this.audio.Play();
				GameProperties.level++;
				Application.LoadLevel(1);
			}
		} else
		{
			if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Replay", this.buttonsStyle))
			{
				this.audio.clip = this.mouseClickSound;
				this.audio.Play();
				Application.LoadLevel(1);
			}
		}
		
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Exit", this.buttonsStyle))
		{
			this.audio.clip = this.mouseClickSound;
				this.audio.Play();
			Application.LoadLevel(0);
		}
	}
}

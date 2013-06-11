using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{
	private GUIStyle titleLabelsStyle;
	private GUIStyle labelsStyle;
	private GUIStyle buttonsStyle;
	public AudioClip buttonsClick;

	// Use this for initialization
	void Start()
	{
		this.labelsStyle = new GUIStyle();
		this.labelsStyle.normal.textColor = Color.black;
		this.labelsStyle.alignment = TextAnchor.MiddleCenter;
		if (GameProperties.IsTactil())
		{
			this.labelsStyle.fontSize = 16;
		}
		
		this.titleLabelsStyle = new GUIStyle();
		this.titleLabelsStyle.normal.textColor = Color.black;
		this.titleLabelsStyle.alignment = TextAnchor.MiddleCenter;
		this.titleLabelsStyle.fontSize = 35;
		
		this.buttonsStyle = null;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameProperties.paused)
			{
				GameProperties.paused = false;
				Application.LoadLevel(1);
			}
		}
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
		float buttonsHeight = (GameProperties.IsTactil() ? 80.0f : 50.0f);
		float buttonsWidth = (GameProperties.IsTactil() ? 180.0f : 150.0f);
		float buttonsSep = 10.0f;
		int nButtons = (GameProperties.IsTactil() ? 1 : 3);
		
		GUI.Label(new Rect(((Screen.width - 300) / 2), 50, 300, 30), "Welcome to Momino!", this.titleLabelsStyle);
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (!GameProperties.IsTactil())
		{
			GUI.Label(new Rect(((Screen.width - 300) / 2), (currY - 60), 300, 30), "Select a game mode and start playing", this.labelsStyle);
		}
		
		string firstTitle = (GameProperties.IsTactil() ? "Start game" : "Targets mode");
		if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), firstTitle, this.buttonsStyle))
		{
			this.audio.clip = this.buttonsClick;
			audio.Play();
			GameProperties.gameType = GameType.kGameTypeMominoTargets;
			GameProperties.editMode = EditMode.kEditModeDominos;
			GameProperties.paused = false;
			GameProperties.level = 1;
			Time.timeScale = 1.0f;
			Application.LoadLevel(1);
			AudioListener.pause = false;
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (!GameProperties.IsTactil())
		{
			if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Momino", this.buttonsStyle))
			{
				this.audio.clip = this.buttonsClick;
				audio.Play();
				GameProperties.gameType = GameType.kGameTypeMomino;
				GameProperties.editMode = EditMode.kEditModeDominos;
				GameProperties.paused = false;
				GameProperties.level = 1;
				Time.timeScale = 1.0f;
				Application.LoadLevel(1);
				AudioListener.pause = false;
			}
			currY += (buttonsSep + buttonsHeight);
		
			if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "God", this.buttonsStyle))
			{
				this.audio.clip = this.buttonsClick;
				audio.Play();
				GameProperties.gameType = GameType.kGameTypeGod;
				GameProperties.editMode = EditMode.kEditModeDominos;
				GameProperties.paused = false;
				GameProperties.level = 1;
				Time.timeScale = 1.0f;
				Application.LoadLevel(1);
				AudioListener.pause = false;
			}
			currY += (buttonsSep + buttonsHeight);
		
			if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Exit", this.buttonsStyle))
			{
				this.audio.clip = this.buttonsClick;
				audio.Play();
				Application.Quit();
			}
		}
		
		GUI.Label(new Rect(((Screen.width - 300) / 2), Screen.height - 100, 300, 30), "Developer: Ricard Perez", this.labelsStyle);
		GUI.Label(new Rect(((Screen.width - 300) / 2), Screen.height - 80, 300, 30), "Sound director: Pau Bukowski", this.labelsStyle);
	}
}

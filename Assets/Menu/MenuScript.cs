using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{
	
	private GUIStyle labelsStyle;

	// Use this for initialization
	void Start()
	{
		this.labelsStyle = new GUIStyle();
		this.labelsStyle.normal.textColor = Color.black;
		this.labelsStyle.alignment = TextAnchor.MiddleCenter; 
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
		float buttonsHeight = 50.0f;
		float buttonsSep = 10.0f;
		int nButtons = 3;
		
		
		GUI.Label(new Rect(((Screen.width - 300)/2), 50, 300, 30), "Welcome to Momino!", this.labelsStyle);
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		GUI.Label(new Rect(((Screen.width - 300)/2), (currY - 60), 300, 30), "Select a game mode and start playing", this.labelsStyle);
		
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Targets mode"))
		{
			GameProperties.gameType = GameType.kGameTypeMominoTargets;
			GameProperties.editMode = EditMode.kEditModeDominos;
			GameProperties.paused = false;
			GameProperties.level = 1;
			Time.timeScale = 1.0f;
			Application.LoadLevel(1);
			AudioListener.pause = false;
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Momino"))
		{
			GameProperties.gameType = GameType.kGameTypeMomino;
			GameProperties.editMode = EditMode.kEditModeDominos;
			GameProperties.paused = false;
			GameProperties.level = 1;
			Time.timeScale = 1.0f;
			Application.LoadLevel(1);
			AudioListener.pause = false;
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "God"))
		{
			GameProperties.gameType = GameType.kGameTypeGod;
			GameProperties.editMode = EditMode.kEditModeDominos;
			GameProperties.paused = false;
			GameProperties.level = 1;
			Time.timeScale = 1.0f;
			Application.LoadLevel(1);
			AudioListener.pause = false;
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			Application.Quit();
		}
		
		
		
		GUI.Label(new Rect(((Screen.width - 300)/2), Screen.height-80, 300, 30), "Developed by ricard.perez!", this.labelsStyle);
	}
}

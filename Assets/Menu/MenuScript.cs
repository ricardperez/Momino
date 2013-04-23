using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	
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
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Play God mode"))
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
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Play with Momino"))
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
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			Application.Quit();
		}
	}
}

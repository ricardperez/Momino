using UnityEngine;
using System.Collections;

public class FinishScript : MonoBehaviour
{
	private GUIStyle labelsStyle;
	// Use this for initialization
	void Start()
	{
		this.labelsStyle = new GUIStyle();
		labelsStyle.normal.textColor = Color.black;
		labelsStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(((Screen.width - 300) / 2), 100, 300, 30), (GameProperties.gameSuccess ? "Congrats! You finished the level" : "Ooops, you failed"), this.labelsStyle);
		
		float buttonsHeight = 50.0f;
		float buttonsSep = 10.0f;
		int nButtons = 3;
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GameProperties.gameSuccess)
		{
			if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Next level"))
			{
				GameProperties.level++;
				Application.LoadLevel(1);
			}
		} else
		{
			if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Replay"))
			{
				Application.LoadLevel(1);
			}
		}
		
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			Application.LoadLevel(0);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum DominoColor
{
	kColorRandom = 0,
	kColorRed,
	kColorGreen,
	kColorBlue,
	kColorCyan,
	kColorSalmon,
	kColorViolet,
	kColorPink,
	kColorOrange,
	kColorWhite,
	kColorGray,
	kColorBlack,
	
	nColors
}

public class LevelPropertiesScript : MonoBehaviour
{
	private static LevelPropertiesScript singleton;
	public Transform mouseClick;
	public GameObject floor;
	public HashSet<GameObject> allSteps;
	public Camera[] cameras;
	private int currentIndex;
	private GUIStyle labelsStyle;
	public int maxNDominos = 100;
	public GameObject powerupPrefab;
	public int nPowerups = 5;
	public int nPowerupsGot = 0;
	public int nPowerupsExplode = 0;
	public ArrayList powerupsPositions;
	public ArrayList powerups;
	public int nDominos = 0;
	public int nDominosCombo = 0;
	public bool dominosFalling;
	public Vector3 dominosFallingPosition;
	double timeWithoutFalling;
	private DominoColor dominoColor = DominoColor.kColorRandom;
	private System.Random rnd;
	private GUIStyle buttonsStyle;
	private bool _wasPaused;
	private double timeSincePaused;
	private float _audioTime;
	
	void Awake()
	{
		LevelPropertiesScript.singleton = this;
		
		this.rnd = new System.Random();
		
		this.floor.transform.localScale = this.defaultFloorSize();
		
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			this.nPowerups = 3 + (GameProperties.level * 2);
			this.instantiatePowerups();
		}
	}
	
	void OnDestroy()
	{
		LevelPropertiesScript.singleton = null;
	}
	
	// Use this for initialization
	void Start()
	{
		this.labelsStyle = new GUIStyle();
		labelsStyle.normal.textColor = Color.black;
		if (GameProperties.IsTactil())
		{
			labelsStyle.fontSize = 15;
		}
		
		switch (GameProperties.gameType)
		{
		case GameType.kGameTypeMominoTargets:
			this.maxNDominos = (int)(ShortestPathScript.sharedInstance().minRequiredDominos() * 1.35f);
			break;
		case GameType.kGameTypeGod:
			this.maxNDominos = 1000;
			break;
		case GameType.kGameTypeMomino:
		default:
			this.maxNDominos = -1;
			break;
		}
		
		if (this.allSteps == null)
		{
			this.allSteps = new HashSet<GameObject>();
		}
		
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			ShortestPathScript.sharedInstance().calculateCheckpoints();
			ShortestPathScript.sharedInstance().instantiatePath();
		}
		
		MominoScript.sharedInstance().reset();
		MominoScript.sharedInstance().gameObject.SetActive(GameProperties.gameType != GameType.kGameTypeGod);
		
		this.enableFirstCamera();
	}
	
	public static LevelPropertiesScript sharedInstance()
	{
		return LevelPropertiesScript.singleton;
	}
	
	void instantiatePowerups()
	{
		this.powerups = new ArrayList(this.nPowerups);
		this.powerupsPositions = new ArrayList(this.nPowerups);
				
		float posY = 1.5f;
		float minX = -1.0f;
		float maxX = 1.0f;
		float minZ = -1.0f;
		float maxZ = 1.0f;
				
		float mapSize = (10 + GameProperties.level * 2.0f);
			
		posY = (this.floor.transform.position.y + this.floor.transform.localScale.y * 0.5f) + 1.0f;
		minX = this.floor.transform.position.x - mapSize;
		maxX = this.floor.transform.position.x + mapSize;
		minZ = this.floor.transform.position.z - mapSize;
		maxZ = this.floor.transform.position.z + mapSize;
			
		for (int i=0; i<this.nPowerups; i++)
		{
			float posX = (float)(minX + this.rnd.NextDouble() * (maxX - minX));
			float posZ = (float)(minZ + this.rnd.NextDouble() * (maxZ - minZ));
			Vector3 position = new Vector3(posX, posY, posZ);
			this.powerupsPositions.Add(position);
			GameObject powerup = (GameObject)Instantiate(this.powerupPrefab, position, Quaternion.Euler(0, 0, 0));
					
			this.powerups.Add(powerup);
		}
	}
	
	void resetPowerups()
	{
		this.removeAllPowerups();
		if (this.powerupsPositions != null)
		{
			this.powerups = new ArrayList(this.powerupsPositions.Count);
			foreach (Vector3 powerupPosition in this.powerupsPositions)
			{
				GameObject powerup = (GameObject)Instantiate(this.powerupPrefab, powerupPosition, Quaternion.Euler(0, 0, 0));
				this.powerups.Add(powerup);
			}
		}
		this.nPowerupsGot = 0;
		this.nPowerupsExplode = 0;
	}
	
	void removeAllPowerups()
	{
		if (this.powerups != null)
		{
			foreach (GameObject powerup in this.powerups)
			{
				Destroy(powerup);
			}
			this.powerups = null;
		}
	}
	
	void Update()
	{
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			if (this.dominosFalling)
			{
				this.timeWithoutFalling += Time.deltaTime;
				if (this.timeWithoutFalling >= 3.0)
				{
					if (this.nPowerupsExplode >= this.nPowerups)
					{
						GameProperties.gameSuccess = true;
						Application.LoadLevel(2);
					} else
					{
						GameProperties.gameSuccess = false;
						Application.LoadLevel(2);
					}
				}
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameProperties.paused)
			{
				this.resume();
			} else
			{
				this.pause();
			}
		}
		
		if (this._wasPaused && !GameProperties.paused)
		{
			this.timeSincePaused += Time.deltaTime;
			if (this.timeSincePaused > 0.25)
			{
				this._wasPaused = false;
			}
		}
	}
	
	public void changeCamera()
	{
		if (this.cameras.Length > 1)
		{
			int nextIndex = ((this.currentIndex + 1) % (this.cameras.Length));
			this.setCamera(nextIndex);
		}
	}
	
	void OnGUI()
	{
		if (this.buttonsStyle == null)
		{
			this.buttonsStyle = new GUIStyle(GUI.skin.button);
			if (GameProperties.IsTactil())
			{
				this.buttonsStyle.fontSize = 16;
			}
		}
		
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			GUI.Label(new Rect(10, 10, 300, 50), "Dominos: " + this.nDominos + "/" + this.maxNDominos, this.labelsStyle);
		} else
		{
			GUI.Label(new Rect(10, 10, 300, 50), "Dominos: " + this.nDominos, this.labelsStyle);
		}
		
		GUI.Label(new Rect(10, 30, 300, 50), "Combo: " + this.nDominosCombo, this.labelsStyle);
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			GUI.Label(new Rect(10, 50, 300, 50), "Powerups: " + this.nPowerupsGot + "(" + this.nPowerupsExplode + ")" + "/" + this.nPowerups, this.labelsStyle);
		}
		
		if (GameProperties.paused)
		{
			this.displayPauseMenu();
		} else if (GameProperties.IsTactil())
		{
			if (GUI.Button(new Rect((Screen.width - 100.0f), 15.0f, 80.0f, 40.0f), "Menu", this.buttonsStyle))
			{
				Instantiate(this.mouseClick);
				this.pause();
			}
		}
	}
	
	void displayPauseMenu()
	{
		float buttonsHeight = (GameProperties.IsTactil() ? 80.0f : 50.0f);
		float buttonsSep = (GameProperties.IsTactil() ? 20.0f : 10.0f);
		float buttonsWidth = (GameProperties.IsTactil() ? 180.0f : 150.0f);
		int nButtons = 3;
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Resume", this.buttonsStyle))
		{
			Instantiate(this.mouseClick);
			this.resume();
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Reset", this.buttonsStyle))
		{
			Instantiate(this.mouseClick);
			CreateDominos.sharedInstance().reset();
			EditModeScript.sharedInstance().reset();
			if (MominoScript.sharedInstance() != null)
			{
				MominoScript.sharedInstance().reset();
			}
			
			if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
			{
				this.resetPowerups();
			}
			this.dominosFalling = false;
			this.enableFirstCamera();
			this.floor.transform.localScale = this.defaultFloorSize();
			
			this.resume();
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - buttonsWidth) / 2, currY, buttonsWidth, buttonsHeight), "Exit", this.buttonsStyle))
		{
			Instantiate(this.mouseClick);
			this.resume();
			Application.LoadLevel(0);
		}
	}
	
	public bool positionIsOnGUI(Vector3 screenPos)
	{
		bool found = false;
		if (GameProperties.paused)
		{
			float buttonsHeight = (GameProperties.IsTactil() ? 80.0f : 50.0f);
			float buttonsSep = (GameProperties.IsTactil() ? 20.0f : 10.0f);
			float buttonsWidth = (GameProperties.IsTactil() ? 180.0f : 150.0f);
			
			int nButtons = 3;
			float startY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2.0f;
			float startX = (Screen.width - buttonsWidth) / 2.0f;
		
			float endY = (startY + nButtons * buttonsHeight + (nButtons - 1) * buttonsSep);
			float endX = startX + buttonsWidth;
		
			if ((screenPos.x >= startX) && (screenPos.x <= endX) && (screenPos.y >= startY) && (screenPos.y <= endY))
			{
				int i = 0;
				float currY = startY;
				while (!found && i<nButtons)
				{
					Rect buttonRect = new Rect(startX, currY, buttonsWidth, buttonsHeight);
					found = buttonRect.Contains(screenPos);
					i++;	
				}
			}
		}
		
		return found;
			
	}
	
	void enableFirstCamera()
	{
		bool found = false;
		int i = 0;
		while (!found && i<this.cameras.Length)
		{
			Camera camera = this.cameras[i];
			found = camera.enabled;
			if (!found)
			{
				i++;
			}
		}
		if (found)
		{
			this.currentIndex = i;
		} else
		{
			if (this.cameras.Length > 0)
			{
				this.setCamera(0);
			} else
			{
				this.currentIndex = -1;
			}
		}
		
		for (int j=0; j<cameras.Length; j++)
		{
			if (j != i)
			{
				Camera camera = this.cameras[j];
				camera.enabled = false;
			}
		}
	}
	
	void setCamera(int index)
	{
		Camera oldCamera = this.cameras[this.currentIndex];
		oldCamera.enabled = false;
		AudioListener oldAudioListener = oldCamera.GetComponent<AudioListener>();
		oldAudioListener.enabled = false;
		
		this.currentIndex = index;
		Camera currentCamera = this.cameras[this.currentIndex];
		currentCamera.enabled = true;
		AudioListener currentAudioListener = currentCamera.GetComponent<AudioListener>();
		currentAudioListener.enabled = true;
	}
	
	public void setFollowCamera()
	{
		this.setCamera(2);
	}
	
	public void setMainCamera()
	{
		this.setCamera(0);
	}
	
	private Vector3 defaultFloorSize()
	{
		return new Vector3(500, 1, 500);
	}
	
	public void updateFallingPosition(Vector3 position)
	{
		this.dominosFallingPosition = position;
		this.timeWithoutFalling = 0.0;
	}
	
	public GameObject stairsStepAtPosition(Vector3 position)
	{	
		Vector3 hitPos;
		Ray ray = new Ray(new Vector3(position.x, -1.0f, position.y), Vector3.up);
		GameObject step = this.rayCastWithStairSteps(ray, out hitPos);
		return step;
	}
	
	public GameObject rayCastWithStairSteps(Ray ray, out Vector3 hitPosition)
	{
		hitPosition = new Vector3(0, 0, 0);
		GameObject collidingStep = null;
		
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject.tag == "StairStep")
			{
				collidingStep = hit.collider.gameObject;
				hitPosition = hit.point;
			}
		}
		return collidingStep;
	}
	
	public void addStep(GameObject step)
	{
		if (this.allSteps == null)
		{
			this.allSteps = new HashSet<GameObject>();
		}
		this.allSteps.Add(step);
	}
	
	public void removeStep(GameObject step)
	{
		if (this.allSteps != null)
		{
			this.allSteps.Remove(step);
		}
	}
	
	public string currentColorName()
	{
		string c = null;
		switch (this.dominoColor)
		{
		case DominoColor.kColorBlack:
			c = "Black";
			break;
		case DominoColor.kColorGray:
			c = "Gray";
			break;
		case DominoColor.kColorWhite:
			c = "White";
			break;
		case DominoColor.kColorRed:
			c = "Red";
			break;
		case DominoColor.kColorGreen:
			c = "Green";
			break;
		case DominoColor.kColorBlue:
			c = "Blue";
			break;
		case DominoColor.kColorCyan:
			c = "Cyan";
			break;
		case DominoColor.kColorSalmon:
			c = "Salmon";
			break;
		case DominoColor.kColorViolet:
			c = "Violet";
			break;
		case DominoColor.kColorPink:
			c = "Pink";
			break;
		case DominoColor.kColorOrange:
			c = "Orange";
			break;
		case DominoColor.kColorRandom:
		default:
			c = "Random";
			break;
		}
		return c;
	}
	
	public Color currentColor()
	{
		Color c;
		switch (this.dominoColor)
		{
		case DominoColor.kColorBlack:
			c = Color.black;
			break;
		case DominoColor.kColorGray:
			c = Color.gray;
			break;
		case DominoColor.kColorWhite:
			c = Color.white;
			break;
		case DominoColor.kColorRed:
			c = Color.red;
			break;
		case DominoColor.kColorGreen:
			c = Color.green;
			break;
		case DominoColor.kColorBlue:
			c = Color.blue;
			break;
		case DominoColor.kColorCyan:
			c = Color.cyan;
			break;
		case DominoColor.kColorSalmon:
			c = new Color(0.98f, 0.50f, 0.45f);
			break;
		case DominoColor.kColorViolet:
			c = new Color(0.93f, 0.51f, 0.93f);
			break;
		case DominoColor.kColorPink:
			c = new Color(1.0f, 0.75f, 0.80f);
			break;
		case DominoColor.kColorOrange:
			c = new Color(1.0f, 0.64f, 0.0f);
			break;
		case DominoColor.kColorRandom:
		default:
			float r = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
			float g = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
			float b = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
			float a = 1.0f;
			c = new Color(r, g, b, a);
			break;
		}
		return c;
	}
	
	public void changeCurrentColor()
	{
		this.dominoColor = (DominoColor)((int)this.dominoColor + 1);
		if (this.dominoColor >= DominoColor.nColors)
		{
			this.dominoColor = (DominoColor)0;
		}
	}
	
	public void pause()
	{
		GameProperties.paused = true;
		AudioListener.pause = true;
		Time.timeScale = 0.000001f;
		
		this._audioTime = this.audio.time;
		this.audio.Pause();
		
		this.setWasPaused();
	}
	
	public void resume()
	{
		GameProperties.paused = false;
		AudioListener.pause = false;
		Time.timeScale = 1.0f;
		this.timeSincePaused = 0.0;
		
		this.audio.Play();
		this.audio.time = this._audioTime;
	}
	
	public bool wasPaused()
	{
		return this._wasPaused;
	}
	
	public void setWasPaused()
	{
		this.timeSincePaused = 0.0;
		this._wasPaused = true;
	}
}

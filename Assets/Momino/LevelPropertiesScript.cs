using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelPropertiesScript : MonoBehaviour
{
	private static LevelPropertiesScript singleton;
	public GameObject player;
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
	private ArrayList powerupsPositions;
	public ArrayList powerups;
	public int nDominos = 0;
	public int nDominosCombo = 0;
	public bool dominosFalling;
	public Vector3 dominosFallingPosition;
	double timeWithoutFalling;
	private float maxHeight = 100.0f;
	
	void Awake()
	{
		LevelPropertiesScript.singleton = this;
		
		this.floor.transform.localScale = this.defaultFloorSize();
		this.nPowerups = 3 + (GameProperties.level * 2);
		if (this.nPowerups > 10)
		{
			this.nPowerups = 10;
		}
		this.instantiatePowerups();
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
		
		if (this.player != null)
		{
			this.player.SetActive(GameProperties.gameType == GameType.kGameTypeMomino);
		}
		
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			this.maxNDominos = (int)(ShortestPathScript.sharedInstance().minRequiredDominos() * 1.35f);
		} else
		{
			this.maxNDominos = 1000;
		}
		
		if (this.allSteps == null)
		{
			this.allSteps = new HashSet<GameObject>();
		}
		
		this.enableFirstCamera();
	}
	
	public static LevelPropertiesScript sharedInstance()
	{
		return LevelPropertiesScript.singleton;
	}
	
	void instantiatePowerups()
	{
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			if (this.powerupPrefab != null)
			{
				this.powerups = new ArrayList(this.nPowerups);
				this.powerupsPositions = new ArrayList(this.nPowerups);
				
				float posY = 1.5f;
				float minX = -1.0f;
				float maxX = 1.0f;
				float minZ = -1.0f;
				float maxZ = 1.0f;
				
				float mapSize = (3 + GameProperties.level * 0.5f);
			
				if (this.floor != null)
				{
					posY = this.floor.transform.position.y + 1.0f;
					minX = this.floor.transform.position.x - mapSize * 5;
					maxX = this.floor.transform.position.x + mapSize * 5;
					minZ = this.floor.transform.position.z - mapSize * 5;
					maxZ = this.floor.transform.position.z + mapSize * 5;
				}
			
				System.Random r = new System.Random();
				for (int i=0; i<this.nPowerups; i++)
				{
					float posX = (float)(minX + r.NextDouble() * (maxX - minX));
					float posZ = (float)(minZ + r.NextDouble() * (maxZ - minZ));
					Vector3 position = new Vector3(posX, posY, posZ);
					this.powerupsPositions.Add(position);
					GameObject powerup = (GameObject)Instantiate(this.powerupPrefab, position, Quaternion.Euler(0, 0, 0));
					
					this.powerups.Add(powerup);
				}
			}
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
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.gameType == GameType.kGameTypeMomino)
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
			GameProperties.paused = (!GameProperties.paused);
			AudioListener.pause = GameProperties.paused;
			if (GameProperties.paused)
			{
				Time.timeScale = 0.000001f;
			} else
			{
				Time.timeScale = 1.0f;
			}
		}
		
		if (Input.GetButtonDown("ChangeCamera"))
		{
			if (this.cameras.Length > 1)
			{
				int nextIndex = ((this.currentIndex + 1) % (this.cameras.Length));
				this.setCamera(nextIndex);
			}
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 300, 50), "Dominos: " + this.nDominos + "/" + this.maxNDominos, this.labelsStyle);
		GUI.Label(new Rect(10, 30, 300, 50), "Combo: " + this.nDominosCombo, this.labelsStyle);
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			GUI.Label(new Rect(10, 50, 300, 50), "Powerups: " + this.nPowerupsGot + "(" + this.nPowerupsExplode + ")" + "/" + this.nPowerups, this.labelsStyle);
		}
		
		if (GameProperties.paused)
		{
			this.displayPauseMenu();
		}
	}
	
	void displayPauseMenu()
	{
		float buttonsHeight = 50.0f;
		float buttonsSep = 10.0f;
		int nButtons = 3;
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Resume"))
		{
			GameProperties.paused = false;
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Reset"))
		{
			CreateDominos.sharedInstance().deleteDominos();
			this.resetPowerups();
			EditModeScript.sharedInstance().reset();
			this.enableFirstCamera();
			
			this.floor.transform.localScale = this.defaultFloorSize();
			this.player.transform.position = new Vector3(0, this.player.transform.localScale.y * 0.5f, 0);
			this.player.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			GameProperties.paused = false;
			Application.LoadLevel(0);
		}
	}
	
	public bool positionIsOnGUI(Vector3 screenPos)
	{
		bool found = false;
		if (GameProperties.paused)
		{
			float buttonsHeight = 50.0f;
			float buttonsSep = 10.0f;
			
			int nButtons = 3;
			float startY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2.0f;
			float startX = (Screen.width - 150.0f) / 2.0f;
		
			float endY = (startY + nButtons * buttonsHeight + (nButtons - 1) * buttonsSep);
			float endX = startX + 150.0f;
		
			if ((screenPos.x >= startX) && (screenPos.x <= endX) && (screenPos.y >= startY) && (screenPos.y <= endY))
			{
				int i = 0;
				float currY = startY;
				while (!found && i<nButtons)
				{
					Rect buttonRect = new Rect(startX, currY, 150.0f, buttonsHeight);
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
		return new Vector3(100, 1, 100);
	}
	
	public void updateFallingPosition(Vector3 position)
	{
		this.dominosFallingPosition = position;
		this.timeWithoutFalling = 0.0;
	}
	
	public GameObject stairsStepAtPosition(Vector3 position)
	{
//		GameObject collidingStep = null;
//		foreach (GameObject nextStep in this.allSteps)
//		{
//			Vector3 stepPosition = nextStep.transform.position;
//			Vector3 stepSize = nextStep.transform.localScale;
//			
//			if ((position.x > (stepPosition.x - stepSize.x * 0.5f)) && (position.x < (stepPosition.x + stepSize.x * 0.5f)) && (position.z > (stepPosition.z - stepSize.z * 0.5f)) && (position.z < (stepPosition.z + stepSize.z * 0.5f)))
//			{
//				collidingStep = nextStep;
//				break;
//			}
//		}
//		return collidingStep;
		
		Vector3 hitPos;
		Ray ray = new Ray(new Vector3(position.x, -1.0f, position.y), Vector3.up);
		GameObject step = this.rayCastWithStairSteps(ray, out hitPos);
		if (step != null)
		{
			Debug.Log("Hited a step");
		}
		return step;
	}
	
	public GameObject rayCastWithStairSteps(Ray ray, out Vector3 hitPosition)
	{
		Debug.Log("raycasting steps");
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
}

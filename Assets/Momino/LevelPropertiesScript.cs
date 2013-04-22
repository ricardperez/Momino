using UnityEngine;
using System.Collections;

public class LevelPropertiesScript : MonoBehaviour
{
	private static LevelPropertiesScript singleton;
	public GameObject player;
	public GameObject floor;
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
		this.powerups = new ArrayList(this.powerupsPositions.Count);
		foreach (Vector3 powerupPosition in this.powerupsPositions)
		{
			GameObject powerup = (GameObject)Instantiate(this.powerupPrefab, powerupPosition, Quaternion.Euler(0, 0, 0));
			this.powerups.Add(powerup);
		}
		this.nPowerupsGot = 0;
		this.nPowerupsExplode = 0;
	}
	
	void removeAllPowerups()
	{
		foreach (GameObject powerup in this.powerups)
		{
			Destroy(powerup);
		}
		this.powerups = null;
	}
	
	// Update is called once per frame
	void Update()
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
			this.enableFirstCamera();
			
			this.floor.transform.localScale = this.defaultFloorSize();
			this.player.transform.position = new Vector3(0, this.player.transform.localScale.y * 0.5f, 0);
			this.player.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			if (MakeStairs.sharedInstance() != null)
			{
				MakeStairs.sharedInstance().allSteps = null;
			}
			
			GameProperties.paused = false;
			Application.LoadLevel(0);
		}
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
}

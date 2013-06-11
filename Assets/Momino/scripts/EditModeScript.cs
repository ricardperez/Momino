using UnityEngine;
using System.Collections;

public class EditModeScript : MonoBehaviour
{
	public GameObject[] prefabs;
	public Transform mouseClick;
	private static EditModeScript singleton;
	private float buttonsHeight = 25.0f;
	private float buttonsWidth = 100.0f;
	private float buttonsRightOffset = 20.0f;
	private float buttonsSep = 10.0f;
	private GameObject editingPrefab;
	private bool isPressingRotateKey = false;
	private float rotationX;
	public float rotationXSpeed = 50.0f;
	private ArrayList instances;
	private double timeSinceEditing;
	private bool hasToGoBackToEditDominos;
	
	public static EditModeScript sharedInstance()
	{
		return EditModeScript.singleton;
	}
	
	void Awake()
	{
		EditModeScript.singleton = this;
	}
	
	void OnDestroy()
	{
		EditModeScript.singleton = null;
	}
	
	// Use this for initialization
	void Start()
	{
		this.instances = new ArrayList(10);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.editMode == EditMode.kEditModePrefabs)
		{
			if (this.editingPrefab != null)
			{
				if (this.isPressingRotateKey)
				{
					this.rotationX -= Input.GetAxis("Mouse X") * this.rotationXSpeed * 0.02f;
					this.editingPrefab.transform.rotation = Quaternion.Euler(0.0f, this.rotationX, 0.0f);
				} else
				{
					Vector3 screenCoordinates = Input.mousePosition;
					GameObject floor = LevelPropertiesScript.sharedInstance().floor;
					Vector3 position = this.worldCoordinatesFromScreenCoordinates(screenCoordinates, (floor.transform.position + Vector3.up * floor.transform.localScale.y * 0.5f));
					this.editingPrefab.transform.position = position;
				}
				
				
				if (Input.GetMouseButtonDown(0))
				{
					if (this.editingPrefab.tag == "DominosCollection")
					{
						MakeDominos makeDominos = this.editingPrefab.GetComponent<MakeDominos>();
						makeDominos.saveStateAsOriginal();
					}
				
					if (MominoScript.sharedInstance() != null)
					{
						MominoScript.sharedInstance().updateCurrentFloor();
					}
					
					this.editingPrefab = null;
					this.timeSinceEditing = 0.0;
					this.hasToGoBackToEditDominos = true;
				}
			}
			
			if (this.hasToGoBackToEditDominos)
			{
				this.timeSinceEditing += Time.deltaTime;
				if (timeSinceEditing > 0.25)
				{
					GameProperties.editMode = EditMode.kEditModeDominos;
					this.hasToGoBackToEditDominos = false;
				}
			}
			
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (this.editingPrefab != null)
				{
					this.instances.Remove(this.editingPrefab);
					Destroy(this.editingPrefab);
					this.editingPrefab = null;
					GameProperties.editMode = EditMode.kEditModeDominos;
					this.hasToGoBackToEditDominos = false;
				}
			}
		}
		
		if (GameProperties.gameType != GameType.kGameTypeMominoTargets)
		{
			if (Input.GetButtonDown("ChangeCamera"))
			{
				Instantiate(this.mouseClick);
				LevelPropertiesScript.sharedInstance().changeCamera();
			}
		}
		
		if (Input.GetButtonDown("ChangeColor"))
		{
			Instantiate(this.mouseClick);
			LevelPropertiesScript.sharedInstance().changeCurrentColor();
		}
		
	}
	
	void OnGUI()
	{
		int nButtons = this.prefabs.Length;
		float currY = (Screen.height - nButtons * this.buttonsHeight - (nButtons - 1) * this.buttonsSep) / 2;
		float startX = (Screen.width - this.buttonsWidth - this.buttonsRightOffset);
		if (GameProperties.gameType != GameType.kGameTypeMominoTargets)
		{
			for (int i=0; i<nButtons; i++)
			{
				if (GUI.Button(new Rect(startX, currY, this.buttonsWidth, this.buttonsHeight), this.prefabs[i].name))
				{
					Instantiate(this.mouseClick);
					LevelPropertiesScript.sharedInstance().setWasPaused();
					GameProperties.editMode = EditMode.kEditModePrefabs;
					this.editingPrefab = (GameObject)Instantiate(this.prefabs[i], new Vector3(0, 0, 0), Quaternion.identity);
				
					if (this.editingPrefab.tag == "DominosCollection")
					{
						MakeDominos makeDominos = this.editingPrefab.GetComponent<MakeDominos>();
						makeDominos.applyCurrentColor();
					}
					
					this.rotationX = 0.0f;
					this.instances.Add(this.editingPrefab);
				}
				currY += (this.buttonsSep + this.buttonsHeight);
			}
			
			
			if (GUI.Button(new Rect(startX, 15.0f, this.buttonsWidth, this.buttonsHeight), ("Color (x): " + LevelPropertiesScript.sharedInstance().currentColorName())))
			{
				Instantiate(this.mouseClick);
				LevelPropertiesScript.sharedInstance().setWasPaused();
				LevelPropertiesScript.sharedInstance().changeCurrentColor();
			}
		
			if (GameProperties.gameType != GameType.kGameTypeGod)
			{
				bool shoot = MominoScript.sharedInstance().shoot;
				if (GUI.Button(new Rect(startX - this.buttonsWidth - this.buttonsRightOffset, 15.0f, this.buttonsWidth, this.buttonsHeight), (shoot ? "Stop (q)" : "Continue (q)")))
				{
					Instantiate(this.mouseClick);
					MominoScript.sharedInstance().shoot = !shoot;
				}
			}
		
			if (GUI.Button(new Rect(startX - (this.buttonsWidth + this.buttonsRightOffset) * 2, 15.0f, this.buttonsWidth, this.buttonsHeight), "Camera (c)"))
			{
				Instantiate(this.mouseClick);
				LevelPropertiesScript.sharedInstance().setWasPaused();
				LevelPropertiesScript.sharedInstance().changeCamera();
			}
			
		} else
		{
			if (!GameProperties.IsTactil())
			{
				if (GUI.Button(new Rect(startX, 15.0f, this.buttonsWidth, this.buttonsHeight), ("Color (x): " + LevelPropertiesScript.sharedInstance().currentColorName())))
				{
					Instantiate(this.mouseClick);
					LevelPropertiesScript.sharedInstance().setWasPaused();
					LevelPropertiesScript.sharedInstance().changeCurrentColor();
				}
		
				if (GameProperties.gameType != GameType.kGameTypeGod)
				{
					bool shoot = MominoScript.sharedInstance().shoot;
					if (GUI.Button(new Rect(startX - this.buttonsWidth - this.buttonsRightOffset, 15.0f, this.buttonsWidth, this.buttonsHeight), (shoot ? "Stop (q)" : "Continue (q)")))
					{
						Instantiate(this.mouseClick);
						MominoScript.sharedInstance().shoot = !shoot;
					}
				}
			}
		}
		
		
		Event e = Event.current;
		this.isPressingRotateKey = (e.alt || e.command || e.control);
	}
	
	public bool positionIsOnGUI(Vector3 screenPos)
	{
		bool isOnPrefabs = this.positionIsOnPrefabsGUI(screenPos);
		return isOnPrefabs;
	}
	
	private bool positionIsOnPrefabsGUI(Vector3 screenPos)
	{
		if (GameProperties.gameType != GameType.kGameTypeMominoTargets)
		{
			int nButtons = this.prefabs.Length;
			float startY = (Screen.height - nButtons * this.buttonsHeight - (nButtons - 1) * this.buttonsSep) / 2;
			float startX = (Screen.width - this.buttonsWidth - this.buttonsRightOffset);
		
			float endY = (startY + nButtons * this.buttonsHeight + (nButtons - 1) * this.buttonsSep);
			float endX = (Screen.width - this.buttonsRightOffset);
		
			return ((screenPos.x >= startX) && (screenPos.x <= endX) && (screenPos.y >= startY) && (screenPos.y <= endY));
		} else
		{
			return false;
		}
	}
	
	public Vector3 worldCoordinatesFromScreenCoordinates(Vector3 screenCoordinates, Vector3 floorPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenCoordinates);
		
		Vector3 position;
		Plane plane = new Plane(Vector3.up, floorPosition);
		float distance;
		if (plane.Raycast(ray, out distance))
		{
			position = ray.GetPoint(distance);
			return position;
		} else
		{
			position = new Vector3(0.0f, 0.0f, 0.0f);
		}
		
		return position;
	}
	
	public void reset()
	{
		foreach (GameObject instance in this.instances)
		{
			Destroy(instance);
		}
		this.instances = new ArrayList(10);
	}
}

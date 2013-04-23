using UnityEngine;
using System.Collections;

public class EditModeScript : MonoBehaviour
{

	public GameObject[] prefabs;
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
					Vector3 position = EditModeScript.worldCoordinatesFromScreenCoordinates(screenCoordinates, LevelPropertiesScript.sharedInstance().floor.transform.position);
					this.editingPrefab.transform.position = position;
				}
			}
			
			
			if (Input.GetMouseButtonDown(0))
			{
				GameProperties.editMode = EditMode.kEditModeDominos;
				this.editingPrefab = null;
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (this.editingPrefab != null)
			{
				this.instances.Remove(this.editingPrefab);
				Destroy(this.editingPrefab);
				this.editingPrefab = null;
			}
		}
	}
	
	void OnGUI()
	{
		int nButtons = this.prefabs.Length;
		
		float currY = (Screen.height - nButtons * this.buttonsHeight - (nButtons - 1) * this.buttonsSep) / 2;
		float startX = (Screen.width - this.buttonsWidth - this.buttonsRightOffset);
		for (int i=0; i<nButtons; i++)
		{
			if (GUI.Button(new Rect(startX, currY, this.buttonsWidth, this.buttonsHeight), this.prefabs[i].tag))
			{
				GameProperties.editMode = EditMode.kEditModePrefabs;
				this.editingPrefab = (GameObject)Instantiate(this.prefabs[i], new Vector3(0, 0, 0), Quaternion.identity);
				this.rotationX = 0.0f;
				this.instances.Add(this.editingPrefab);
			}
			currY += (this.buttonsSep + this.buttonsHeight);
		}
		
		Event e = Event.current;
		this.isPressingRotateKey = (e.alt || e.command || e.control);
	}
	
	private GameObject prefabButtonOnScreenCoordinates(Vector3 screenPos)
	{
		GameObject prefab = null;
		
		int nButtons = this.prefabs.Length;
		float startY = (Screen.height - nButtons * this.buttonsHeight - (nButtons - 1) * this.buttonsSep) / 2;
		float startX = (Screen.width - this.buttonsWidth - this.buttonsRightOffset);
		
		float endY = (startY + nButtons * this.buttonsHeight + (nButtons - 1) * this.buttonsSep);
		float endX = (Screen.width - this.buttonsRightOffset);
		
		if ((screenPos.x >= startX) && (screenPos.x <= endX) && (screenPos.y >= startY) && (screenPos.y <= endY))
		{
			int i = 0;
			float currY = startY;
			bool found = false;
			while (!found && i<nButtons)
			{
				Rect buttonRect = new Rect(startX, currY, this.buttonsWidth, this.buttonsHeight);
				found = buttonRect.Contains(screenPos);
				if (!found)
				{
					i++;
				}
			}
			
			if (found)
			{
				prefab = this.prefabs[i];
			}
		}
		
		return prefab;
	}
	
	public bool positionIsOnGUI(Vector3 screenPos)
	{
		return (this.prefabButtonOnScreenCoordinates(screenPos) != null);
	}
	
	public static Vector3 worldCoordinatesFromScreenCoordinates(Vector3 screenCoordinates, Vector3 floorPosition)
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

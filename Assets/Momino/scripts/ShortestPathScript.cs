using UnityEngine;
using System.Collections;

public class ShortestPathScript : MonoBehaviour
{
	public ArrayList checkpoints;
	private static ShortestPathScript singleton;
	public Transform pathPrefab;
	
	public static ShortestPathScript sharedInstance()
	{
		return singleton;
	}
	
	void Awake()
	{
		ShortestPathScript.singleton = this;
	}
	
	void OnDestroy()
	{
		ShortestPathScript.singleton = null;
	}
	
	// Use this for initialization
	void Start()
	{
	}
	
	public void calculateCheckpoints()
	{
		LevelPropertiesScript properties = LevelPropertiesScript.sharedInstance();
		this.checkpoints = new ArrayList(properties.powerups.Count);
		
		ArrayList placesToGo = new ArrayList(properties.powerups.Count);
		foreach (GameObject powerup in properties.powerups)
		{
			placesToGo.Add(powerup.transform.position);
		}
		
		Vector3 nextPos = (Vector3)placesToGo[0];
		this.checkpoints.Add(nextPos);
		placesToGo.RemoveAt(0);
		while (placesToGo.Count > 0)
		{
			float minDistance = float.MaxValue;
			Vector3 closestPosition = nextPos;
			foreach (Vector3 position in placesToGo)
			{
				float dist = Vector3.SqrMagnitude(position - nextPos);
				if (dist < minDistance)
				{
					closestPosition = position;
					minDistance = dist;
				}
			}
			
			nextPos = closestPosition;
			this.checkpoints.Add(closestPosition);
			placesToGo.Remove(closestPosition);
		}
	}
	
	public void instantiatePath()
	{
		GameObject floor = LevelPropertiesScript.sharedInstance().floor;
		float floorYPos = floor.transform.position.y + floor.transform.localScale.y * 0.5f + +0.01f;
		if (this.checkpoints.Count > 0)
		{
			GameObject circleGO;
			Circle circle;
			Vector3 prevPos = (Vector3)this.checkpoints[0];
			for (int i=1; i<this.checkpoints.Count; i++)
			{
				Vector3 nextPos = (Vector3)this.checkpoints[i];
				Vector3 pos = new Vector3((nextPos.x + prevPos.x) / 2.0f, floorYPos, (nextPos.z + prevPos.z) / 2.0f);
				Quaternion lookRotation = Quaternion.LookRotation(nextPos - prevPos);
				Quaternion rotation = Quaternion.Euler(0.0f, lookRotation.eulerAngles.y, 0.0f);
				GameObject pathPlane = ((Transform)Instantiate(this.pathPrefab, pos, rotation)).gameObject;
				pathPlane.transform.localScale = new Vector3(0.1f, 1.0f, (Vector3.Distance(nextPos, prevPos) / 10.0f));
				
				circleGO = new GameObject();
				circle = circleGO.AddComponent<Circle>();
				circle.SetRadius(0.5f);
				circleGO.transform.position = new Vector3(prevPos.x, floorYPos, prevPos.z);
				
				prevPos = nextPos;
			}
			
			circleGO = new GameObject();
			circle = circleGO.AddComponent<Circle>();
			circle.SetRadius(0.5f);
			circleGO.transform.position = new Vector3(prevPos.x, 0.001f, prevPos.z);
		}
		
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
	
	public int minRequiredDominos()
	{
		int nDominos = 0;
		LevelPropertiesScript properties = LevelPropertiesScript.sharedInstance();
		ArrayList placesToGo = new ArrayList(properties.powerups.Count);
		foreach (GameObject powerup in properties.powerups)
		{
			placesToGo.Add(powerup.transform.position);
		}
		
		Vector3 nextPos = MominoScript.sharedInstance().gameObject.transform.position;
		while (placesToGo.Count > 0)
		{
			float minDistance = float.MaxValue;
			Vector3 closestPosition = nextPos;
			foreach (Vector3 position in placesToGo)
			{
				float dist = Vector3.SqrMagnitude(position - nextPos);
				if (dist < minDistance)
				{
					closestPosition = position;
					minDistance = dist;
				}
			}
			
			nextPos = closestPosition;
			placesToGo.Remove(closestPosition);
			nDominos += (int)(Mathf.Sqrt(minDistance) / CreateDominos.dominosSeparation);
		}
		
		return nDominos;
		
	}
	
	public void reset()
	{
		GameObject[] pathPlanes = GameObject.FindGameObjectsWithTag("PathPlane");
		foreach (GameObject pathPlane in pathPlanes)
		{
			Destroy(pathPlane);
		}
		
		this.calculateCheckpoints();
		this.instantiatePath();
	}
}

using UnityEngine;
using System.Collections;

public class ShortestPathScript : MonoBehaviour
{
	private ArrayList checkpoints;
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
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			this.calculateCheckpoints();
			this.instantiatePath();
		}
	}
	
	void calculateCheckpoints()
	{
		LevelPropertiesScript properties = LevelPropertiesScript.sharedInstance();
		this.checkpoints = new ArrayList((properties.powerups.Count + 1));
		this.checkpoints.Add(properties.player.transform.position);
		
		ArrayList placesToGo = new ArrayList(properties.powerups.Count);
		foreach (GameObject powerup in properties.powerups)
		{
			placesToGo.Add(powerup.transform.position);
		}
		
		Vector3 nextPos = properties.player.transform.position;
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
	
	void instantiatePath()
	{
		float floorYPos = LevelPropertiesScript.sharedInstance().floor.transform.position.y;
		if (this.checkpoints.Count > 0)
		{
			Vector3 prevPos = (Vector3)this.checkpoints[0];
			for (int i=1; i<this.checkpoints.Count; i++)
			{
				Vector3 nextPos = (Vector3)this.checkpoints[i];
				Vector3 pos = new Vector3((nextPos.x+prevPos.x)/2.0f, floorYPos+0.01f, (nextPos.z+prevPos.z)/2.0f);
				GameObject pathPlane = ((Transform)Instantiate(this.pathPrefab, pos, Quaternion.LookRotation(nextPos-prevPos))).gameObject;
				pathPlane.transform.localScale = new Vector3(0.1f, 1.0f, (Vector3.Distance(nextPos, prevPos)/10.0f));
				prevPos = nextPos;
			}
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
		
		Vector3 nextPos = properties.player.transform.position;
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

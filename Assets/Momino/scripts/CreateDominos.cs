using UnityEngine;
using System.Collections;

public class CreateDominos : MonoBehaviour
{
	private static CreateDominos singleton;
	public GameObject momino;
	public Rigidbody dominoPrefab;
	public static float dominosSeparation = 1.0f;
	private Vector3 lastDominoPosition;
	public Camera followDominosCollisionsCamera;
	public GameObject lastDomino;
	public bool dominosRunning = false;
	private ArrayList allDominos;
	
	void Awake()
	{
		CreateDominos.singleton = this;
	}
	
	void OnDestroy()
	{
		CreateDominos.singleton = null;
	}
	
	// Use this for initialization
	void Start()
	{
		if (this.dominoPrefab != null)
		{
			this.lastDominoPosition = this.transform.position;
			this.lastDominoPosition.y = (this.transform.position.y - this.transform.localScale.y * 0.5f + (this.dominoPrefab.transform.localScale.y * 0.5f));
		}
		this.lastDomino = null;
		this.allDominos = new ArrayList();
	}
	
	public static CreateDominos sharedInstance()
	{
		return CreateDominos.singleton;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.paused || LevelPropertiesScript.sharedInstance().wasPaused())
		{
			return;
		}
		
		if (Input.GetButtonDown("Jump"))
		{
			LevelPropertiesScript.sharedInstance().nDominosCombo = 0;
			LevelPropertiesScript levelProps = this.GetComponent<LevelPropertiesScript>();
			if (this.dominosRunning)
			{
				this.resetDominosPositions();
				levelProps.setMainCamera();
			} else
			{
				this.pushDominos();
				levelProps.setFollowCamera();
			}
			this.dominosRunning = !this.dominosRunning;
		}
	}
	
	void resetDominosPositions()
	{
		if (this.allDominos.Count > 0)
		{
			foreach (GameObject domino in this.allDominos)
			{
				if (this.dominosRunning)
				{
					domino.rigidbody.isKinematic = true;
					Domino dominoAttributes = domino.GetComponent<Domino>();
					dominoAttributes.hasCollided = false;
					domino.transform.position = dominoAttributes.originalPosition;
					domino.transform.rotation = dominoAttributes.originalRotation;
				} else
				{
					domino.rigidbody.isKinematic = false;
				}
			}
			LevelPropertiesScript.sharedInstance().dominosFalling = false;
		}
	}
	
	void pushDominos()
	{
		if (this.allDominos.Count > 0)
		{
			foreach (GameObject domino in this.allDominos)
			{
				domino.rigidbody.isKinematic = false;
			}
			GameObject firstDomino = (GameObject)this.allDominos[0];
			Domino dominoAttributes = firstDomino.GetComponent<Domino>();
			dominoAttributes.hasCollided = true;
			LevelPropertiesScript.sharedInstance().nDominosCombo = 1;
			firstDomino.rigidbody.AddForce(firstDomino.transform.forward * 100);
			LevelPropertiesScript.sharedInstance().dominosFalling = true;
		}
	}
	
	void OnGUI()
	{
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			if (!LevelPropertiesScript.sharedInstance().dominosFalling && (LevelPropertiesScript.sharedInstance().nPowerupsGot >= LevelPropertiesScript.sharedInstance().nPowerups))
			{
				if (GUI.Button(new Rect((Screen.width - 250), 50, 150, 50), "Action"))
				{
					LevelPropertiesScript.sharedInstance().nDominosCombo = 0;
					if (this.dominosRunning)
					{
						this.resetDominosPositions();
						LevelPropertiesScript.sharedInstance().setMainCamera();
					} else
					{
						this.pushDominos();
						LevelPropertiesScript.sharedInstance().setFollowCamera();
					}
					this.dominosRunning = !this.dominosRunning;
				}
			}
		}
	}
	
	public bool positionIsOnGUI(Vector3 screenPos)
	{
		bool found = false;
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			if (LevelPropertiesScript.sharedInstance().nPowerupsGot >= LevelPropertiesScript.sharedInstance().nPowerups)
			{
				found = (new Rect((Screen.width - 250), 50, 150, 50)).Contains(screenPos);
			}
		}
		
		return found;
	}
	
	public Vector3 worldCoordinatesFromScreenCoordinates(Vector3 screenCoordinates, Vector3 floorPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenCoordinates);
		
		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
		GameObject collidingStep = LevelPropertiesScript.sharedInstance().rayCastWithStairSteps(ray, out position);
		
		if (collidingStep == null)
		{
			Plane plane = new Plane(Vector3.up, floorPosition);
			float distance;
			if (plane.Raycast(ray, out distance))
			{
				position = ray.GetPoint(distance);
				return position;
			}
		}
		
		return position;
	}
	
	public GameObject instantiateDominoIfFarEnough(ref Vector3 position, Quaternion rotation)
	{
		Vector3 diffVector = (position - this.lastDominoPosition);
		float distance = diffVector.sqrMagnitude;
		if (distance >= (CreateDominos.dominosSeparation * CreateDominos.dominosSeparation))
		{
			this.lastDominoPosition = position;
			GameObject previousDomino = this.lastDomino;
			GameObject domino = this.instantiateDomino(ref position, rotation);
			
			if (previousDomino != null)
			{
				Domino lastDominoAttributes = previousDomino.GetComponent<Domino>();
				Quaternion lastRotation = lastDominoAttributes.originalRotation;
				lastRotation = Quaternion.Lerp(lastRotation, rotation, 0.5f);
								
				previousDomino.transform.rotation = lastRotation;
				lastDominoAttributes.originalRotation = lastRotation;
				lastDominoAttributes.originalForwardVector = previousDomino.transform.forward;
			}
			
			return domino;
		}
		
		return null;
	}
	
	public GameObject instantiateDomino(ref Vector3 position, Quaternion rotation)
	{
		int nMaxDominos = LevelPropertiesScript.sharedInstance().maxNDominos;
		if ((nMaxDominos >= 0) && (LevelPropertiesScript.sharedInstance().nDominos >= nMaxDominos))
		{
			return null;
		}
//		if (!this.positionIsValidToInstantiateDomino(position))
//		{
//			return null;
//		}
		
		position.y += this.dominoPrefab.transform.localScale.y * 0.5f;
		
		Rigidbody domino = (Rigidbody)Instantiate(this.dominoPrefab, position, rotation);
		domino.isKinematic = true;
		
		Domino dominoAttributes = domino.GetComponent<Domino>();
		dominoAttributes.setColor(LevelPropertiesScript.sharedInstance().currentColor());
		dominoAttributes.originalPosition = position;
		dominoAttributes.originalRotation = rotation;
		dominoAttributes.originalForwardVector = domino.transform.forward;
		if (this.followDominosCollisionsCamera != null)
		{
			dominoAttributes.followCamera = this.followDominosCollisionsCamera;
		}
		
		
		this.allDominos.Add(domino.gameObject);
		this.lastDomino = domino.gameObject;
		LevelPropertiesScript.sharedInstance().nDominos++;
		return domino.gameObject;
	}
	
	public void reset()
	{
		foreach (GameObject domino in this.allDominos)
		{
			Destroy(domino);
		}
		this.allDominos = new ArrayList();
		LevelPropertiesScript.sharedInstance().nDominosCombo = 0;
		LevelPropertiesScript.sharedInstance().nDominos = 0;
	}
	
	/**
	 * Returns true if position is valid to instantiate a domino. This means that the respective bounding box would not collide with any pre-existing domino.
	 */
//	private bool positionIsValidToInstantiateDomino(Vector3 position)
//	{
//		Collider[] hitColliders = Physics.OverlapSphere(position, this.dominoPrefab.transform.localScale.x * 0.5f);
//		
//		bool found = false;
//		int i = 0;
//		while (!found && i<hitColliders.Length)
//		{
//			Collider collider = hitColliders[i];
//			found = (collider.gameObject.tag == "Domino");
//			i++;
//		}
//		
//		return (!found);
//	}
	
	public static GameObject rayCastWithDominos(Ray ray, out Vector3 hitPosition)
	{
		hitPosition = new Vector3(0, 0, 0);
		GameObject collidingDomino = null;
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject.tag == "Domino")
			{
				collidingDomino = hit.collider.gameObject;
				hitPosition = hit.point;
			}
		}
		
		return collidingDomino;
	}
	
	public void addDomino(GameObject domino)
	{
		this.allDominos.Add(domino);
	}
	
	public void removeDomino(GameObject domino)
	{
		this.allDominos.Remove(domino);
		Destroy(domino);
		LevelPropertiesScript.sharedInstance().nDominos--;
	}
}

using UnityEngine;
using System.Collections;

public class CreateDominos : MonoBehaviour
{
	private static CreateDominos singleton;
	public GameObject momino;
	public Rigidbody dominoPrefab;
	public static float dominosSeparation = 1.0f;
	private Vector3 lastDominoPosition;
	private System.Random rnd;
	public Camera followDominosCollisionsCamera;
	public GameObject floor;
	private Vector3 mouseLastDominoPosition;
	private Quaternion mouseLastDominoAngle;
	private int mouseNDominosCurrentMotion;
	private GameObject mouseLastDomino;
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
			this.mouseLastDominoPosition = this.transform.position;
			this.mouseLastDominoPosition.y = (this.dominoPrefab.transform.localScale.y * 0.5f);
			this.mouseLastDominoAngle = this.transform.rotation;
		}
		this.lastDomino = null;
		this.rnd = new System.Random();
		this.allDominos = new ArrayList();
	}
	
	public static CreateDominos sharedInstance()
	{
		return CreateDominos.singleton;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.paused)
		{
			return;
		}
		
		if (GameProperties.gameType == GameType.kGameTypeGod)
		{
			this.godUpdate();
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
	
	void godUpdate()
	{
		if (GameProperties.editMode != EditMode.kEditModeDominos)
		{
			return;
		}
		
		if (Input.GetMouseButton(0) || (Input.GetMouseButtonUp(0)))
		{
			Vector3 screenCoordinates = Input.mousePosition;
			
			if (this.positionIsOnGUI(screenCoordinates) || LevelPropertiesScript.sharedInstance().positionIsOnGUI(screenCoordinates) || EditModeScript.sharedInstance().positionIsOnGUI(screenCoordinates))
			{
				return;
			}
			
			Vector3 position = CreateDominos.worldCoordinatesFromScreenCoordinates(screenCoordinates, this.floor.transform.position);
			position.y = (this.dominoPrefab.transform.localScale.y * 0.5f);
					
			if (Input.GetMouseButtonDown(0))
			{
				this.mouseLastDominoPosition = position;
				this.mouseNDominosCurrentMotion = 0;
				this.mouseLastDomino = null;
			}
					
			Vector3 diffVector = (position - this.mouseLastDominoPosition);
			float distanceWithLastDomino = diffVector.magnitude;
			if (distanceWithLastDomino >= CreateDominos.dominosSeparation)
			{
						
				Vector3 nextPosition = this.mouseLastDominoPosition;
				Vector3 moveVector = (diffVector.normalized * CreateDominos.dominosSeparation);
						
				this.mouseLastDominoAngle = Quaternion.LookRotation(diffVector);
				this.mouseLastDominoAngle.x = 0.0f;
				this.mouseLastDominoAngle.z = 0.0f;
				int nDominos = (int)(distanceWithLastDomino / CreateDominos.dominosSeparation);
				Quaternion rotation = this.mouseLastDominoAngle;
				for (int i=0; i<nDominos; i++)
				{
					if (i == 0)
					{
						if (this.mouseLastDomino != null)
						{
							Domino mouseLastDominoAttributes = this.mouseLastDomino.GetComponent<Domino>();
							rotation = mouseLastDominoAttributes.originalRotation;
							if (this.mouseNDominosCurrentMotion == 1)
							{
								rotation = this.mouseLastDominoAngle;
							} else
							{
								rotation = Quaternion.Lerp(rotation, this.mouseLastDominoAngle, 0.5f);
							}
								
							this.mouseLastDomino.transform.rotation = rotation;
							mouseLastDominoAttributes.originalRotation = rotation;
						}
					}
							
							
					nextPosition += moveVector;
							
					float f = (0.5f + (0.5f * i / nDominos));
					Quaternion nextRotation = Quaternion.Lerp(rotation, this.mouseLastDominoAngle, f);
					GameObject domino = this.instantiateDomino(ref nextPosition, nextRotation);
					
					if (domino != null)
					{
						this.mouseNDominosCurrentMotion++;
						this.mouseLastDomino = domino;
					}
				}
					
				this.mouseLastDominoPosition = nextPosition;
			}
					
			if (Input.GetMouseButtonUp(0))
			{
				if (this.mouseNDominosCurrentMotion == 0)
				{
					Ray ray = Camera.main.ScreenPointToRay(screenCoordinates);
					Vector3 pos;
					GameObject selectedDomino = CreateDominos.rayCastWithDominos(ray, out pos);
					if (selectedDomino != null)
					{
						this.allDominos.Remove(selectedDomino);
						Destroy(selectedDomino);
						LevelPropertiesScript.sharedInstance().nDominos--;
					} else
					{
						this.instantiateDomino(ref position, this.mouseLastDominoAngle);
					}
				}
					
				this.mouseNDominosCurrentMotion = 0;
				this.mouseLastDomino = null;
			}
		}
	}
	
	void OnGUI()
	{
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			if (LevelPropertiesScript.sharedInstance().nPowerupsGot >= LevelPropertiesScript.sharedInstance().nPowerups)
			{
				if (GUI.Button(new Rect((Screen.width - 250), 20, 150, 50), "Action"))
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
		if (GameProperties.gameType == GameType.kGameTypeMomino)
		{
			if (LevelPropertiesScript.sharedInstance().nPowerupsGot >= LevelPropertiesScript.sharedInstance().nPowerups)
			{
				found = (new Rect((Screen.width - 250), 20, 150, 50)).Contains(screenPos);
			}
		}
		
		return found;
	}
	
	public static Vector3 worldCoordinatesFromScreenCoordinates(Vector3 screenCoordinates, Vector3 floorPosition)
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
		float distance = diffVector.magnitude;
		if (distance >= CreateDominos.dominosSeparation)
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
		if (LevelPropertiesScript.sharedInstance().nDominos >= LevelPropertiesScript.sharedInstance().maxNDominos)
		{
			return null;
		}
		if (!this.positionIsValidToInstantiateDomino(position))
		{
			return null;
		}
		
		GameObject stairsStep = LevelPropertiesScript.sharedInstance().stairsStepAtPosition(position);
		if (stairsStep != null)
		{
			float stepTopPos = (stairsStep.transform.position.y + stairsStep.transform.localScale.y * 0.5f);
			float floorPos;
				
			if (this.mouseLastDomino != null)
			{
				floorPos = (this.mouseLastDomino.transform.position.y - this.mouseLastDomino.transform.localScale.y * 0.5f);
			} else if (MominoScript.sharedInstance() != null)
			{
				floorPos = MominoScript.sharedInstance().transform.position.y - MominoScript.sharedInstance().transform.localScale.y * 0.5f;
			} else
			{
				floorPos = (this.floor.transform.position.y + this.dominoPrefab.transform.localScale.y * 0.5f);
			}
				
			float separation = (stepTopPos - floorPos);
			if (separation > 0.5f)
			{
//				position.y = (this.floor.transform.position.y + this.dominoPrefab.transform.localScale.y * 0.5f);
				position.y = (floorPos + this.dominoPrefab.transform.localScale.y * 0.5f);
			} else
			{
				position.y = ((stairsStep.transform.position.y + stairsStep.transform.localScale.y * 0.5f) + this.dominoPrefab.transform.localScale.y * 0.5f);
			}
			
		} else
		{
			position.y = (this.floor.transform.position.y + this.dominoPrefab.transform.localScale.y * 0.5f);
		}
		
		Rigidbody domino = (Rigidbody)Instantiate(this.dominoPrefab, position, rotation);
		domino.isKinematic = true;
		float r = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
		float g = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
		float b = ((float)(this.rnd.Next(0, int.MaxValue)) / (int.MaxValue));
		float a = 1.0f;
		domino.renderer.material.color = new Color(r, g, b, a);
		
		Domino dominoAttributes = domino.GetComponent<Domino>();
		dominoAttributes.originalPosition = position;
		dominoAttributes.originalRotation = rotation;
		dominoAttributes.originalForwardVector = domino.transform.forward;
		if (this.followDominosCollisionsCamera != null)
		{
			dominoAttributes.followCamera = this.followDominosCollisionsCamera;
		}
		
		
		if (this.floor != null)
		{
			Vector3 floorScale = this.floor.transform.localScale;
			if (position.x < ((floor.transform.position.x - floorScale.x * 0.5f) * 10.0f))
			{
				floorScale.x = (((this.floor.transform.position.x - position.x) * 2.0f) / 10.0f) + 1.0f;
			}
			if (position.x > ((floor.transform.position.x + floorScale.x * 0.5f) * 10.0f))
			{
				floorScale.x = (((position.x - this.floor.transform.position.x) * 2.0f) / 10.0f) + 1.0f;
			}
			if (position.z < ((this.floor.transform.position.z - floorScale.z * 0.5f) * 10.0f))
			{
				floorScale.z = (((this.floor.transform.position.z - position.z) * 2.0f) / 10.0f) + 1.0f;
			}
			if (position.z > ((this.floor.transform.position.z + floorScale.z * 0.5f) * 10.0f))
			{
				floorScale.z = (((position.z - this.floor.transform.position.z) * 2.0f) / 10.0f) + 1.0f;
			}
			this.floor.transform.localScale = floorScale;
		}
		
		
		this.allDominos.Add(domino.gameObject);
		this.lastDomino = domino.gameObject;
		LevelPropertiesScript.sharedInstance().nDominos++;
		return domino.gameObject;
	}
	
	public void deleteDominos()
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
	private bool positionIsValidToInstantiateDomino(Vector3 position)
	{
		Collider[] hitColliders = Physics.OverlapSphere(position, this.dominoPrefab.transform.localScale.x * 0.5f);
		
		bool found = false;
		int i = 0;
		while (!found && i<hitColliders.Length)
		{
			Collider collider = hitColliders[i];
			found = (collider.gameObject.tag == "Domino");
			i++;
		}
		
		return (!found);
	}
	
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
}

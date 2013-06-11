using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovementDirection
{
	kMovementStopped,
	kMovementForwards,
	kMovementBackwards,
}

public class MominoScript : MonoBehaviour
{
	private static MominoScript singleton;
	public float movementSpeed = 6;
	public float turningSpeed = 100;
	public MovementDirection movingDirection = MovementDirection.kMovementStopped;
	public bool shoot = true;
	public float maxClimbingHeight = 0.5f;
	private HashSet<GameObject> allCollidedFloors;
	private GameObject currFloor;
	
	public Transform joystick;
	private Joystick moveJoystick;
	
	void Awake()
	{
		MominoScript.singleton = this;
	}
	
	void OnDestroy()
	{
		MominoScript.singleton = null;
	}
	
	public static MominoScript sharedInstance()
	{
		return MominoScript.singleton;
	}
	
	// Use this for initialization
	void Start()
	{
		this.currFloor = LevelPropertiesScript.sharedInstance().floor;
		this.allCollidedFloors = new HashSet<GameObject>();
		
		if (GameProperties.IsTactil())
		{
			this.moveJoystick = this.joystick.GetComponent<Joystick>();
		} else
		{
			Destroy(this.joystick.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		this.updatePosition();
		this.updateFloorSize();
		this.updateDominosInstances();
	}
	
	void updatePosition()
	{
		
		float axisHorizontal;
		float axisVertical;
		if (GameProperties.IsTactil())
		{
			Vector2 joystickInput = this.moveJoystick.position;
			axisHorizontal = (Mathf.Abs(joystickInput.x) > 0.3f ? joystickInput.x*0.9f : 0.0f);
			axisVertical = (Mathf.Abs(joystickInput.y) > 0.3f ? joystickInput.y*0.65f : 0.0f);
		} else
		{
			axisHorizontal = Input.GetAxis("Horizontal");
			axisVertical = Input.GetAxis("Vertical");
		}
		
		if (axisVertical > 0.001f && Time.deltaTime > 0.0f)
		{
			this.movingDirection = MovementDirection.kMovementForwards;
		} else if (axisVertical < -0.001f && Time.deltaTime > 0.0f)
		{
			this.movingDirection = MovementDirection.kMovementBackwards;
		} else
		{
			this.movingDirection = MovementDirection.kMovementStopped;
		}
		
		float horizontal = axisHorizontal * turningSpeed * Time.deltaTime;
		this.transform.Rotate(0, horizontal, 0);
		float vertical = axisVertical * movementSpeed * Time.deltaTime;
		this.transform.Translate(0, 0, vertical);
		
		
		if (GameProperties.editMode != EditMode.kEditModePrefabs)
		{
			float currY = this.transform.position.y;
			float finalY = (this.currFloor.transform.position.y + this.currFloor.transform.localScale.y * 0.5f + this.transform.localScale.y * 0.5f);
				
			
			float nextY;
			if (finalY > currY)
			{
				nextY = finalY;
			} else
			{
				nextY = Mathf.Lerp(currY, finalY, 0.15f);
			}
				
			Vector3 position = this.transform.position;
			position.y = nextY;
			this.transform.position = position;
		}
		
	}
	
	void updateFloorSize()
	{
		GameObject floor = LevelPropertiesScript.sharedInstance().floor;
		Vector3 position = this.transform.position;
		Vector3 floorScale = floor.transform.localScale;
		
		float offset = 75;
		float ratio = 1.0f;
		
		if ((position.x - offset) < ((floor.transform.position.x - floorScale.x * 0.5f) * ratio))
		{
			floorScale.x = (((floor.transform.position.x - (position.x - offset)) * 2.0f) / ratio) + 1.0f;
		}
		if ((position.x + offset) > ((floor.transform.position.x + floorScale.x * 0.5f) * ratio))
		{
			floorScale.x = ((((position.x + offset) - floor.transform.position.x) * 2.0f) / ratio) + 1.0f;
		}
		if ((position.z - offset) < ((floor.transform.position.z - floorScale.z * 0.5f) * ratio))
		{
			floorScale.z = (((floor.transform.position.z - (position.z - offset)) * 2.0f) / ratio) + 1.0f;
		}
		if ((position.z + offset) > ((floor.transform.position.z + floorScale.z * 0.5f) * ratio))
		{
			floorScale.z = ((((position.z + offset) - floor.transform.position.z) * 2.0f) / ratio) + 1.0f;
		}
		floor.transform.localScale = floorScale;
	}
	
	void updateDominosInstances()
	{
		if (Input.GetButtonDown("Stop"))
		{
			shoot = (!shoot);
		}
		
		if (shoot)
		{
			if (this.movingDirection == MovementDirection.kMovementForwards)
			{
				Vector3 position = this.transform.position - this.transform.forward * 0.65f;
				position.y -= this.transform.localScale.y * 0.5f;
				CreateDominos.sharedInstance().instantiateDominoIfFarEnough(ref position, this.transform.rotation);
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		GameObject go = other.gameObject;
		if (go.tag == "Floor" || go.tag == "StairStep")
		{
			int nFloors = this.allCollidedFloors.Count;
			this.allCollidedFloors.Add(go);
			if (nFloors < this.allCollidedFloors.Count)
			{
				this.updateCurrentFloor();
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		GameObject go = other.gameObject;
		if (go.tag == "Floor" || go.tag == "StairStep")
		{
			this.allCollidedFloors.Remove(go);
			if (go == this.currFloor)
			{
				this.updateCurrentFloor();
			}
		}
	}
	
	public void updateCurrentFloor()
	{
		if (this.allCollidedFloors.Count > 0)
		{
			GameObject topFloor = null;
			float topY = float.MinValue;
			foreach (GameObject floor in this.allCollidedFloors)
			{
				float currY = floor.transform.position.y + floor.transform.localScale.y * 0.5f;
				if (currY > topY)
				{
					topFloor = floor;
					topY = currY;
				}
				this.currFloor = topFloor;
			}
		} else
		{
			this.currFloor = LevelPropertiesScript.sharedInstance().floor;
		}
	}
	
	public void reset()
	{
		this.allCollidedFloors = new HashSet<GameObject>();
		this.currFloor = LevelPropertiesScript.sharedInstance().floor;
		
		if (GameProperties.gameType == GameType.kGameTypeMominoTargets)
		{
			Vector3 firstPowerupPos = (Vector3)ShortestPathScript.sharedInstance().checkpoints[0];
			Vector3 secondPowerupPos = (Vector3)ShortestPathScript.sharedInstance().checkpoints[1];
			Vector3 firstDirection = (secondPowerupPos - firstPowerupPos);
			
			this.transform.position = (firstPowerupPos - firstDirection.normalized);
			this.transform.rotation = Quaternion.LookRotation(firstDirection);
		} else
		{
			this.transform.position = new Vector3(0, this.transform.localScale.y * 0.5f, 0);
		this.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
}

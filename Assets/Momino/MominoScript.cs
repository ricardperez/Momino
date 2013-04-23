using UnityEngine;
using System.Collections;

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
	public GameObject floor;
	private bool shoot = true;
	public float maxClimbingHeight = 0.5f;
	
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
		float axisHorizontal = Input.GetAxis("Horizontal");
		float axisVertical = Input.GetAxis("Vertical");
		
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
		
		Vector3 position = this.transform.position;
		GameObject collidingStep = LevelPropertiesScript.sharedInstance().stairsStepAtPosition(position);
		if (collidingStep == null)
		{
			position.y = (this.floor.transform.position.y + this.transform.localScale.y * 0.5f);
		} else
		{
			float stepTopPos = (collidingStep.transform.position.y + collidingStep.transform.localScale.y * 0.5f);
			float mominoFloorPos = (this.transform.position.y - this.transform.localScale.y * 0.5f);
			
			float separation = (stepTopPos - mominoFloorPos);
			if (separation > this.maxClimbingHeight)
			{
				position.y = (this.floor.transform.position.y + this.transform.localScale.y * 0.5f);
			} else
			{
				position.y = (collidingStep.transform.position.y + collidingStep.transform.localScale.y*0.5f + this.transform.localScale.y * 0.5f);
			}
		}
		this.transform.position = position;
	}
	
	void updateFloorSize()
	{
		Vector3 position = this.transform.position;
		Vector3 floorScale = this.floor.transform.localScale;
		
		float offset = 75;
		
		if ((position.x - offset) < ((this.floor.transform.position.x - floorScale.x * 0.5f) * 10.0f))
		{
			floorScale.x = (((this.floor.transform.position.x - (position.x - offset)) * 2.0f) / 10.0f) + 1.0f;
		}
		if ((position.x + offset) > ((this.floor.transform.position.x + floorScale.x * 0.5f) * 10.0f))
		{
			floorScale.x = ((((position.x + offset) - this.floor.transform.position.x) * 2.0f) / 10.0f) + 1.0f;
		}
		if ((position.z - offset) < ((this.floor.transform.position.z - floorScale.z * 0.5f) * 10.0f))
		{
			floorScale.z = (((this.floor.transform.position.z - (position.z - offset)) * 2.0f) / 10.0f) + 1.0f;
		}
		if ((position.z + offset) > ((this.floor.transform.position.z + floorScale.z * 0.5f) * 10.0f))
		{
			floorScale.z = ((((position.z + offset) - this.floor.transform.position.z) * 2.0f) / 10.0f) + 1.0f;
		}
		this.floor.transform.localScale = floorScale;
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
				CreateDominos.sharedInstance().instantiateDominoIfFarEnough(ref position, this.transform.rotation);
			}
		}
	}
}

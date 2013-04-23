using UnityEngine;
using System.Collections;

public class CameraOrbitScript : MonoBehaviour
{
	
	public float movementSpeed = 5.0f;
	public float rotationXSpeed = 50.0f;
	public float rotationYSpeed = 50.0f;
	private float x = 0.0f;
	private float y = 0.0f;
	
	// Use this for initialization
	void Start()
	{
		if (GameProperties.gameType == GameType.kGameTypeGod)
		{
			SmoothFollow follow = this.GetComponent<SmoothFollow>();
			follow.target = null;
			
			this.transform.position = new Vector3(10.0f, 5.0f, 2.0f);
			this.transform.rotation = Quaternion.LookRotation(-this.transform.position);
			
			Vector3 euler = this.transform.eulerAngles;
			this.x = euler.y;
			this.y = euler.x;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.gameType == GameType.kGameTypeGod)
		{
			this.updatePosition();
			this.updateRotation();
		}
	}
	
	void updatePosition()
	{
		float moveZ = Input.GetAxis("Vertical");
		float moveX = Input.GetAxis("Horizontal");
		float moveY = Input.GetAxis("AxisY");
		
		float translateX = moveX * movementSpeed * Time.deltaTime;
		float translateY = moveY * movementSpeed * Time.deltaTime;
		float translateZ = moveZ * movementSpeed * Time.deltaTime;
		
		this.transform.Translate(translateX, translateY, translateZ);
	}
	
	void updateRotation()
	{
		if (Input.GetMouseButton(1))
		{
			this.x += Input.GetAxis("Mouse X") * this.rotationXSpeed * 0.02f;
			this.y -= Input.GetAxis("Mouse Y") * this.rotationYSpeed * 0.02f;
			this.transform.rotation = Quaternion.Euler(y, x, 0);
		}
	}
}

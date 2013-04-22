using UnityEngine;
using System.Collections;

public class CollisionFollowCameraScript : MonoBehaviour {
	
	public Quaternion rotationTo;
	public Vector3 positionTo;
	public float rotationSpeed = 1.0f;
	public float movementSpeed = 1.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		this.gameObject.transform.position = Vector3.Slerp(this.camera.transform.position, this.positionTo, Time.deltaTime * this.movementSpeed);
		this.gameObject.transform.rotation = Quaternion.Slerp(this.camera.transform.rotation, this.rotationTo, Time.deltaTime * this.rotationSpeed);
	}
}

using UnityEngine;
using System.Collections;

public class DominoColliderScript : MonoBehaviour
{
	
	private GameObject parent;
	private GameObject currFloor;
	
	// Use this for initialization
	void Start()
	{
		this.parent = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (this.parent.rigidbody.isKinematic)
		{
			if (this.currFloor == null)
			{
				Vector3 pos = this.parent.transform.position;
				pos.y += Vector3.down.y * Time.deltaTime;
				this.parent.transform.position = pos;
			}
			
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Floor" || other.tag == "StairStep")
		{
			if (this.parent.rigidbody.isKinematic)
			{
				this.currFloor = other.gameObject;
			
				Vector3 pos = this.parent.transform.position;
				pos.y = this.currFloor.transform.position.y + this.currFloor.transform.localScale.y * 0.5f + this.parent.transform.localScale.y * 0.5f;
				this.parent.transform.position = pos;
			}
		}
	}
}

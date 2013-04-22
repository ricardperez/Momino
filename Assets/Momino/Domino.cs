using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Domino : MonoBehaviour
{
	public Vector3 originalPosition;
	public Quaternion originalRotation;
	public Vector3 originalForwardVector;
	public Camera followCamera;
	public float cameraSpeed;
	public float thresholdVelocity = 2.1f;
	public AudioClip audioFile;
	public Transform collisionParticleSystem;
	public bool hasCollided;
	private GameObject explosionParticleSystem;

	// Use this for initialization
	void Start()
	{
		this.cameraSpeed = 1.0f;
		this.hasCollided = false;
		
		if (this.followCamera != null)
		{
			CollisionFollowCameraScript followCameraScript = this.followCamera.GetComponent<CollisionFollowCameraScript>();
			Vector3 position = this.followCameraPosition();
			followCameraScript.positionTo = position;
			followCameraScript.rotationTo = Quaternion.LookRotation(this.transform.position - position);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		float collisionMagnitude = collision.relativeVelocity.magnitude;
		if (collisionMagnitude > thresholdVelocity)
		{
			Domino otherDomino = null;
			ContactPoint [] contactPoints = collision.contacts;
			bool foundOther = false;
			int i = 0;
			while (!foundOther && i<contactPoints.Length)
			{
				ContactPoint contactPoint = contactPoints[i];
				GameObject firstGameObject = contactPoint.thisCollider.gameObject;
				GameObject secondGameObject = contactPoint.otherCollider.gameObject;
				foundOther = ((firstGameObject == this.gameObject) && (secondGameObject.tag == "Domino"));
				if (foundOther)
				{
					otherDomino = secondGameObject.GetComponent<Domino>();
				}
				i++;
			}
			
			if (foundOther)
			{
				if (!otherDomino.hasCollided)
				{
					otherDomino.hasCollided = true;
					LevelPropertiesScript.sharedInstance().nDominosCombo += 1;
					
					audio.clip = audioFile;
					float volume = Mathf.InverseLerp(thresholdVelocity, thresholdVelocity * 4, collisionMagnitude);
					audio.volume = volume;
					audio.Play();
				
					if (this.followCamera != null)
					{
						CollisionFollowCameraScript followCameraScript = this.followCamera.GetComponent<CollisionFollowCameraScript>();
						Vector3 position = this.followCameraPosition();
						followCameraScript.positionTo = position;
						followCameraScript.rotationTo = Quaternion.LookRotation(contactPoints[0].point - position);
					}
					
					LevelPropertiesScript.sharedInstance().updateFallingPosition(contactPoints[0].point);
					
					Transform explosionTransform = (Transform)Instantiate(this.collisionParticleSystem, contactPoints[0].point, this.transform.rotation);
					this.explosionParticleSystem = explosionTransform.gameObject;
					ParticleSystem ps = this.explosionParticleSystem.particleSystem;
					
					ps.startColor = this.gameObject.renderer.material.color;
					
				}
			}
		}
	}
	
	void OnDestroy()
	{
		Destroy(this.explosionParticleSystem);
	}
	
	Vector3 followCameraPosition()
	{
		return (this.originalPosition + (this.originalForwardVector * 8.0f) + (Vector3.up * this.transform.localScale.y * 2.5f) + Vector3.left * 10.0f);
	}
}

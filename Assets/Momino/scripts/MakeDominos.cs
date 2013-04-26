using UnityEngine;
using System.Collections;

public class MakeDominos : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	public void saveStateAsOriginal()
	{
		foreach (Transform dominoTransf in this.transform)
		{
			Domino domino = dominoTransf.gameObject.GetComponent<Domino>();
			domino.originalRotation = dominoTransf.rotation;
			domino.originalPosition = dominoTransf.position;
			domino.originalForwardVector = dominoTransf.forward;
			dominoTransf.gameObject.rigidbody.isKinematic = true;
			domino.hasCollided = false;
			CreateDominos.sharedInstance().addDomino(dominoTransf.gameObject);
		}
	}
	
	public void applyCurrentColor()
	{
		foreach (Transform dominoTransf in this.transform)
		{
			Domino domino = dominoTransf.gameObject.GetComponent<Domino>();
			domino.setColor(LevelPropertiesScript.sharedInstance().currentColor());
		}
	}
}

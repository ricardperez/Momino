using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeStairs : MonoBehaviour
{
	public int nSteps = 8;
	public GameObject stepPrefab;
	public HashSet<GameObject> allSteps;
	private static MakeStairs singleton;
	
	void Awake()
	{
		MakeStairs.singleton = this;
	}
	
	void OnDestroy()
	{
		MakeStairs.singleton = null;
	}
	
	public static MakeStairs sharedInstance()
	{
		return MakeStairs.singleton;
	}
	
	// Use this for initialization
	void Start()
	{
		if (this.allSteps == null)
		{
			this.allSteps = new HashSet<GameObject>();
		}
		
		Vector3 currPosition = this.transform.position;
		bool assignedFirstY = false;
				
		float scaleYIncr = this.stepPrefab.transform.localScale.y;
		float currScaleY = (this.stepPrefab.transform.localScale.y + scaleYIncr);
		
		int i = 0;
		foreach (Transform stepTransf in this.transform)
		{
			GameObject step = stepTransf.gameObject;
			if (!assignedFirstY)
			{
				currPosition.y = (step.transform.localScale.y * 0.5f);
			}
			this.addStep(step, currScaleY, currPosition);
			currPosition += (step.transform.forward * step.transform.localScale.z);
					
			if ((i + 1) == this.nSteps / 2)
			{
				if ((this.nSteps % 2) == 1)
				{
					currScaleY += scaleYIncr;
				}
			} else
			{
				if ((i + 1) < (this.nSteps / 2))
				{
					currScaleY += scaleYIncr;
				} else
				{
					currScaleY -= scaleYIncr;
				}
			}
			i++;
		}
		
		if (this.stepPrefab != null)
		{
			if (!assignedFirstY)
			{
				currPosition.y = (this.stepPrefab.transform.localScale.y * 0.5f);
			}
			for (; i<this.nSteps; i++)
			{
				GameObject step = (GameObject)Instantiate(this.stepPrefab, currPosition, this.transform.rotation);
				this.addStep(step, currScaleY, currPosition);
				currPosition += (step.transform.forward * step.transform.localScale.z);
					
				if ((i + 1) == this.nSteps / 2)
				{
					if ((this.nSteps % 2) == 1)
					{
						currScaleY += scaleYIncr;
					}
				} else
				{
					if ((i + 1) < (this.nSteps / 2))
					{
						currScaleY += scaleYIncr;
					} else
					{
						currScaleY -= scaleYIncr;
					}
				}
			}
		}
	}
	
	void addStep(GameObject step, float scaleY, Vector3 position)
	{
		this.allSteps.Add(step);
		
		Vector3 scale = step.transform.localScale;
		scale.y = scaleY;
		step.transform.localScale = scale;
		
		position.y = (step.transform.localScale.y * 0.5f);
		step.transform.localPosition = position;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
	
	public GameObject stairsStepAtPosition(Vector3 position)
	{
		GameObject collidingStep = null;
		if (this.allSteps != null)
		{
			foreach (GameObject nextStep in this.allSteps)
			{
				Vector3 stepPosition = nextStep.transform.position;
				Vector3 stepSize = nextStep.transform.localScale;
			
				if ((position.x > (stepPosition.x - stepSize.x * 0.5f)) && (position.x < (stepPosition.x + stepSize.x * 0.5f)) && (position.z > (stepPosition.z - stepSize.z * 0.5f)) && (position.z < (stepPosition.z + stepSize.z * 0.5f)))
				{
					collidingStep = nextStep;
					break;
				}
			}
		}
		return collidingStep;
	}
	
	public GameObject rayCastWithStairSteps(Ray ray, out Vector3 hitPosition)
	{
		hitPosition = new Vector3(0, 0, 0);
		GameObject collidingStep = null;
		
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject.tag == "StairStep")
			{
				collidingStep = hit.collider.gameObject;
				hitPosition = hit.point;
			}
		}
		return collidingStep;
	}
}

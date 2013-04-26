using UnityEngine;
using System.Collections;

public class MakeStairs : MonoBehaviour
{
	public bool needsBuild = true;
	public int nSteps = 8;
	public GameObject stepPrefab;
	private ArrayList steps;
	
	// Use this for initialization
	void Start()
	{
		Vector3 currPosition = this.transform.position;
		bool assignedFirstY = false;
				
		float scaleYIncr = this.stepPrefab.transform.localScale.y;
		float currScaleY = (this.stepPrefab.transform.localScale.y + scaleYIncr);
		
		this.steps = new ArrayList(this.nSteps);
		
		int i = 0;
		foreach (Transform stepTransf in this.transform)
		{
			if (stepTransf.gameObject.tag == "StairStep")
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
		}
		
		if (this.needsBuild)
		{
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
	}
	
	void OnDestroy()
	{
		if ((this.steps != null) && (LevelPropertiesScript.sharedInstance() != null))
		{
			foreach (GameObject step in this.steps)
			{
				LevelPropertiesScript.sharedInstance().removeStep(step);
			}
		}
	}
	
	void addStep(GameObject step, float scaleY, Vector3 position)
	{
		if (this.needsBuild)
		{
			Vector3 scale = step.transform.localScale;
			scale.y = scaleY;
			step.transform.localScale = scale;
		
			position.y = (step.transform.localScale.y * 0.5f);
			step.transform.localPosition = position;
		}
		
		this.steps.Add(step);
		LevelPropertiesScript.sharedInstance().addStep(step);
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}

using UnityEngine;
using System.Collections;

public class GodScript : MonoBehaviour
{
	private Vector3 mouseLastDominoPosition;
	private Quaternion mouseLastDominoAngle;
	private int mouseNDominosCurrentMotion;
	private GameObject mouseLastDomino;
	private static GodScript singleton;
	public float maxDistanceFromCenter = 50.0f;
	
	public static GodScript sharedInstance()
	{
		return singleton;
	}
	
	void Awake()
	{
		GodScript.singleton = this;
	}
	
	void OnDestroy()
	{
		GodScript.singleton = null;
	}
	
	// Use this for initialization
	void Start()
	{
		GameObject floor = LevelPropertiesScript.sharedInstance().floor;
		this.mouseLastDominoPosition = new Vector3(0, (floor.transform.position.y + floor.transform.localScale.y*0.5f), 0);
		this.mouseLastDominoAngle = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (GameProperties.gameType == GameType.kGameTypeGod)
		{
			if ((GameProperties.editMode != EditMode.kEditModeDominos) || GameProperties.paused || LevelPropertiesScript.sharedInstance().wasPaused())
			{
				return;
			}
		
			if (Input.GetMouseButton(0) || (Input.GetMouseButtonUp(0)))
			{
				Vector3 screenCoordinates = Input.mousePosition;
			
				if (CreateDominos.sharedInstance().positionIsOnGUI(screenCoordinates) || LevelPropertiesScript.sharedInstance().positionIsOnGUI(screenCoordinates) || EditModeScript.sharedInstance().positionIsOnGUI(screenCoordinates))
				{
					return;
				}
			
				GameObject floor = LevelPropertiesScript.sharedInstance().floor;
				Vector3 floorPosition = floor.transform.position;
				floorPosition.y = (floor.transform.position.y + floor.transform.localScale.y*0.5f);
				
				Vector3 position = CreateDominos.sharedInstance().worldCoordinatesFromScreenCoordinates(screenCoordinates, floorPosition);
				
				if (!this.positionIsValid(position))
				{
					return;
				}
					
				if (Input.GetMouseButtonDown(0))
				{
					this.mouseLastDominoPosition = position;
					this.mouseNDominosCurrentMotion = 0;
					this.mouseLastDomino = null;
				}
					
				Vector3 diffVector = (position - this.mouseLastDominoPosition);
				diffVector.y = 0.0f;
				float distanceWithLastDominoSqr = diffVector.sqrMagnitude;
				if (distanceWithLastDominoSqr >= (CreateDominos.dominosSeparation * CreateDominos.dominosSeparation))
				{
					float distanceWithLastDomino = Mathf.Sqrt(distanceWithLastDominoSqr);
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
						Vector3 p = new Vector3(nextPosition.x, nextPosition.y, nextPosition.z);
							
						float f = (0.5f + (0.5f * i / nDominos));
						Quaternion nextRotation = Quaternion.Lerp(rotation, this.mouseLastDominoAngle, f);
						GameObject domino = CreateDominos.sharedInstance().instantiateDomino(ref nextPosition, nextRotation);
						
						nextPosition.x = p.x;
						nextPosition.y = p.y;
						nextPosition.z = p.z;
					
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
							CreateDominos.sharedInstance().removeDomino(selectedDomino);
						} else
						{
							CreateDominos.sharedInstance().instantiateDomino(ref position, this.mouseLastDominoAngle);
						}
					}
					
					this.mouseNDominosCurrentMotion = 0;
					this.mouseLastDomino = null;
				}
			}
		}
	}
	
	bool positionIsValid(Vector3 position)
	{
		GameObject floor = LevelPropertiesScript.sharedInstance().floor;
		Vector3 vToCenter = (position - floor.transform.position);
		float distSqr = (vToCenter.x*vToCenter.x + vToCenter.z*vToCenter.z);
		return (distSqr < (this.maxDistanceFromCenter * this.maxDistanceFromCenter));
	}
}

using UnityEngine;
using System.Collections;

public enum PowerupState
{
	kPowerupStateDefault,
	kPowerupStateWaiting,
	kPowerupStateCorrect
}

public class PowerupScript : MonoBehaviour
{
	public AudioClip audioClip;
	public Color haloColor = new Color(0.0f, 1.0f, 0.118f);
	public float haloMaxSize = 2.0f;
	public float haloMinSize = 1.0f;
	public float haloSpeed = 1.0f;
	public Transform explosionParticleSystemPrefab;
	private float haloCurrSize;
	private bool haloIncreasing;
	private PowerupState state;
	private Light powerupLight;
	private GameObject powerupSphere;
	private ParticleSystem explosionParticleSystem;

	// Use this for initialization
	void Start()
	{
		this.powerupLight = this.transform.FindChild("Light").gameObject.light;
		this.powerupSphere = this.transform.FindChild("Sphere").gameObject;
		
		this.setState(PowerupState.kPowerupStateDefault);
		this.powerupLight.intensity = 2.0f;
		this.powerupLight.range = 2.0f;
		this.powerupLight.transform.position = this.transform.position;
		this.haloCurrSize = this.haloMinSize;
		this.haloIncreasing = true;
		this.applyHaloCurrSize();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		this.updateState();
		if (this.state == PowerupState.kPowerupStateCorrect)
		{
			if (this.explosionParticleSystem == null)
			{
				this.explode();
			}
		} else
		{
			this.updateSize();
		}
	}
	
	void updateSize()
	{
		float deltaSize = haloSpeed * Time.deltaTime;
		if (!this.haloIncreasing)
		{
			deltaSize = -deltaSize;
		}
		
		this.haloCurrSize += deltaSize;
		this.applyHaloCurrSize();
			
		if (this.haloCurrSize >= this.haloMaxSize)
		{
			this.haloIncreasing = false;
		} else if (this.haloCurrSize <= this.haloMinSize)
		{
			this.haloIncreasing = true;
		}
	}
	
	void updateState()
	{
		if (this.state == PowerupState.kPowerupStateDefault)
		{
			MominoScript momino = MominoScript.sharedInstance();
			GameObject lastDomino = ((momino == null) ? null : CreateDominos.sharedInstance().lastDomino);
			if (lastDomino != null)
			{
				if (Vector3.SqrMagnitude(this.transform.position - lastDomino.transform.position) < (this.haloMinSize * this.haloMinSize))
				{
					this.setState(PowerupState.kPowerupStateWaiting);
				}
			}
		} else if ((this.state == PowerupState.kPowerupStateWaiting) && LevelPropertiesScript.sharedInstance().dominosFalling)
		{
			if (Vector3.SqrMagnitude(this.transform.position - LevelPropertiesScript.sharedInstance().dominosFallingPosition) < (this.haloMinSize * this.haloMinSize))
			{
				this.setState(PowerupState.kPowerupStateCorrect);
			}
		}
	}
	
	void explode()
	{
		;
		this.explosionParticleSystem = ((Transform)Instantiate(this.explosionParticleSystemPrefab, this.transform.position, Quaternion.Euler(-90, 0, 0))).gameObject.particleSystem;
		Destroy(this.gameObject);
	}
	
	void applyHaloCurrSize()
	{
		if (this.powerupLight != null)
		{
			this.powerupLight.range = this.haloCurrSize;
		}
		
	}
	
	public void setState(PowerupState theState)
	{
		Color color;
		switch (theState)
		{
		case PowerupState.kPowerupStateCorrect:
			LevelPropertiesScript.sharedInstance().nPowerupsExplode++;
			color = Color.green;
			break;
		case PowerupState.kPowerupStateWaiting:
			LevelPropertiesScript.sharedInstance().nPowerupsGot++;
			color = Color.yellow;
			audio.clip = this.audioClip;
			float volume = 2.0f;
			audio.volume = volume;
			audio.Play();
			break;
		case PowerupState.kPowerupStateDefault:
		default:
			color = Color.blue;
			break;
		}
		
		this.powerupLight.color = color;
		this.powerupSphere.renderer.material.color = new Color(color.r, color.g, color.b, 0.5f);
		
		this.state = theState;
	}
	
	public PowerupState getState()
	{
		return this.state;
	}
}

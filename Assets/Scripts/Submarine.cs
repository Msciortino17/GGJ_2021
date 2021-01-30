using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Submarine : MonoBehaviour
{
	private Rigidbody myRigidBody;
	private Inventory myInventory;

	public float Health;
	public float HealthRegen;
	private float maxHealth;
	public float lowHealthThreshold;
	public AudioSource alertAudio;
	public HUD hudReference;

	private bool gameFinished;
	private float gameOverTimer;

	[Header("Abilities")]
	public int SonarPingArtifactNumber;
	public Transform SonarPingTransform;
	private Vector3 sonarPingStartSize;
	public float SonarPingSize;
	private Vector3 sonarPingFinalSize;
	public float SonarPingDuration;
	private float sonarPingTimer;
	public AudioSource sonarAudio;

	public int TorpedoArtifactNumber;
	public GameObject TorpedoPrefab;
	public float TorpedoCooldownTime;
	private float torpedoCooldownTimer;
	public AudioSource torpedoAudio;

	public int DeepWaterArtifactNumber;
	public float DeepWaterDepth;
	public float DeepWaterDamage;

	[Header("Movement")]
	public float maxSpeed = 5;
	public float maxPitchSpeed = 3;
	public float maxTurnSpeed = 50;
	public float acceleration = 2;
	public float smoothSpeed = 3;
	public float smoothTurnSpeed = 3;
	public Transform propeller;
	public Transform rudderPitch;
	public Transform rudderYaw;
	public float propellerSpeedFac = 2;
	public float rudderAngle = 30;
	public AudioSource engineAudio;

	Vector3 velocity;
	float yawVelocity;
	float pitchVelocity;
	float currentSpeed;
	public Material propSpinMat;

	void Start()
	{
		maxHealth = Health;
		currentSpeed = maxSpeed;
		myRigidBody = GetComponent<Rigidbody>();
		myInventory = GetComponent<Inventory>();

		sonarPingStartSize = SonarPingTransform.localScale;
		sonarPingFinalSize = new Vector3(SonarPingSize, SonarPingSize, SonarPingSize);
	}

	void Update()
	{
		if (!gameFinished)
		{
			UpdateMovement();
			UpdateSonarPing();
			UpdateShootTorpedoes();
			UpdateTooDeep();
			UpdateHealth();
		}
		UpdateGameOver();

		// temp testing
		if (Input.GetKeyDown(KeyCode.J))
		{
			TakeDamage(10f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Castle") && !myInventory.HasArtifact(3))
		{
			hudReference.AddDialogue("This is the gate to Atlantis! It seems the door will only open for an Atlantian...", 8f);
		}
		else if (other.GetComponent<FinalGate>() != null)
		{
			gameFinished = true;
			gameOverTimer = 65f;

			hudReference.FadeBlack(1.1f, 1f);
			hudReference.AddDialogue("", 2f);
			hudReference.AddDialogue("Ah, so this is what happened to Atlantis. How sad...", 8f);
			hudReference.AddDialogue("They perfected their technology and became an ideal civilization.", 8f);
			hudReference.AddDialogue("However, fate kept them bound to the sea and they would never set foot on land.", 8f);
			hudReference.AddDialogue("With no more problems to solve or goals to reach for, a horrible despair cursed the Atlantians.", 8f);
			hudReference.AddDialogue("They all chose to end their lives rather than live empty ones.", 8f);
			hudReference.AddDialogue("And now they are but relics to us surface dwellers.", 8f);
			hudReference.AddDialogue("I can only pray that humanity is spared a similar fate...", 8f);
			hudReference.AddDialogue("The End. Thanks for playing!", 8f);
		}
	}

	/// <summary>
	/// Reads input from the player for controlling the direction and speed of the submarine.
	/// </summary>
	private void UpdateMovement()
	{
		float accelDir = 0;
		if (Input.GetKey(KeyCode.Q))
		{
			accelDir -= 1;
		}
		if (Input.GetKey(KeyCode.E))
		{
			accelDir += 1;
		}

		currentSpeed += acceleration * Time.deltaTime * accelDir;
		currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
		float speedPercent = currentSpeed / maxSpeed;

		Vector3 targetVelocity = transform.forward * currentSpeed;
		velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * smoothSpeed);

		float targetPitchVelocity = Input.GetAxisRaw("Vertical") * maxPitchSpeed;
		pitchVelocity = Mathf.Lerp(pitchVelocity, targetPitchVelocity, Time.deltaTime * smoothTurnSpeed);

		float targetYawVelocity = Input.GetAxisRaw("Horizontal") * maxTurnSpeed;
		yawVelocity = Mathf.Lerp(yawVelocity, targetYawVelocity, Time.deltaTime * smoothTurnSpeed);
		transform.localEulerAngles += (Vector3.up * yawVelocity + Vector3.left * pitchVelocity) * Time.deltaTime * speedPercent;
		transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

		rudderYaw.localEulerAngles = Vector3.up * yawVelocity / maxTurnSpeed * rudderAngle;
		rudderPitch.localEulerAngles = Vector3.left * pitchVelocity / maxPitchSpeed * rudderAngle;

		propeller.Rotate(Vector3.forward * Time.deltaTime * propellerSpeedFac * speedPercent, Space.Self);
		propSpinMat.color = new Color(propSpinMat.color.r, propSpinMat.color.g, propSpinMat.color.b, speedPercent * .3f);
	}

	/// <summary>
	/// Checks for input to activate the ping, and handles logic for expanding it.
	/// </summary>
	private void UpdateSonarPing()
	{
		//if (!myInventory.HasArtifact(SonarPingArtifactNumber))
		//{
		//	return;
		//}

		if (sonarPingTimer > 0f)
		{
			SonarPingTransform.gameObject.SetActive(true);

			float ratio = sonarPingTimer / SonarPingDuration;
			ratio = 1f - ratio;
			SonarPingTransform.localScale = Vector3.Slerp(sonarPingStartSize, sonarPingFinalSize, ratio);

			sonarPingTimer -= Time.deltaTime;
		}
		else
		{
			SonarPingTransform.gameObject.SetActive(false);

			if (Input.GetKeyDown(KeyCode.C))
			{
				sonarPingTimer = SonarPingDuration;
				sonarAudio.Play();
			}
		}
	}

	/// <summary>
	/// Checks for input to spawn torpedoes and handles instanstiation and setup, along with cooldown.
	/// </summary>
	private void UpdateShootTorpedoes()
	{
		//if (!myInventory.HasArtifact(TorpedoArtifactNumber))
		//{
		//	return;
		//}

		if (Input.GetKeyDown(KeyCode.Space) && torpedoCooldownTimer <= 0f)
		{
			Transform torpedo = Instantiate(TorpedoPrefab).transform;
			torpedo.position = transform.position;
			torpedo.rotation = transform.rotation;
			torpedoCooldownTimer = TorpedoCooldownTime;
			torpedoAudio.pitch = Random.Range(0.9f, 1.1f);
			torpedoAudio.Play();
		}

		if (torpedoCooldownTimer > 0f)
		{
			torpedoCooldownTimer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Checks Y position to see if you're too deep, then applies damage, unless you have the artifact.
	/// </summary>
	private void UpdateTooDeep()
	{
		//if (myInventory.HasArtifact(DeepWaterArtifactNumber))
		//{
		//	return;
		//}

		if (transform.position.y < DeepWaterDepth)
		{
			TakeDamage( DeepWaterDamage * Time.deltaTime );

			if (hudReference.DialogueEmpty())
			{
				hudReference.AddDialogue("WARNING: Water pressure too great at this depth.", 2f);
			}
		}
	}

	/// <summary>
	/// Passively regenerates health, and checks for death.
	/// </summary>
	private void UpdateHealth()
	{
		Health += HealthRegen * Time.deltaTime;
		if (Health > maxHealth)
		{
			Health = maxHealth;
		}

		if (Health <= 0f)
		{
			gameFinished = true;
			gameOverTimer = 8f;

			hudReference.FadeBlack(1.1f, 1f);
			hudReference.AddDialogue("", 1f);
			hudReference.AddDialogue("We're taking on water, we're finished... It seems the sea floor will be our tomb.", 6f);
		}
	}

	/// <summary>
	/// If the game is finished, begin counting down. Once all the way, load the main menu scene.
	/// </summary>
	private void UpdateGameOver()
	{
		if (gameFinished)
		{
			gameOverTimer -= Time.deltaTime;
			if (gameOverTimer < 0f)
			{
				SceneManager.LoadScene("MainMenu");
			}
		}
	}

	/// <summary>
	/// Simple ratio of current health to max health.
	/// </summary>
	public float GetHealthRatio()
	{
		return Health / maxHealth;
	}

	/// <summary>
	/// Deals Damage to the player for a certain amount.
	/// </summary>
	public void TakeDamage(float damage)
	{
		Health -= damage;
		if(GetHealthRatio() < lowHealthThreshold)
        {
			alertAudio.Play();
        }
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
	private Rigidbody myRigidBody;
	private Inventory myInventory;

	[Header("Abilities")]
	public int SonarPingArtifactNumber;
	public Transform SonarPingTransform;
	private Vector3 sonarPingStartSize;
	public float SonarPingSize;
	private Vector3 sonarPingFinalSize;
	public float SonarPingDuration;
	private float sonarPingTimer;

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

	Vector3 velocity;
	float yawVelocity;
	float pitchVelocity;
	float currentSpeed;
	public Material propSpinMat;

	void Start()
	{
		currentSpeed = maxSpeed;
		myRigidBody = GetComponent<Rigidbody>();
		myInventory = GetComponent<Inventory>();

		sonarPingStartSize = SonarPingTransform.localScale;
		sonarPingFinalSize = new Vector3(SonarPingSize, SonarPingSize, SonarPingSize);
	}

	void Update()
	{
		UpdateControls();
		UpdateSonarPing();
	}

	private void UpdateControls()
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

			if (Input.GetKeyDown(KeyCode.Space))
			{
				sonarPingTimer = SonarPingDuration;
			}
		}
	}
}
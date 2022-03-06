using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example skid script. Put this on a WheelCollider.
// Copyright 2017 Nition, BSD licence (see LICENCE file). http://nition.co
[RequireComponent(typeof(WheelCollider))]
public class WheelSkid : MonoBehaviour {

	// INSPECTOR SETTINGS

	[SerializeField]
	Rigidbody rb;

	[HideInInspector]
	public Skidmarks skidmarksController;

	private CarMovement car;

	const float SKID_FX_SPEED = 0.5f; // Min side slip speed in m/s to start showing a skid
	const float MAX_SKID_INTENSITY = 20.0f; // m/s where skid opacity is at full intensity
	const float WHEEL_SLIP_MULTIPLIER = 10.0f * 5; // For wheelspin. Adjust how much skids show
	int lastSkid = -1; // Array index for the skidmarks controller. Index of last skidmark piece this wheel used
	private float timer = 0;
	float lastFixedUpdateTime;

	// #### UNITY INTERNAL METHODS ####

	protected void Awake() {
		lastFixedUpdateTime = Time.time;

		car = rb.GetComponent<CarMovement>();

		if (skidmarksController == null)
		{
			skidmarksController = FindObjectOfType<Skidmarks>();
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 0.1f)
		{
			lastSkid = -1;
		}
	}

	/*protected void FixedUpdate() {
		lastFixedUpdateTime = Time.time;
	}

	protected void LateUpdate() {
		if (wheelCollider.GetGroundHit(out wheelHitInfo))
		{
			// Check sideways speed

			// Gives velocity with +z being the car's forward axis
			Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			float skidTotal = Mathf.Abs(localVelocity.x);

			// Check wheel spin as well

			float wheelAngularVelocity = wheelCollider.radius * ((2 * Mathf.PI * wheelCollider.rpm) / 60);
			float carForwardVel = Vector3.Dot(rb.velocity, transform.forward);
			float wheelSpin = Mathf.Abs(carForwardVel - wheelAngularVelocity) * WHEEL_SLIP_MULTIPLIER;

			// NOTE: This extra line should not be needed and you can take it out if you have decent wheel physics
			// The built-in Unity demo car is actually skidding its wheels the ENTIRE time you're accelerating,
			// so this fades out the wheelspin-based skid as speed increases to make it look almost OK
			wheelSpin = Mathf.Max(0, wheelSpin * (10 - Mathf.Abs(carForwardVel)));

			skidTotal += wheelSpin;

			// Skid if we should
			if (skidTotal >= SKID_FX_SPEED) {
				float intensity = Mathf.Clamp01(skidTotal / MAX_SKID_INTENSITY);
				// Account for further movement since the last FixedUpdate
				Vector3 skidPoint = wheelHitInfo.point + (rb.velocity * (Time.time - lastFixedUpdateTime));
				lastSkid = skidmarksController.AddSkidMark(skidPoint, wheelHitInfo.normal, intensity, lastSkid);
			}
			else {
				lastSkid = -1;
			}
		}
		else {
			lastSkid = -1;
		}
	}*/

	// #### PUBLIC METHODS ####

	public void AddSkidMark()
	{
		if (Physics.Raycast(transform.position, -rb.transform.up, out RaycastHit hit, car.wheelRadius * 1.1f))
		{
			lastSkid = skidmarksController.AddSkidMark(hit.point + rb.velocity * Time.deltaTime, hit.normal, 1, lastSkid);
			timer = 0;
		}
	}

	// #### PROTECTED/PRIVATE METHODS ####


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ShakeEvent : UnityEvent<float> { }

// https://stackoverflow.com/questions/31389598/how-can-i-detect-a-shake-motion-on-a-mobile-device-using-unity3d-c-sharp/31389776
public class ShakeDetection : MonoBehaviour
{
	[Header( "Variables" )]
	public float lowPassKernelWidthInSeconds = 1.0f;
	public float shakeDetectionThreshold = 2.0f;
	public float accelerometerUpdateInterval = 1.0f / 60.0f;
	public bool MustBeGrabbed = true;

	[Header( "Events" )]
	public InputEvent OnShake;

	float lowPassFilterFactor;
	Vector3 lowPassValue;

	private KinematicVelocity Kinematic;

	void Start()
	{
		Kinematic = GetComponent<KinematicVelocity>();

		lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
		//shakeDetectionThreshold *= shakeDetectionThreshold;
		lowPassValue = Vector3.zero;
	}

	void Update()
	{
		Vector3 acceleration = Kinematic.Acceleration + Kinematic.AngularAcceleration;
		if ( !float.IsNaN( acceleration.x ) )
		{
			lowPassValue = Vector3.Lerp( lowPassValue, acceleration, lowPassFilterFactor );
			Vector3 deltaAcceleration = acceleration - lowPassValue;

			if ( deltaAcceleration.magnitude >= shakeDetectionThreshold )
			{
				// Perform your "shaking actions" here. If necessary, add suitable
				// guards in the if check above to avoid redundant handling during
				// the same shake (e.g. a minimum refractory period).
				OnShake.Invoke( deltaAcceleration.magnitude.ToString() );
			}
		}
	}
}

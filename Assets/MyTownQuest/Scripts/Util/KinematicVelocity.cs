using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

// Basically just auto applies the sources/relatives.
// So not everything needs to exist in the scene on load...
public class KinematicVelocity : VRTK_VelocityEstimator
{
	[HideInInspector]
	public Vector3 Velocity
	{
		get
		{
			return GetVelocityEstimate();
		}
	}
	[HideInInspector]
	public Vector3 AngularVelocity
	{
		get
		{
			return GetAngularVelocityEstimate();
		}
	}
	[HideInInspector]
	public Vector3 Acceleration;
	[HideInInspector]
	public Vector3 AngularAcceleration;

	private float oldTime;
	private Vector3 oldVelocity;
	private Vector3 oldAngularVelocity;

	// https://forum.unity.com/threads/manually-calculating-acceleration-and-velocity.443537/
	private void Update()
	{
		float newTime = Time.time;
		{
			// Acceleration
			Vector3 newVelocity = Velocity;
			Acceleration = ( ( newVelocity - oldVelocity ) / ( newTime - oldTime ) );
			oldVelocity = newVelocity;

			// Angular Acceleration
			Vector3 newAngularVelocity = AngularVelocity;
			AngularAcceleration = ( ( newAngularVelocity - oldAngularVelocity ) / ( newTime - oldTime ) );
			oldAngularVelocity = newAngularVelocity;
		}
		oldTime = newTime;
	}
}

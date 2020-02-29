using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Tracking.Velocity;

// Basically just auto applies the sources/relatives.
// So not everything needs to exist in the scene on load...
public class KinematicVelocity : AverageVelocityEstimator
{
	[HideInInspector]
	public Vector3 Velocity
	{
		get
		{
			return DoGetVelocity();
		}
	}
	[HideInInspector]
	public Vector3 AngularVelocity
	{
		get
		{
			return DoGetAngularVelocity();
		}
	}
	[HideInInspector]
	public Vector3 Acceleration;
	[HideInInspector]
	public Vector3 AngularAcceleration;

	private float oldTime;
	private Vector3 oldVelocity;
	private Vector3 oldAngularVelocity;

	private void Start()
	{
		Source = gameObject;
		RelativeTo = transform.parent.gameObject;
	}

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

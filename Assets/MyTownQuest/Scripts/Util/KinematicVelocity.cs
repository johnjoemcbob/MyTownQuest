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

	private void Start()
	{
		Source = gameObject;
		RelativeTo = transform.parent.gameObject;
	}
}

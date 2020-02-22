using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Matthew 27/08/19
// There is a built in BillboardRenderer in Unity but I think that just faces the camera, this faces only on yaw axis
// Also using flat cube mesh renderers for characters to avoid z fighting with sprites etc
[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
	public float FollowDistance = 2;
	public float FollowSpeed = 5;
	public bool FollowCamera = false;
	public bool YawOnly = true;

    void Update()
	{
		if ( FollowCamera )
		{
			Vector3 target = Camera.main.transform.position + Camera.main.transform.forward * FollowDistance;
			transform.position = Vector3.Lerp( transform.position, target, Time.deltaTime * FollowSpeed );
		}

		// Type 1
		//transform.LookAt( Camera.main.transform );

		// Type 2
		transform.eulerAngles = Camera.main.transform.eulerAngles + new Vector3( 0, 180, 0 );

		// Only rotate yaw
		if ( YawOnly )
		{
			transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
		}
	}
}

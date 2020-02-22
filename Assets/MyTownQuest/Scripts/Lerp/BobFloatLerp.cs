using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobFloatLerp : MonoBehaviour
{
	public float Distance = 0.1f;
	public float Angle = 30;
	public float Speed = 1;

    void Update()
    {
		// Position
		Vector3 target = ( transform.right * Mathf.Sin( Time.time ) * Distance / 2 ) + ( transform.up * Mathf.Sin( Time.time ) * Distance );
		transform.localPosition = Vector3.Lerp( transform.localPosition, target, Time.deltaTime * Speed );

		// Rotation
		target = new Vector3( 180, 180, Mathf.Cos( Time.time ) * Angle );
		transform.localRotation = Quaternion.Lerp( transform.localRotation, Quaternion.Euler( target ), Time.deltaTime * Speed );

		// Scale
		target = Vector3.one * ( 1 + Mathf.Cos( Time.time * 2 ) * Distance / 2 );
		transform.localScale = Vector3.Lerp( transform.localScale, target, Time.deltaTime * Speed );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : MonoBehaviour
{
	[Header( "Variables" )]
	public float DistanceMult = 1;
	public float Angle = 30;
	public float Speed = 5;

	[Header( "References" )]
	public Transform BristlesParent;

	private Vector3 LerpedBy;

	private KinematicVelocity Kinematic;

	private void Start()
	{
		Kinematic = GetComponent<KinematicVelocity>();
	}

	private void Update()
	{
		foreach ( Transform bristle in BristlesParent )
		{
			Vector3 target = Vector3.zero;
			//if ( LerpedBy != Vector3.zero )
			{
				//float dist = ( bristle.position.x - LerpedBy.x ) + ( bristle.position.y - LerpedBy.y ) + ( bristle.position.z - LerpedBy.z );
				float dist = Kinematic.Velocity.magnitude + Kinematic.AngularVelocity.magnitude + Vector3.Distance( bristle.position, transform.position );
				dist *= Vector3.Dot( Kinematic.Velocity, transform.right );
				//Debug.Log( dist );
				//Debug.Log( Vector3.Dot( Kinematic.Velocity, transform.right ) );
				target = new Vector3( 0, 0, Mathf.Clamp( dist * DistanceMult * Angle, -Angle, Angle ) );
			}
			bristle.localRotation = Quaternion.Lerp( bristle.localRotation, Quaternion.Euler( target ), Time.deltaTime * Speed );
		}
	}

	//private void OnTriggerEnter( Collider other )
	//{
	//	LerpedBy = other.ClosestPoint( transform.position );
	//}

	//private void OnTriggerExit( Collider other )
	//{
	//	LerpedBy = Vector3.zero;
	//}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHandler : MonoBehaviour
{
	private Transform BaseParent;
	private HandAnimations Animator;

    void Start()
    {
		BaseParent = transform.parent;
		Animator = GetComponent<HandAnimations>();
	}

	public void OnGrab()
	{
		Animator.SwitchState( "GrabLarge" );

		// TODO move to grabbed object version if exists
	}

	public void OnUnGrab()
	{
		Animator.SwitchState( "Idle" );
	}

	public void OnGripChanged( bool change )
	{
		if ( change )
		{
			Animator.SwitchState( "Fist" );
		}
		else
		{
			Animator.SwitchState( "Idle" );
		}
	}
}

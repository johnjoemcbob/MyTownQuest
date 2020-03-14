using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HandHandler : MonoBehaviour
{
	private Transform BaseParent;
	private HandAnimations Animator;
	private VRTK_InteractGrab Grab;

	private bool IsGrabbing = false;

    void Start()
    {
		BaseParent = transform.parent;
		Animator = GetComponentInChildren<HandAnimations>();

		Grab = GetComponentInParent<VRTK_InteractGrab>();
		Grab.ControllerGrabInteractableObject += OnGrab;
		Grab.ControllerUngrabInteractableObject += OnUnGrab;
		Grab.GrabButtonPressed += OnGrabTry;
		Grab.GrabButtonReleased += OnUnGrabTry;
	}

	private void OnTriggerEnter( Collider other )
	{
		var bucket = other.GetComponentInParent<PaintBucket>();
		if ( bucket != null )
		{
			GetComponentInChildren<SkinnedMeshRenderer>().material.color = bucket.PaintColour;
			bucket.Splash( gameObject );
		}
		//var brush = other.GetComponentInParent<PaintBrush>();
		//if ( brush != null )
		//{
		//	GetComponentInChildren<SkinnedMeshRenderer>().material.color = brush.Colour;
		//}
	}

	public void OnGrab( object sender, ObjectInteractEventArgs e )
	{
		Animator.SwitchState( "GrabLarge" );
		IsGrabbing = true;

		// TODO move to grabbed object version if exists
	}

	public void OnUnGrab( object sender, ObjectInteractEventArgs e )
	{
		Animator.SwitchState( "Idle" );
		IsGrabbing = false;
	}

	public void OnGrabTry( object sender, ControllerInteractionEventArgs e )
	{
		StartCoroutine( DelayedFist() );
	}

	public void OnUnGrabTry( object sender, ControllerInteractionEventArgs e )
	{
		Animator.SwitchState( "Idle" );
		IsGrabbing = false;
	}

	IEnumerator DelayedFist()
	{
		yield return new WaitForEndOfFrame();

		if ( !IsGrabbing )
		{
			Animator.SwitchState( "Fist" );
		}
	}
}

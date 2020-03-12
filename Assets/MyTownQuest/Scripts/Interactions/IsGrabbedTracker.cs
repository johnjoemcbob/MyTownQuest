using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class IsGrabbedTracker : MonoBehaviour
{
	public bool IsGrabbed = false;
	public GameObject LastGrabbedBy;

	private VRTK_InteractableObject Interact;

	private void Start()
	{
		Interact = GetComponent<VRTK_InteractableObject>();
		Interact.InteractableObjectGrabbed += OnGrab;
		Interact.InteractableObjectUngrabbed += OnUnGrab;
	}

	//private void Update()
	//{
	//	IsGrabbed = Interact.GetGrabbingObject() != null;
	//	if ( IsGrabbed )
	//	{
	//		LastGrabbedBy = Interact.GetGrabbingObject();
	//	}
	//}

	public void OnGrab( object sender, InteractableObjectEventArgs e )
	{
		IsGrabbed = true;
		LastGrabbedBy = e.interactingObject;
	}

	public void OnUnGrab( object sender, InteractableObjectEventArgs e )
	{
		IsGrabbed = false;
	}
}

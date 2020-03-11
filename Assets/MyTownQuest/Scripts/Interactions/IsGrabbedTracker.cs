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
	}

	private void Update()
	{
		IsGrabbed = Interact.GetGrabbingObject() != null;
		if ( IsGrabbed )
		{
			LastGrabbedBy = Interact.GetGrabbingObject();
		}
	}

	//public void OnGrab( InteractorFacade grab )
	//{
	//	IsGrabbed = true;
	//	LastGrabbedBy = grab;
	//}

	//public void OnUnGrab( InteractorFacade grab )
	//{
	//	IsGrabbed = false;
	//}
}

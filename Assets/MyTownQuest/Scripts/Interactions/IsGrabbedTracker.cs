using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactors;

public class IsGrabbedTracker : MonoBehaviour
{
	public bool IsGrabbed = false;
	public InteractorFacade LastGrabbedBy;

	public void OnGrab( InteractorFacade grab )
	{
		IsGrabbed = true;
		LastGrabbedBy = grab;
	}

	public void OnUnGrab( InteractorFacade grab )
	{
		IsGrabbed = false;
	}
}

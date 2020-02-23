using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InputEvent : UnityEvent<string> { }

public class InputReceiver : MonoBehaviour
{
	public InputEvent OnUse;

	private IsGrabbedTracker Grabbed;

    void Start()
    {
		Grabbed = GetComponentInParent<IsGrabbedTracker>();
	}

	public void OnInternalUse( string hand )
	{
		if ( Grabbed.LastGrabbedBy != null && Grabbed.LastGrabbedBy.name.Contains( hand ) )
		{
			OnUse.Invoke( hand );
		}
	}
}

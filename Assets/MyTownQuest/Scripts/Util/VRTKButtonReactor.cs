using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK.Examples;
using VRTK.Controllables;

[Serializable]
public class VRTKButtonPressEvent : UnityEvent<ControllableEventArgs> { }

public class VRTKButtonReactor : ControllableReactor
{
	public VRTKButtonPressEvent OnPress;

	private void OnTriggerEnter( Collider other )
	{
		Debug.Log( other.gameObject.name );
		if ( other.transform.parent.name.Contains( "Hand" ) )
		{
			OnPress.Invoke( new ControllableEventArgs() );
		}
	}

	private void OnCollisionEnter( Collision collision )
	{
		Debug.Log( collision.transform.parent.name );
		if ( collision.transform.parent.name.Contains( "Hand" ) )
		{
			OnPress.Invoke( new ControllableEventArgs() );
		}
	}

	//protected override void MaxLimitReached( object sender, ControllableEventArgs e )
	//{
	//	OnPress.Invoke( e );
	//}
}

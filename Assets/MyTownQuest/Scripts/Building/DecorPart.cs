using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorPart : BasePart
{
	public static List<DecorPart> DecorsHeld = new List<DecorPart>();

	public void OnGrab()
	{
		DecorsHeld.Add( this );

		ToggleSnapSpheres();
	}

	public void OnUnGrab()
	{
		DecorsHeld.Remove( this );

		ToggleSnapSpheres();
	}

	public static void ToggleSnapSpheres()
	{
		bool on = ( DecorsHeld.Count > 0 );

		foreach ( var snap in FindObjectsOfType<DecorSnapTag>() )
		{
			if ( snap.GetComponentInParent<BuildingPart>().IsSpawned )
			{
				var mesh = snap.GetComponentInChildren<MeshRenderer>( true );
				mesh.enabled = on;
				mesh.transform.parent.gameObject.SetActive( on );

				// Quick fix for previously incorrect snap points, if this is the first new grab - reset all colours
				if ( DecorsHeld.Count == 1 )
				{
					mesh.material.color = Color.white;
				}
			}
		}
	}
}

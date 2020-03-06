using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorPart : BasePart
{
	public static List<DecorPart> DecorsHeld = new List<DecorPart>();
	public static float MaxDistance = 0.1f;

	private IsGrabbedTracker GrabTrack;

	private DecorSnapLogic SnapTo;
	private DecorSnapLogic LastSnappedTo;

	public override void Start()
	{
		base.Start();

		GrabTrack = GetComponent<IsGrabbedTracker>();

		//Visual = transform.GetChild( 0 ).GetChild( 0 );
	}

	private void Update()
	{
		if ( GrabTrack.IsGrabbed )
		{
			// Find all snap points
			// Find closest
			DecorSnapLogic closest = null;
			float maxdist = -1;
			foreach ( var snap in DecorSnapLogic.Snaps )
			{
				if ( snap != null )
				{
					float dist = Vector3.Distance( transform.position, snap.transform.position );
					if ( dist <= MaxDistance && ( maxdist == -1 || dist < maxdist ) && snap.Part.IsSpawned )
					{
						maxdist = dist;
						closest = snap;
					}
				}
			}
			if ( closest != null )
			{
				// Snap
				Visual.SetParent( closest.Part.GetVisual() );
				Visual.position = closest.transform.position;
				Visual.rotation = closest.transform.rotation;
				//Visual.localScale = Vector3.one;
				SnapTo = closest;
			}
			else
			{
				// Unsnap
				//transform.SetParent( null );
				SnapTo = null;

				// Return visual
				Visual.SetParent( transform.GetChild( 0 ).GetChild( 0 ) );
				Visual.localPosition = Vector3.zero;
				Visual.localEulerAngles = Vector3.zero;
			}
		}
		else
		{
			// Constantly have collider/grabbable follow the visual when not grabbed
			transform.position = Visual.position;
			transform.rotation = Visual.rotation;
			transform.localScale = Vector3.one;
		}
	}

	public void OnGrab()
	{
		DecorsHeld.Add( this );

		ToggleSnapSpheres();
	}

	public void OnUnGrab()
	{
		DecorsHeld.Remove( this );

		if ( SnapTo )
		{
			SnapTo.Part.Foundation.OnSnap( this, SnapTo.Part );
			LastSnappedTo = SnapTo;

			// Return visual
			//Visual.SetParent( transform.GetChild( 0 ) );
			//Visual.localPosition = Vector3.zero;
			//Visual.localEulerAngles = Vector3.zero;

			// Snap real object
			//transform.SetParent( SnapTo.Part.GetVisual() );

			// Don't snap grabbable as this seems to cause issues, a grabbable being a child of another grabbable
			// Just move to same position for later.
			transform.position = Visual.position;
			transform.rotation = Visual.rotation;
			transform.localScale = Vector3.one;
		}
		else if ( LastSnappedTo )
		{
			LastSnappedTo.Part.Foundation.OnUnSnap( this );
			LastSnappedTo = null;
		}

		ToggleSnapSpheres();
	}

	public static void ToggleSnapSpheres()
	{
		bool on = ( DecorsHeld.Count > 0 );

		foreach ( var snap in FindObjectsOfType<DecorSnapLogic>() )
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DecorPart : BasePart
{
	public static List<DecorPart> DecorsHeld = new List<DecorPart>();
	public static float MaxDistance = 0.1f;
	public static float ColliderMult = 3;

	private IsGrabbedTracker GrabTrack;

	private DecorSnapLogic SnapTo;
	private DecorSnapLogic LastSnappedTo;

	public override void Start()
	{
		base.Start();

		GrabTrack = GetComponent<IsGrabbedTracker>();

		var interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += OnGrab;
		interact.InteractableObjectUngrabbed += OnUnGrab;

		//Visual = transform.GetChild( 0 ).GetChild( 0 );

		var box = GetComponentInChildren<BoxCollider>();
		box.isTrigger = true;
		box.size = new Vector3( box.size.x, box.size.y, box.size.z * ColliderMult );
		box.center = new Vector3( box.center.x, box.center.y, box.size.z / 2 );
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
				SnapTo = closest;
			}
			else
			{
				// Unsnap
				SnapTo = null;

				// Return visual
				Visual.SetParent( transform.GetChild( 0 ).GetChild( 0 ) );
				Visual.localPosition = Vector3.zero;
				Visual.localEulerAngles = Vector3.zero;
			}
		}
		//else
		//{
		//	// Constantly have collider/grabbable follow the visual when not grabbed
		//	transform.position = Visual.position;
		//	transform.rotation = Visual.rotation;
		//	transform.localScale = Vector3.one;
		//}
	}

	public void OnGrab( object sender, InteractableObjectEventArgs e )
	{
		DecorsHeld.Add( this );

		ToggleSnapSpheres();
	}

	public void OnUnGrab( object sender, InteractableObjectEventArgs e )
	{
		DecorsHeld.Remove( this );

		if ( SnapTo )
		{
			if ( SnapTo.Part.Foundation != null )
			{
				SnapTo.Part.Foundation.OnSnap( this, SnapTo.Part );
			}
			LastSnappedTo = SnapTo;

			// Snap real object
			transform.SetParent( SnapTo.Part.GetVisual() );

			// Don't snap grabbable as this seems to cause issues, a grabbable being a child of another grabbable
			// Just move to same position for later.
			transform.position = Visual.position;
			transform.rotation = Visual.rotation;
			transform.localScale = Vector3.one;

			// Return visual
			Visual.SetParent( transform.GetChild( 0 ) );
			Visual.localPosition = Vector3.zero;
			Visual.localEulerAngles = Vector3.zero;
		}
		else if ( LastSnappedTo )
		{
			if ( LastSnappedTo.Part.Foundation != null )
			{
				LastSnappedTo.Part.Foundation.OnUnSnap( this );
			}
			LastSnappedTo = null;
		}

		ToggleSnapSpheres();
	}

	public static void ToggleSnapSpheres()
	{
		bool on = ( DecorsHeld.Count > 0 );

		foreach ( var snap in FindObjectsOfType<DecorSnapLogic>() )
		{
			snap.gameObject.SetActive( true );
			if ( snap.GetComponentInParent<BuildingPart>().IsSpawned )
			{
				var mesh = snap.GetComponentInChildren<MeshRenderer>( true );
				mesh.enabled = on;

				// Quick fix for previously incorrect snap points, if this is the first new grab - reset all colours
				if ( DecorsHeld.Count == 1 )
				{
					mesh.material.color = Color.white;
				}
			}
		}
	}
}

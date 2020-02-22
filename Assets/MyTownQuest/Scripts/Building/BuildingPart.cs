using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables.Grab.Action;

public class BuildingPart : MonoBehaviour
{
	[Header( "Variables" )]
	public float DestroyDistance = 0.1f;
	public float SnapDistance = 0.05f;

	[Header( "References" )]
	public Transform SnapPointsParent;

	[HideInInspector]
	public List<BuildingPart> InSnapRangeOf = new List<BuildingPart>();
	[HideInInspector]
	public bool Spawned = false;

	private IsGrabbedTracker Grabbed;
	private GrabInteractableFollowAction Follower;
	private Transform LastClosest = null;

	public void Start()
	{
		SnapPointsParent = transform.FindChildren( "SnapZoneTrigger" )[0];

		Grabbed = GetComponent<IsGrabbedTracker>();
		Follower = GetComponentInChildren<GrabInteractableFollowAction>();

		StartCoroutine( CorrectSettings() );
	}

	private IEnumerator CorrectSettings()
	{
		yield return new WaitForSeconds( 0.5f );

		// Force correct settings since prefab is overwritten somehow
		Follower.WillInheritIsKinematicWhenInactiveFromConsumerRigidbody = false;
		Follower.IsKinematicWhenActive = false;
		Follower.IsKinematicWhenInactive = true;
	}

	public void Update()
	{
		if ( !Spawned ) return;

		if ( Grabbed.IsGrabbed && Vector3.Distance( Grabbed.LastGrabbedBy.transform.position, transform.position ) <= SnapDistance )
		{
			Transform closest = null;

			// Find all possible snap points
			// Add to dictionary with distance
			// Sort by closest first
			SortedList<float, Transform> possible_other = new SortedList<float, Transform>();
			foreach ( var snapable in InSnapRangeOf )
			{
				if ( snapable != null )
				{
					foreach ( Transform child in snapable.SnapPointsParent )
					{
						float dist = Vector3.Distance( Grabbed.LastGrabbedBy.transform.position, child.position );
						if ( dist <= SnapDistance && !possible_other.ContainsKey( dist ) )
						{
							possible_other.Add( dist, child );
						}
					}
				}
			}

			// While there are closest remaining and none matched, loop
			while ( possible_other.Count > 0 )
			{
				closest = possible_other.Values[0];

				// Do same list for self snap points, by closest & valid match, loop
				SortedList<float, Transform> possible_self = new SortedList<float, Transform>();
				foreach ( Transform child in SnapPointsParent )
				{
					var childsnap = child.GetComponent<SnapRequirement>();
					var closesnap = closest.GetComponent<SnapRequirement>();
					if ( childsnap != null && closesnap != null && closest.name.Contains( childsnap.NameContains ) && child.name.Contains( closesnap.NameContains ) )
					{
						float dist = Vector3.Distance( closest.position, child.position );
						if ( !possible_self.ContainsKey( dist ) )
						{
							possible_self.Add( dist, child );
						}
					}
				}

				if ( possible_self.Count > 0 )
				{
					//Debug.Log( "possible: " + closest + " " + possible_self.Values[0] );
					// If valid match then snap and return
					Transform offsetparent = transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );
					//offsetparent.transform.localPosition = -possible_self.Values[0].localPosition;
					//transform.GetChild( 0 ).GetChild( 0 ).transform.localEulerAngles = -closestself.localEulerAngles;

					// Snap
					Transform visual = GetVisual();
					visual.position = closest.position;
					visual.localPosition -= possible_self.Values[0].localPosition + new Vector3( 0, 0.5f, 0 );
					visual.rotation = closest.parent.rotation;

					//Follower.FollowTracking = GrabInteractableFollowAction.TrackingType.FollowRigidbody;

					break;
				}

				// Otherwise loops and eventually exits update
				possible_other.RemoveAt( 0 );
			}

			if ( LastClosest != closest )
			{
				MyTownQuest.SpawnResourceAudioSource( "swoosh2", transform.position, Random.Range( 0.8f, 1.2f ), 0.2f );
			}
			LastClosest = closest;
		}
		else
		{
			//Follower.FollowTracking = GrabInteractableFollowAction.TrackingType.FollowTransform;
		}

		if ( LastClosest == null )
		{
			Transform visual = GetVisual();
			visual.localPosition = Vector3.zero;
			visual.localEulerAngles = Vector3.zero;
		}
	}

	private Transform GetVisual()
	{
		return transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );
	}

	// Another building part entered this one's zone of snapping
	// Tell that one its in range
	private void OnTriggerEnter( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponent<BuildingPart>();
		if ( part != null && part != this && Spawned && part.Spawned && !part.InSnapRangeOf.Contains( this ) )
		{
			part.InSnapRangeOf.Add( this );
		}
	}

	private void OnTriggerExit( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponent<BuildingPart>();
		if ( part != null && part.InSnapRangeOf.Contains( this ) )
		{
			part.InSnapRangeOf.Remove( this );
		}
	}

	public void OnGrab()
	{
		transform.GetComponentInChildren<Collider>().isTrigger = false;
	}

	public void OnUnGrab()
	{
		transform.GetComponentInChildren<Collider>().isTrigger = true;

		// Destroy if placed near any spawner
		foreach ( var spawner in FindObjectsOfType<InfiniteSpawner>() )
		{
			if ( Vector3.Distance( transform.position, spawner.transform.position ) <= DestroyDistance )
			{
				MyTownQuest.SpawnResourceAudioSource( "pop1", transform.position, Random.Range( 0.8f, 1.2f ) );
				Destroy( gameObject );
				return;
			}
		}

		// Reset visual/anchor offsets
		Transform visual = GetVisual();
		transform.position = visual.position + transform.up * 0.05f;
		transform.rotation = visual.rotation;
		visual.localPosition = Vector3.zero;
		visual.localEulerAngles = Vector3.zero;
	}
}

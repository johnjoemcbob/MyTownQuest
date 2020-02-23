using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables.Grab.Action;

public class BuildingPart : MonoBehaviour
{
	public static List<BuildingPart> Parts = new List<BuildingPart>();

	[Header( "Variables" )]
	public float DestroyDistance = 0.1f;
	public float SnapDistance = 0.05f;
	public float BetweenSnaps = 0.1f;

	[Header( "References" )]
	public Transform SnapPointsParent;

	[HideInInspector]
	public List<BuildingPart> InSnapRangeOf = new List<BuildingPart>();
	[HideInInspector]
	public List<Vector3Int> OccupiedCells = new List<Vector3Int>();
	[HideInInspector]
	public bool IsSpawned = false;

	[HideInInspector]
	public IsGrabbedTracker Grabbed;
	[HideInInspector]
	public CollisionShape CollisionShape;

	private GrabInteractableFollowAction Follower;
	private Transform LastClosest = null;
	private BuildingPart LastSnappedTo = null;

	private float NextSnap = 0;

	public void Start()
	{
		// Track this part
		Parts.Add( this );

		Grabbed = GetComponent<IsGrabbedTracker>();
		Follower = GetComponentInChildren<GrabInteractableFollowAction>();
		CollisionShape = GetComponentInChildren<CollisionShape>();

		SnapPointsParent = transform.FindChildren( "SnapZoneTrigger" )[0];

		// Disable spheres until snapable
		foreach ( var renderer in SnapPointsParent.GetComponentsInChildren<MeshRenderer>( true ) )
		{
			renderer.enabled = false;
		}

		// Force correct settings since prefab is overwritten somehow
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

	private void OnDestroy()
	{
		Parts.Remove( this );
	}

	public Transform GetVisual()
	{
		return transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );
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
		transform.position = visual.position + visual.up * 0.05f;
		transform.rotation = visual.rotation;
		visual.localPosition = Vector3.zero;
		visual.localEulerAngles = Vector3.zero;

		// Parent
		if ( LastSnappedTo != null )
		{
			transform.SetParent( LastSnappedTo.transform );
		}
		else
		{
			transform.SetParent( null );
		}

		// Hide all spheres
		foreach ( var part in Parts )
		{
			foreach ( var renderer in part.SnapPointsParent.GetComponentsInChildren<MeshRenderer>( true ) )
			{
				renderer.enabled = false;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables.Grab.Action;
using Zinnia.Action;
using Zinnia.Action.Collection;

public class PlotPartComparer : IComparer<KeyValuePair<BuildingPart, GameObject>>
{
	public int Compare( KeyValuePair<BuildingPart, GameObject> a, KeyValuePair<BuildingPart, GameObject> b )
	{
		int ay = a.Key.SnappedCell.y;
		int by = b.Key.SnappedCell.y;
		return ay == by ? 0 : ( ay < by ? 1 : -1 );
	}
}

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
	public Vector3Int SnappedCell;
	[HideInInspector]
	public Vector3 Snapped;
	[HideInInspector]
	public BuildingFoundation Foundation = null;
	[HideInInspector]
	public float DelayedRemoveFoundation = 0;

	[HideInInspector]
	public IsGrabbedTracker Grabbed;
	[HideInInspector]
	public CollisionShape CollisionShape;

	private GrabInteractableFollowAction Follower;
	private BuildingFoundation LastSnappedTo = null;
	private Transform Visual;

	//private void Awake()
	//{
	//	Debug.Log( "try.. on awake.." );
	//	string[] sides = new string[] { "Left", "Right" };
	//	int index = 0;
	//	foreach ( var side in sides )
	//	{
	//		ActionRegistrar.ActionSource src = new ActionRegistrar.ActionSource();
	//		{
	//			src.Container = GameObject.Find( "AttachmentOrigin" + side );
	//			src.Action = GameObject.Find( "FloatActionUse" + side ).GetComponent<FloatAction>();
	//		}
	//		Debug.Log( src.Action );
	//		GetComponentInChildren<ActionRegistrarSourceObservableList>( true ).Add( src );
	//		Debug.Log( GetComponentInChildren<ActionRegistrar>( true ).Sources.Added );
	//		index++;
	//	}
	//}

	public void Start()
	{
		// Track this part
		Parts.Add( this );

		Grabbed = GetComponent<IsGrabbedTracker>();
		Follower = GetComponentInChildren<GrabInteractableFollowAction>();
		CollisionShape = GetComponentInChildren<CollisionShape>();

		SnapPointsParent = transform.FindChildren( "SnapZoneTrigger" )[0];
		Visual = transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );

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

	private void Update()
	{
		if ( DelayedRemoveFoundation != 0 && DelayedRemoveFoundation <= Time.time && Foundation != null )
		{
			Foundation.DeOccupyGrid( this );
			Foundation = null;
			DelayedRemoveFoundation = 0;
		}
	}

	private void OnDestroy()
	{
		Parts.Remove( this );

		if ( LastSnappedTo != null )
		{
			LastSnappedTo.OnUnSnap( this );
		}
	}

	public Transform GetVisual()
	{
		return Visual;
	}

	public Transform GetVisualParent()
	{
		return transform.GetChild( 0 ).GetChild( 0 );
	}

	public void UseWhileHeld()
	{
		Vector3 rotate = new Vector3( 0, 90, 0 );
		transform.GetChild( 0 ).localEulerAngles += rotate;
		for ( int cell = 0; cell < CollisionShape.Cells.Length; cell++ )
		{
			CollisionShape.Cells[cell] = RotatePointAroundPivot( CollisionShape.Cells[cell], Vector3.one * 0.5f, rotate );
		}
	}

	// From: https://answers.unity.com/questions/532297/rotate-a-vector-around-a-certain-point.html
	private Vector3Int RotatePointAroundPivot( Vector3Int point, Vector3 pivot, Vector3 angle )
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
			dir = Quaternion.Euler( angle ) * dir; // rotate it
		Vector3 rotpoint = dir + pivot; // calculate rotated point
		return new Vector3Int( Mathf.RoundToInt( rotpoint.x ), Mathf.RoundToInt( rotpoint.y ), Mathf.RoundToInt( rotpoint.z ) );
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
		visual.SetParent( GetVisualParent() ); // TODO combine with BuildingFoundation.ResetVisual()
		//transform.position = visual.position + visual.up * 0.05f;
		//transform.rotation = visual.rotation;
		visual.localPosition = Vector3.zero;
		visual.localEulerAngles = Vector3.zero;
		if ( Foundation )
		{
			transform.rotation = Foundation.transform.rotation;
			//transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 ); // what was the purpose of this??
			transform.position = Snapped + Foundation.transform.up * 0.05f;

			LastSnappedTo = Foundation;
			Foundation.OnSnap( this );
		}
		else
		{
			if ( LastSnappedTo != null )
			{
				LastSnappedTo.OnUnSnap( this );
				LastSnappedTo = null;
			}
		}

		// Parent
		//if ( LastSnappedTo != null )
		//{
		//	transform.SetParent( LastSnappedTo.transform );
		//}
		//else
		//{
		//	transform.SetParent( null );
		//}

		// Hide all spheres
		//foreach ( var part in Parts )
		//{
		//	foreach ( var renderer in part.SnapPointsParent.GetComponentsInChildren<MeshRenderer>( true ) )
		//	{
		//		renderer.enabled = false;
		//	}
		//}
	}
	
	public void OnColourChange()
	{
		if ( LastSnappedTo != null )
		{
			LastSnappedTo.LinkedPlot.GetBig( this ).GetComponentInChildren<MeshRenderer>().materials = GetVisual().GetComponentInChildren<MeshRenderer>().materials;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BuildingPart : BasePart
{
	public static List<BuildingPart> Parts = new List<BuildingPart>();

	[Header( "Variables" )]
	public float DestroyDistance = 0.1f;
	public float SnapDistance = 0.05f;
	public float BetweenSnaps = 0.1f;

	[HideInInspector]
	public List<BuildingPart> InSnapRangeOf = new List<BuildingPart>();
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

	private BuildingFoundation LastSnappedTo = null;

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

	public override void Start()
	{
		base.Start();

		// Track this part
		Parts.Add( this );

		Grabbed = GetComponent<IsGrabbedTracker>();
		CollisionShape = GetComponentInChildren<CollisionShape>();

		var interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectGrabbed += OnGrab;
		interact.InteractableObjectUngrabbed += OnUnGrab;
		interact.InteractableObjectUsed += OnUseWhileHeld;

		var box = GetComponentInChildren<BoxCollider>();
		box.isTrigger = true;

		// Disable spheres until snapable
		//foreach ( var renderer in SnapPointsParent.GetComponentsInChildren<MeshRenderer>( true ) )
		//{
		//	renderer.enabled = false;
		//}
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
		Visual.SetParent( GetVisualParent() );

		if ( LastSnappedTo != null )
		{
			LastSnappedTo.OnUnSnap( this );
		}

		Parts.Remove( this );
	}

	// From: https://answers.unity.com/questions/532297/rotate-a-vector-around-a-certain-point.html
	private Vector3Int RotatePointAroundPivot( Vector3Int point, Vector3 pivot, Vector3 angle )
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
			dir = Quaternion.Euler( angle ) * dir; // rotate it
		Vector3 rotpoint = dir + pivot; // calculate rotated point
		return new Vector3Int( Mathf.RoundToInt( rotpoint.x ), Mathf.RoundToInt( rotpoint.y ), Mathf.RoundToInt( rotpoint.z ) );
	}

	public void OnUseWhileHeld( object sender, InteractableObjectEventArgs e )
	{
		Vector3 rotate = new Vector3( 0, 90, 0 );
		transform.GetChild( 0 ).localEulerAngles += rotate;
		for ( int cell = 0; cell < CollisionShape.Cells.Length; cell++ )
		{
			CollisionShape.Cells[cell] = RotatePointAroundPivot( CollisionShape.Cells[cell], Vector3.one * 0.5f, rotate );
		}
	}

	public void OnGrab( object sender, InteractableObjectEventArgs e )
	{
		transform.GetComponentInChildren<Collider>().isTrigger = false;
	}

	public void OnUnGrab( object sender, InteractableObjectEventArgs e )
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
			transform.SetParent( Foundation.transform );
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
			transform.SetParent( null );
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

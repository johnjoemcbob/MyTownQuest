using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFoundation : MonoBehaviour
{
	[Header( "Variables" )]
	public Vector3Int BuildableAreaSize = new Vector3Int( 10, 6, 10 );

	[Header( "References" )]
	public Plot LinkedPlot;
	public Transform Foundation;
	public Transform GridPlane;
	public Transform TriggerZone;

	private Grid Grid;

	private bool[,,] GridCollision;

    void Start()
	{
		Grid = GetComponentInChildren<Grid>();

		if ( LinkedPlot != null )
		{
			BuildableAreaSize = LinkedPlot.Size;
		}

		// Set proper buildable area size
		Foundation.localScale = new Vector3( ( BuildableAreaSize.x / 2.0f ) + 0.5f, 0.5f, ( BuildableAreaSize.z / 2.0f ) + 0.5f );
		GridPlane.localScale = new Vector3( ( BuildableAreaSize.x / 20.0f ), 0.5f, ( BuildableAreaSize.z / 20.0f ) );
		GridPlane.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2( ( BuildableAreaSize.x / 4.0f ), ( BuildableAreaSize.z / 4.0f ) );
		TriggerZone.localPosition = new Vector3( -BuildableAreaSize.x, 0, -BuildableAreaSize.z ) / 4;
		TriggerZone.GetComponent<BoxCollider>().center = new Vector3( BuildableAreaSize.x, BuildableAreaSize.y, BuildableAreaSize.z ) / 2;
		TriggerZone.GetComponent<BoxCollider>().size = BuildableAreaSize;

		// Setup collision, all empty
		GridCollision = new bool[BuildableAreaSize.x + 1, BuildableAreaSize.y + 1, BuildableAreaSize.z + 1];
		for ( int x = 0; x <= BuildableAreaSize.x; x++ )
		{
			for ( int y = 0; y <= BuildableAreaSize.y; y++ )
			{
				for ( int z = 0; z <= BuildableAreaSize.z; z++ )
				{
					GridCollision[x, y, z] = false;
				}
			}
		}
    }

	private void OnTriggerEnter( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned )
		{
			//part.transform.SetParent( transform );
		}
	}

	private void OnTriggerStay( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned && part.Grabbed.IsGrabbed )
		{
			// Blank any old cells
			DeOccupyGrid( part );

			// TODO; Check a few positions near this by offsetting the gridpos +1/-1 in each axis
			// Starting with 0,0,0 obviously. obviously.
			Vector3Int[] attempts = new Vector3Int[]
			{
				new Vector3Int( 0, 0, 0 ),
				new Vector3Int( -1, 0, 0 ),
				new Vector3Int( 1, 0, 0 ),
				new Vector3Int( 0, 0, -1 ),
				new Vector3Int( 0, 0, 1 ),
				new Vector3Int( 0, 1, 0 )
			};
			foreach ( var horizontaloff in attempts )
			{
				// Simplify to grid pos
				Vector3Int gridpos = Grid.WorldToCell( part.transform.position ) + horizontaloff;

				// Cast downwards while level below has no collision issue
				while ( gridpos.y > 0 )
				{
					Vector3Int off = new Vector3Int( 0, -1, 0 );
					if ( !DoesCollide( gridpos, part, off ) )
					{
						gridpos += off;
					}
					else
					{
						break;
					}
				}

				// Check its a valid position based on collision grid
				if ( !DoesCollide( gridpos, part, Vector3Int.zero ) )
				{
					// Occupy the grid cells now
					// Part keeps track of the cells it occupied? so if move it blanks those first?
					OccupyGrid( gridpos, part );

					// Visual there
					Transform visual = part.GetVisual();
					visual.rotation = transform.rotation;
					visual.localEulerAngles += part.transform.GetChild( 0 ).localEulerAngles; // Inherit yaw rotation still (from player input)
					//visual.localEulerAngles = new Vector3( visual.localEulerAngles.x, 0, visual.localEulerAngles.z );
					visual.position = Grid.CellToWorld( gridpos );

					// Swish sound if new snap position
					if ( part.Snapped != Grid.CellToWorld( gridpos ) || part.Foundation != this )
					{
						MyTownQuest.SpawnResourceAudioSource( "swoosh3", transform.position, Random.Range( 0.8f, 2.2f ), 0.2f );
					}

					// Flag as snapped
					part.Snapped = Grid.CellToWorld( gridpos );
					part.Foundation = this;
					part.transform.SetParent( transform );

					break;
				}
				else
				{
					Transform visual = part.GetVisual();
					visual.localPosition = Vector3.zero;
					visual.localEulerAngles = Vector3.zero;

					part.Foundation = null;
					part.transform.SetParent( null );
				}
			}
		}
	}

	private void OnTriggerExit( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned && part.Grabbed.IsGrabbed )
		{
			Transform visual = part.GetVisual();
			visual.localPosition = Vector3.zero;
			visual.localEulerAngles = Vector3.zero;

			part.Foundation = null;
			part.transform.SetParent( null );
		}
	}

	public void OnSnap( BuildingPart part )
	{
		LinkedPlot.OnSnap( part );
	}

	public void OnUnSnap( BuildingPart part )
	{
		LinkedPlot.OnUnSnap( part );
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = new Color( 1, 0, 0, 0.5f );
	//	float size = 0.05f;

	//	// Draw a semitransparent blue cube at the transforms position
	//	for ( int x = 1; x <= BuildableAreaSize.x; x++ )
	//	{
	//		for ( int y = 0; y <= BuildableAreaSize.y; y++ )
	//		{
	//			for ( int z = 1; z <= BuildableAreaSize.z; z++ )
	//			{
	//				if ( GridCollision[x, y, z] )
	//				{
	//					Gizmos.DrawCube( transform.position + transform.up * size + new Vector3( x, y, z ) * size - Vector3.one * size / 2, Vector3.one * size * 1.1f );
	//				}
	//			}
	//		}
	//	}
	//}

	private bool DoesCollide( Vector3Int pos, BuildingPart part, Vector3Int off )
	{
		foreach ( var cell in part.CollisionShape.Cells )
		{
			Vector3Int check = pos + cell + off;
			// If outside grid (i.e. checking if NOT inside)
			if ( !(
				( check.x >  0 && check.x <= BuildableAreaSize.x ) &&
				( check.y >= 0 && check.y <= BuildableAreaSize.y ) &&
				( check.z >  0 && check.z <= BuildableAreaSize.z )
			) )
			{
				return true;
			}
			// If occupied
			if ( GridCollision[check.x, check.y, check.z] )
			{
				return true;
			}
		}
		return false;
	}

	private void OccupyGrid( Vector3Int pos, BuildingPart part )
	{
		foreach ( var cell in part.CollisionShape.Cells )
		{
			Vector3Int occupy = pos + cell;
			GridCollision[occupy.x, occupy.y, occupy.z] = true;
			part.OccupiedCells.Add( occupy );
		}
	}

	private void DeOccupyGrid( BuildingPart part )
	{
		foreach ( var cell in part.OccupiedCells )
		{
			GridCollision[cell.x, cell.y, cell.z] = false;
		}
		part.OccupiedCells.Clear();
	}
}

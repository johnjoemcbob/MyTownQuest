using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFoundation : MonoBehaviour
{
	[Header( "Variables" )]
	public Vector3Int BuildableAreaSize = new Vector3Int( 10, 6, 10 );

	private Grid Grid;

	private bool[,,] GridCollision;

    void Start()
	{
		Grid = GetComponent<Grid>();

		// Set proper buildable area size
		transform.localPosition = new Vector3( -BuildableAreaSize.x, 0, -BuildableAreaSize.z ) / 2;
		GetComponent<BoxCollider>().center = new Vector3( BuildableAreaSize.x, BuildableAreaSize.y, BuildableAreaSize.z ) / 2;
		GetComponent<BoxCollider>().size = BuildableAreaSize;

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

    void Update()
    {
        
    }

	private void OnTriggerEnter( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned )
		{
			part.transform.SetParent( transform );
		}
	}

	private void OnTriggerStay( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned && part.Grabbed.IsGrabbed )
		{
			// TODO; Check a few positions near this by offsetting the gridpos +1/-1 in each axis
			// Starting with 0,0,0 obviously. obviously.

			// Simplify to grid pos
			Vector3Int gridpos = Grid.WorldToCell( part.transform.position );

			// Blank any old cells
			DeOccupyGrid( part );

			// Check its a valid position based on collision grid
			if ( !DoesCollide( gridpos, part ) )
			{
				// Occupy the grid cells now
				// Part keeps track of the cells it occupied? so if move it blanks those first?
				OccupyGrid( gridpos, part );

				// Visual there
				Transform visual = part.GetVisual();
				visual.position = Grid.CellToWorld( gridpos );
				visual.rotation = transform.rotation;
			}
		}
	}

	private void OnTriggerExit( Collider other )
	{
		BuildingPart part = other.attachedRigidbody.GetComponentInParent<BuildingPart>();
		if ( part != null )
		{
			part.transform.SetParent( null );
		}
	}

	private bool DoesCollide( Vector3Int pos, BuildingPart part )
	{
		foreach ( var cell in part.CollisionShape.Cells )
		{
			Vector3Int check = pos + cell;
			// If outside grid (i.e. checking if NOT inside)
			if ( !(
				( check.x >= 0 && check.x <= BuildableAreaSize.x ) &&
				( check.y >= 0 && check.y <= BuildableAreaSize.y ) &&
				( check.z >= 0 && check.z <= BuildableAreaSize.z )
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

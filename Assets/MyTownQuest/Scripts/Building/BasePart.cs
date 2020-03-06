using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Prefabs.Interactions.Interactables.Grab.Action;
using Zinnia.Action;
using Zinnia.Action.Collection;

public class PlotPartComparer : IComparer<KeyValuePair<BasePart, GameObject>>
{
	public int Compare( KeyValuePair<BasePart, GameObject> a, KeyValuePair<BasePart, GameObject> b )
	{
		int ay = a.Key.SnappedCell.y;
		int by = b.Key.SnappedCell.y;
		return ay == by ? 0 : ( ay < by ? 1 : -1 );
	}
}

public class BasePart : MonoBehaviour
{
	[HideInInspector]
	public bool IsSpawned = false;

	[HideInInspector]
	public Vector3Int SnappedCell;
	[HideInInspector]
	public List<Vector3Int> OccupiedCells = new List<Vector3Int>();

	protected Transform Visual;

	public virtual void Start()
	{
		Visual = transform.GetChild( 0 ).GetChild( 0 ).GetChild( 0 );
	}

	public Transform GetVisual()
	{
		return Visual;
	}

	public Transform GetVisualParent()
	{
		return transform.GetChild( 0 ).GetChild( 0 );
	}

}

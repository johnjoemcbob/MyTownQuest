using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
	public Vector3Int Size;

	private Dictionary<BuildingPart, GameObject> Parts = new Dictionary<BuildingPart, GameObject>();

	// Visualise via gizmos
	void OnDrawGizmos()
	{
		Gizmos.color = new Color( 1, 0, 0, 0.5f );
		float size = 1.5f;// FindObjectOfType<MapLoader>().Scale;
		Gizmos.DrawCube( transform.position + new Vector3( Size.x, Size.y, Size.z ) * size / 2, new Vector3( Size.x, Size.y, Size.z ) * size );
	}

	public GameObject GetBig( BuildingPart part )
	{
		return Parts[part];
	}

	public void OnSnap( BuildingPart part )
	{
		float scale = MapLoader.Instance.Scale * 2;

		// Find or create
		GameObject big = null;
		if ( !Parts.ContainsKey( part ) )
		{
			big = Instantiate( part.GetVisual().gameObject, transform );
			Parts.Add( part, big );
		}
		else
		{
			big = Parts[part];
		}

		// Set position
		big.transform.localScale = Vector3.one * scale;
		big.transform.localPosition = ( part.transform.localPosition + new Vector3( Size.x / 4.0f, -0.5f, Size.z / 4.0f ) ) * scale;
		big.transform.localEulerAngles = part.transform.GetChild( 0 ).localEulerAngles;
	}

	public void OnUnSnap( BuildingPart part )
	{
		if ( Parts.ContainsKey( part ) )
		{
			Destroy( Parts[part] );

			Parts.Remove( part );
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
	public Vector3Int Size;

	public Dictionary<BasePart, GameObject> Parts = new Dictionary<BasePart, GameObject>();

	// Visualise via gizmos
	void OnDrawGizmos()
	{
		Gizmos.color = new Color( 1, 0, 0, 0.5f );
		float size = 1.5f;// FindObjectOfType<MapLoader>().Scale;
		Gizmos.DrawCube( transform.position + new Vector3( Size.x, Size.y, Size.z ) * size / 2, new Vector3( Size.x, Size.y, Size.z ) * size );
	}

	public GameObject GetBig( BasePart part )
	{
		return Parts[part];
	}

	public void OnSnap( BasePart part, BasePart parent = null )
	{
		float scale = MapLoader.Instance.Scale;

		PartLerper lerp = null;

		//Vector3 targetpos = ( part.transform.localPosition + new Vector3( Size.x / 4.0f, -0.5f, Size.z / 4.0f ) ) * scale;
		Vector3 targetpos =
			transform.right * ( part.SnappedCell.x ) * scale +
			new Vector3( 0, part.SnappedCell.y * scale, 0 ) +
			transform.forward * ( part.SnappedCell.z ) * scale;
		Vector3 targetang = part.transform.GetChild( 0 ).localEulerAngles;
		Vector3 targetsca = Vector3.one * scale * 2;
		// Decor snaps differently.
		if ( parent != null )
		{
			targetpos = part.GetVisual().localPosition;
			targetang = part.GetVisual().localEulerAngles;
			targetsca = Vector3.one;
		}

		// Find or create
		GameObject big = null;
		if ( !Parts.ContainsKey( part ) )
		{
			big = Instantiate( part.GetVisual().gameObject, transform );
				// Decor snaps differently.
				if ( parent != null )
				{
					big.transform.SetParent( Parts[parent].transform );
				}
			Parts.Add( part, big );

			lerp = big.AddComponent( typeof( PartLerper ) ) as PartLerper;
			lerp.StartPos = targetpos + Vector3.up * 5;
			lerp.StartAng = targetang + new Vector3( 0, 180, 0 );
			lerp.StartSca = Vector3.zero;
			lerp.CurrentState = PartLerper.State.In;
		}
		else
		{
			big = Parts[part];

			lerp = big.GetComponent<PartLerper>();
			lerp.StartPos = big.transform.localPosition;
			lerp.StartAng = big.transform.localEulerAngles;
			lerp.StartSca = big.transform.localScale;
			lerp.CurrentState = PartLerper.State.Move;
		}

		// Lerp to new pos!
		{
			lerp.Duration = 0.2f;

			lerp.TargetPos = targetpos;
			lerp.TargetAng = targetang;
			lerp.TargetSca = targetsca;
		}
		lerp.Play();
	}

	public void OnUnSnap( BasePart part )
	{
		if ( Parts.ContainsKey( part ) )
		{
			PartLerper lerp = Parts[part].GetComponent<PartLerper>();
			lerp.CurrentState = PartLerper.State.Out;
			lerp.PlayBackwards();
			lerp.TargetPos = lerp.StartPos + Vector3.up * 5;
			lerp.TargetAng = lerp.StartAng + new Vector3( 0, 180, 0 );

			// Remove all and any decor attached also
			foreach ( var child in part.GetComponentsInChildren<BasePart>() )
			{
				Parts.Remove( child );
			}
		}
	}
}

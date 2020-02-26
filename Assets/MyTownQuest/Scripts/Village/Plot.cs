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

		PartLerper lerp = null;

		Vector3 targetpos = ( part.transform.localPosition + new Vector3( Size.x / 4.0f, -0.5f, Size.z / 4.0f ) ) * scale;
		Vector3 targetang = part.transform.GetChild( 0 ).localEulerAngles;
		Vector3 targetsca = Vector3.one * scale;

		// Find or create
		GameObject big = null;
		if ( !Parts.ContainsKey( part ) )
		{
			big = Instantiate( part.GetVisual().gameObject, transform );
			Parts.Add( part, big );

			lerp = big.AddComponent( typeof( PartLerper ) ) as PartLerper;
			lerp.StartPos = targetpos + Vector3.up * 5;
			lerp.StartAng = Vector3.zero;
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

	public void OnUnSnap( BuildingPart part )
	{
		if ( Parts.ContainsKey( part ) )
		{
			Parts[part].GetComponent<PartLerper>().CurrentState = PartLerper.State.Out;
			Parts[part].GetComponent<PartLerper>().PlayBackwards();
			Parts[part].GetComponent<PartLerper>().TargetPos = Parts[part].GetComponent<PartLerper>().StartPos + Vector3.up * 5;

			Parts.Remove( part );
		}
	}
}

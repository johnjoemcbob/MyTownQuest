using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorSnapLogic : MonoBehaviour
{
	public static List<DecorSnapLogic> Snaps = new List<DecorSnapLogic>();

	public BuildingPart Part;

	private void Start()
	{
		Snaps.Add( this );

		Part = GetComponentInParent<BuildingPart>();
	}

	public void OnSnapped( GameObject snap )
	{
		snap.transform.SetParent( GetComponentInParent<BuildingPart>().GetVisual() );
	}

	public void OnUnSnapped( GameObject snap )
	{
		snap.transform.SetParent( null );
	}
}

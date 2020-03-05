using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorSnapLogic : MonoBehaviour
{
	public void OnSnapped( GameObject snap )
	{
		snap.transform.SetParent( GetComponentInParent<BuildingPart>().GetVisual() );
	}

	public void OnUnSnapped( GameObject snap )
	{
		snap.transform.SetParent( null );
	}
}

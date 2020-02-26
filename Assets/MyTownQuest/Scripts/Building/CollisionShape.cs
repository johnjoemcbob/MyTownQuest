using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionShape : MonoBehaviour
{
	public Vector3Int[] Cells;
	public bool MustBeGrounded = true;

	void OnDrawGizmosSelected()
	{
		// Draw a semitransparent blue cube at the transforms position
		Gizmos.color = new Color( 1, 0, 0, 0.5f );
		foreach ( var cell in Cells )
		{
			float size = 0.5f;
			Gizmos.DrawCube( transform.position + transform.up * size + new Vector3( cell.x, cell.y, cell.z ) * size - Vector3.one * size / 2, Vector3.one * size * 1.1f );
		}
	}
}

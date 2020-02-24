using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBucket : MonoBehaviour
{
	[Header( "Variables" )]
	public Color PaintColour;

	[Header( "References" )]
	public MeshRenderer PaintRenderer;

    void Start()
    {

    }

    void Update()
    {
		PaintRenderer.material.color = PaintColour;
	}
}

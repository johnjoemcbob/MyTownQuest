using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
	public static MapLoader Instance;

	[Serializable]
	public struct VillageCell
	{
		public Color Colour;
		public GameObject Prefab;
	}

	public Texture2D MapTexture;
	public VillageCell[] DataParse;
	public float Scale = 3;

	[HideInInspector]
	public int MapWidth;
	[HideInInspector]
	public int MapHeight;
	[HideInInspector]
	public GameObject[,] MapGrid;

	private void Awake()
	{
		Instance = this;
	}

	public void Generate()
    {
		// Cleanup
		foreach ( Transform child in transform.GetChild( 0 ) )
		{
			DestroyImmediate( child.gameObject );
		}

		// Load
		MapWidth = MapTexture.width;
		MapHeight = MapTexture.height;

		MapGrid = new GameObject[MapWidth, MapHeight];

		for ( int x = 0; x < MapWidth; x++ )
		{
			for ( int y = 0; y < MapHeight; y++ )
			{
				Color col = MapTexture.GetPixel( x, y );
				bool found = false;
				foreach ( var parse in DataParse )
				{
					if ( parse.Colour == col )
					{
						MapGrid[x, y] = Instantiate( parse.Prefab, transform.GetChild( 0 ) );
						MapGrid[x, y].transform.localPosition = new Vector3( x, 0, y ) * Scale;
						MapGrid[x, y].transform.localScale *= Scale;

						found = true;
						break;
					}
				}
				if ( !found )
				{
					Debug.LogWarning( "Warning: Missing parse for colour: " + col );
				}
			}
		}
    }
}

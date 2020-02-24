using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor( typeof( MapLoader ) )]
public class MapLoaderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if ( GUILayout.Button( "Load Map" ) )
		{
			MapLoader loader = (MapLoader) target;
			loader.Generate();
		}
	}
}
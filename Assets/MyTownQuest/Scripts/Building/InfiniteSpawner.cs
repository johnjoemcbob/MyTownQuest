using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawner : MonoBehaviour
{
	[Header( "Variables" )]
	public float MaxDistance = 0.1f;

	[Header( "References" )]
	public GameObject ModelPrefab;
	public GameObject GrabPrefab;

	private GameObject CurrentGrabbable;

    void Start()
    {
		// Set display model as prefab
		// Spawn a grabbable
		SpawnGrabbable();
	}

	void Update()
    {
		// If out of range (i.e. has been grabbed and removed)
		if ( CurrentGrabbable && Vector3.Distance( CurrentGrabbable.transform.position, transform.position ) > MaxDistance )
		{
			// Flag to spawn a new one
			CurrentGrabbable.GetComponent<BuildingPart>().Spawned = true;
			CurrentGrabbable = null;
		}

		// Spawn a new one
        if ( CurrentGrabbable == null )
		{
			SpawnGrabbable();
		}
    }

	private void SpawnGrabbable()
	{
		CurrentGrabbable = Instantiate( GrabPrefab, transform );
		GameObject mdl = Instantiate( ModelPrefab, CurrentGrabbable.transform.GetChild( 0 ) );
		//CurrentGrabbable.GetComponent<Rigidbody>().isKinematic = true;

		MyTownQuest.SpawnResourceAudioSource( "click1", transform.position, Random.Range( 0.8f, 1.2f ) );
	}
}

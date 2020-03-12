using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TeleportFollower : MonoBehaviour
{
    void Start()
    {
		foreach ( var teleport in FindObjectsOfType<VRTK_BasicTeleport>() )
		{
			teleport.Teleported += OnTeleport;
		}
    }

    void OnTeleport( object sender, DestinationMarkerEventArgs e )
	{
		transform.position = e.destinationPosition;
		transform.rotation = Quaternion.Euler( 0, VRTK_DeviceFinder.HeadsetTransform().transform.eulerAngles.y, 0 );
	}
}

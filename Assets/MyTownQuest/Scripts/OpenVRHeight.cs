using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OpenVRHeight : MonoBehaviour
{
    void Start()
    {
		List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
		SubsystemManager.GetInstances<XRInputSubsystem>( subsystems );
		for ( int i = 0; i < subsystems.Count; i++ )
		{
			subsystems[i].TrySetTrackingOriginMode( TrackingOriginModeFlags.Floor );
		}
	}
}

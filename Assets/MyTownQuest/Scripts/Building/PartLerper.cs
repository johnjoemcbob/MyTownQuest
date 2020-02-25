using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartLerper : MonoBehaviour
{
	public float Duration;

	public Vector3 StartPos;
	public Vector3 StartAng;
	public Vector3 StartSca;
	public Vector3 TargetPos;
	public Vector3 TargetAng;
	public Vector3 TargetSca;

	private float StartTime = 0;

	void Update()
    {
        if ( StartTime != 0 )
		{
			float progress = Mathf.Clamp( Time.time - StartTime, 0, Duration ) / Duration;
			transform.localPosition = Vector3.Lerp( StartPos, TargetPos, progress );
			transform.localRotation = Quaternion.Lerp( Quaternion.Euler( StartAng ), Quaternion.Euler( TargetAng ), progress );
			transform.localScale = Vector3.Lerp( StartSca, TargetSca, progress );

			// Auto stop when duration elapsed
			if ( progress == 1 )
			{
				Stop();
			}
		}
    }

	public void Play()
	{
		StartTime = Time.time;
	}

	public void PlayBackwards()
	{
		Vector3 startpos = StartPos;
		Vector3 startang = StartAng;
		Vector3 startsca = StartSca;

		StartPos = TargetPos;
		StartAng = TargetAng;
		StartSca = TargetSca;

		TargetPos = startpos;
		TargetAng = startang;
		TargetSca = startsca;

		Play();
	}

	public void Stop()
	{
		StartTime = 0;
	}
}

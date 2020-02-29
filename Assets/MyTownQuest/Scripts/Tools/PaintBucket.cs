using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBucket : MonoBehaviour
{
	[Header( "Variables" )]
	public Color PaintColour;
	public Vector3 HitScale = new Vector3( 1.5f, 1.5f, 1.5f );
	public float HitAngle = 30;
	public float LerpDuration = 0.2f;

	[Header( "References" )]
	public MeshRenderer PaintRenderer;

	private Quaternion TargetAng;
	private float HitTime = 0;

	void Start()
    {

    }

    void Update()
    {
		PaintRenderer.material.color = PaintColour;

		if ( HitTime != 0 )
		{
			float progress = Mathf.Clamp( Time.time - HitTime, 0, LerpDuration ) / LerpDuration;
			transform.GetChild( 0 ).localScale = Vector3.Lerp( transform.GetChild( 0 ).localScale, Vector3.one, progress );
			transform.GetChild( 0 ).localRotation = Quaternion.Lerp( transform.GetChild( 0 ).localRotation, TargetAng, progress );

			// Stop lerping once finished anim
			HitTime = ( progress == 1 ) ? 0 : HitTime;
		}
	}

	public void OnPaintBrush( IsGrabbedTracker grab )
	{
		if ( HitTime == 0 )
		{
			TargetAng = Quaternion.Euler( transform.GetChild( 0 ).localEulerAngles );
		}

		HitTime = Time.time;

		// Hit scale increase and let Update lerp it back to normal size
		transform.GetChild( 0 ).localScale = HitScale;
		float range = HitAngle;
		transform.GetChild( 0 ).localEulerAngles += new Vector3( Random.Range( -range, range ), Random.Range( -range, range ), Random.Range( -range, range ) );

		// Effects
		GameObject particles = MyTownQuest.EmitParticleImpact( transform.position + transform.up * 0.2f );
		particles.transform.LookAt( transform.position + transform.up * 5 );
		ParticleSystemRenderer sys = particles.GetComponentInChildren<ParticleSystemRenderer>();
		sys.material.color = PaintColour;

		MyTownQuest.SpawnResourceAudioSource( "splash1", transform.position, Random.Range( 0.8f, 2.2f ), 0.2f );
		if ( grab.LastGrabbedBy != null )
		{
			MyTownQuest.VibrateController( grab.LastGrabbedBy.name.Contains( "Left" ), 10, Random.Range( 0.3f, 0.5f ) );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		BuildingPart part = other.GetComponentInParent<BuildingPart>();
		if ( part != null )
		{
			foreach ( var renderer in part.GetComponentsInChildren<Renderer>() )
			{
				foreach ( var material in renderer.materials )
				{
					material.color = PaintColour;
				}
			}

			OnPaintBrush( other.GetComponentInParent<IsGrabbedTracker>() );
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PaintBrush : MonoBehaviour
{
	[Header( "Variables" )]
	public int MaterialEdit = 0;
	public float DistanceMult = 1;
	public float HorizontalAngle = 30;
	public float VerticalAngle = 10;
	public float BristleLerpSpeed = 5;
	public float ColourApplyLerpSpeed = 1;
	public float ColourGainLerpSpeed = 1;

	[Header( "References" )]
	public Transform BristlesParent;

	[HideInInspector]
	public Color Colour;

	private KinematicVelocity Kinematic;

	private void Start()
	{
		Kinematic = GetComponent<KinematicVelocity>();
		Colour = BristlesParent.GetComponentInChildren<MeshRenderer>().material.color;

		var interact = GetComponent<VRTK_InteractableObject>();
		//interact.InteractableObjectGrabbed += OnGrab;
		//interact.InteractableObjectUngrabbed += OnUnGrab;
		interact.InteractableObjectUsed += OnUseWhileHeld;
	}

	private void Update()
	{
		foreach ( Transform bristle in BristlesParent )
		{
			Vector3 target = Vector3.zero;
			{
				float dist = Kinematic.Velocity.magnitude + Kinematic.AngularVelocity.magnitude + Vector3.Distance( bristle.position, transform.position );
				float vertdist = dist * Vector3.Dot( Kinematic.Velocity, transform.forward );
				dist *= Vector3.Dot( Kinematic.Velocity, transform.right );
				target = new Vector3( Mathf.Clamp( vertdist * DistanceMult * VerticalAngle, -VerticalAngle, VerticalAngle ), 0, Mathf.Clamp( dist * DistanceMult * HorizontalAngle, -HorizontalAngle, HorizontalAngle ) );
			}
			bristle.localRotation = Quaternion.Lerp( bristle.localRotation, Quaternion.Euler( target ), Time.deltaTime * BristleLerpSpeed );
		}
	}

	private void OnTriggerEnter( Collider other )
	{
		// Bucket
		var bucket = other.GetComponentInParent<PaintBucket>();
		if ( bucket != null )
		{
			bucket.OnPaintBrush( GetComponent<IsGrabbedTracker>() );
		}

		// Building Part
		var part = other.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned )
		{
			GameObject particles = MyTownQuest.EmitParticleImpact( BristlesParent.position );
			particles.transform.localScale *= 0.01f;
			ParticleSystemRenderer sys = particles.GetComponentInChildren<ParticleSystemRenderer>();
			sys.material.color = Colour;
			part.OnColourChange();

			MyTownQuest.SpawnResourceAudioSource( "stroke" + Random.Range( 1, 5 + 1 ), BristlesParent.position, Random.Range( 0.8f, 1.2f ), 0.5f );
		}
	}

	private void OnTriggerStay( Collider other )
	{
		// Bucket
		var bucket = other.GetComponentInParent<PaintBucket>();
		if ( bucket != null )
		{
			Colour = Color.Lerp( Colour, bucket.PaintColour, Time.deltaTime * ColourGainLerpSpeed );

			foreach ( Transform bristle in BristlesParent )
			{
				var ren = bristle.GetChild( 0 ).GetChild( 0 ).GetComponent<MeshRenderer>();
				ren.material.color = Colour;
			}
		}

		// Building Part
		var part = other.GetComponentInParent<BuildingPart>();
		if ( part != null && part.IsSpawned )
		{
			foreach ( var ren in part.GetComponentsInChildren<MeshRenderer>() )
			{
				int index = Mathf.Clamp( MaterialEdit, 0, ren.materials.Length - 1 );
				ren.materials[index].color = Color.Lerp( ren.materials[index].color, Colour, Time.deltaTime * ColourApplyLerpSpeed );
			}
		}
	}

	public void OnUseWhileHeld( object sender, InteractableObjectEventArgs e )
	{
		MaterialEdit = ( MaterialEdit == 1 ) ? 0 : 1;

		float scale = 1;
			if ( MaterialEdit != 1 )
			{
				scale = 0.5f;
			}
		transform.localScale = new Vector3( scale, 1, scale );
	}
}

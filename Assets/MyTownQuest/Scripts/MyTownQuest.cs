using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GoogleSheetsToUnity;

// Main class with helpers
public class MyTownQuest : MonoBehaviour
{
	public static float NextAllowedLoad = 0;

	public const float BetweenLoads = 20;
	public const float BetweenSheetRefreshes = 5;

	bool loading = false;

	public void Start()
	{
		//StartCoroutine( KeepSheetsUpToDate() );

		//Debug.Log( SystemInfo.deviceUniqueIdentifier );

		mono_gmail.Init();
		//mono_gmail.SendMail();
	}

	//private void Update()
	//{
	//	Debug.Log( SystemInfo.deviceUniqueIdentifier );
	//}

	#region Sheets
	public IEnumerator KeepSheetsUpToDate()
	{
		while ( true )
		{
			GSTU_Search search;

			// Overall
			search = new GSTU_Search( "11OrTxEBP8hyyjbZG-SV7oFkDuMbAw-24jSlZ_v7c0lI", "Overall" );
			SpreadsheetManager.ReadPublicSpreadsheet( search, OnLoaded_Overall );

			// Tools
			search = new GSTU_Search( "11OrTxEBP8hyyjbZG-SV7oFkDuMbAw-24jSlZ_v7c0lI", "Tools" );
			SpreadsheetManager.ReadPublicSpreadsheet( search, OnLoaded_Tools );

			// Debug
			search = new GSTU_Search( "11OrTxEBP8hyyjbZG-SV7oFkDuMbAw-24jSlZ_v7c0lI", "Debug" );
			SpreadsheetManager.ReadPublicSpreadsheet( search, OnLoaded_Debug );

			// Helpers
			search = new GSTU_Search( "11OrTxEBP8hyyjbZG-SV7oFkDuMbAw-24jSlZ_v7c0lI", "Helpers" );
			SpreadsheetManager.ReadPublicSpreadsheet( search, OnLoaded_Helpers );

			yield return new WaitForSeconds( BetweenSheetRefreshes );
		}
	}

	public void OnLoaded_Overall( GstuSpreadSheet sheet )
	{
		int index = 0;
		foreach ( var row in sheet.rows.primaryDictionary )
		{
			// Ignore title row
			if ( index != 0 )
			{
				for ( int cell = 0; cell < row.Value.Count; cell++ )
				{
					string val = row.Value[cell].value;
					switch ( cell )
					{
						// Physics Grab
						case 0:
							//bool physics = bool.Parse( val );
							//foreach ( var obj in FindObjectsOfType<GrabInteractableFollowAction>() )
							//{
							//	obj.FollowTracking = physics ? GrabInteractableFollowAction.TrackingType.FollowRigidbody : GrabInteractableFollowAction.TrackingType.FollowTransform;
							//}

							break;
						default:
							break;
					}
				}
			}
			index++;
		}
	}

	public void OnLoaded_Tools( GstuSpreadSheet sheet )
	{
		int index = 0;
		foreach ( var row in sheet.rows.primaryDictionary )
		{
			// Ignore title row
			if ( index != 0 )
			{
				string name = "";
				float weight = 0;
				string prefab = "";
				bool offset = false;

				GameObject obj = null;

				for ( int cell = 0; cell < row.Value.Count; cell++ )
				{
					string val = row.Value[cell].value;
					switch ( cell )
					{
						case 0:
							name = val;

							break;
						case 1:
							weight = float.Parse( val );

							obj = GameObject.Find( name );
							obj.GetComponentInChildren<Rigidbody>().mass = weight;

							break;
						case 2:
							prefab = val;

							break;
						case 3:
							offset = bool.Parse( val );

							//obj.GetComponentInChildren<GrabInteractableFollowAction>().GrabOffset = offset ? GrabInteractableFollowAction.OffsetType.PrecisionPoint : GrabInteractableFollowAction.OffsetType.OrientationHandle;

							break;
						default:
							break;
					}
				}
			}
			index++;
		}
	}

	public void OnLoaded_Debug( GstuSpreadSheet sheet )
	{
		int index = 0;
		foreach ( var row in sheet.rows.primaryDictionary )
		{
			// Ignore title row
			if ( index != 0 )
			{
				string deviceID = "";
				bool reset = false;
				bool demoscene = false;
				bool casthelper = false;
				bool showmessage = false;
				string message = "";

				GameObject messageobj = null;

				for ( int cell = 0; cell < row.Value.Count; cell++ )
				{
					string val = row.Value[cell].value;
					switch ( cell )
					{
						case 0:
							deviceID = val;

							if ( deviceID != SystemInfo.deviceUniqueIdentifier ) return;

							break;
						case 1:
							reset = bool.Parse( val );

							Debug.Log( reset );
							if ( reset )
							{
								ButtonResetScene();
							}

							break;
						case 2:
							demoscene = bool.Parse( val );

							Debug.Log( demoscene );
							if ( demoscene )
							{
								ButtonTestScene();
							}

							break;
						case 3:
							casthelper = bool.Parse( val );

							// Disabling means it can't be found later.. quick hack
							GameObject.Find( "CastingHelper" ).transform.localScale = casthelper ? Vector3.one : Vector3.zero;

							break;
						case 4:
							showmessage = bool.Parse( val );

							messageobj = GameObject.Find( "Message (Canvas)" );
							messageobj.GetComponent<Canvas>().enabled = showmessage;

							break;
						case 5:
							message = val;

							messageobj.GetComponentInChildren<Text>().text = message;

							break;
						default:
							break;
					}
				}
			}
			index++;
		}
	}

	public void OnLoaded_Helpers( GstuSpreadSheet sheet )
	{
		// Hide arrows unless any are on
		GameObject arrowobj = GameObject.Find( "Arrow (Canvas)" );
		arrowobj.GetComponent<Canvas>().enabled = false;

		int index = 0;
		foreach ( var row in sheet.rows.primaryDictionary )
		{
			// Ignore title row
			if ( index != 0 )
			{
				string name = "";
				bool highlights = false;
				bool arrow = false;

				for ( int cell = 0; cell < row.Value.Count; cell++ )
				{
					string val = row.Value[cell].value;
					switch ( cell )
					{
						case 0:
							name = val;

							break;
						case 1:
							highlights = bool.Parse( val );

							break;
						case 2:
							arrow = bool.Parse( val );

							break;
						default:
							break;
					}
				}

				// Logic
				if ( arrow )
				{
					arrowobj.GetComponent<Canvas>().enabled = true;
					arrowobj.transform.position = GameObject.Find( name ).transform.position;
				}
			}
			index++;
		}
	}
	#endregion

	#region Loading
	public void ButtonResetScene()
	{
		if ( loading || NextAllowedLoad > Time.time ) return;

		AudioClip clip = Resources.Load( "Sounds/phone" ) as AudioClip;
		AudioSource.PlayClipAtPoint( clip, transform.position, 1 );

		SceneManager.LoadSceneAsync( 0 );
		loading = true;
		NextAllowedLoad = Time.time + BetweenLoads;
	}

	public void ButtonTestScene()
	{
		if ( loading || NextAllowedLoad > Time.time ) return;

		AudioClip clip = Resources.Load( "Sounds/phone" ) as AudioClip;
		AudioSource.PlayClipAtPoint( clip, transform.position, 1 );

		SceneManager.LoadSceneAsync( 1 );
		loading = true;
		NextAllowedLoad = Time.time + BetweenLoads;
	}
	#endregion

	#region Statics
	public static GameObject CreateGrabbable( GameObject from )
	{
		GameObject grabbable = Instantiate( Resources.Load( "Prefabs/BaseGrabbable" ), from.transform.parent ) as GameObject;
		{
			grabbable.transform.position = from.transform.position;
			grabbable.transform.rotation = from.transform.rotation;

			from.transform.SetParent( grabbable.transform.GetChild( 0 ) );

			from.transform.localPosition = Vector3.zero;
			from.transform.localEulerAngles = Vector3.zero;
		}
		return grabbable;
	}

	public static GameObject EmitParticleImpact( Vector3 point )
	{
		GameObject particle = Instantiate( Resources.Load( "Prefabs/Particle Effect" ) ) as GameObject;
		{
			particle.transform.position = point;

			Destroy( particle, 1.5f );
		}
		return particle;
	}

	public static GameObject EmitParticleDust( Vector3 point )
	{
		GameObject particle = Instantiate( Resources.Load( "Prefabs/Particle Dust" ) ) as GameObject;
		{
			particle.transform.position = point;

			Destroy( particle, 2 );
		}
		return particle;
	}

	public static GameObject SpawnResourceAudioSource( string clipname, Vector3 point, float pitch = 1, float volume = 1 )
	{
		AudioClip clip = Resources.Load( "Sounds/" + clipname ) as AudioClip;
		return SpawnAudioSource( clip, point, pitch, volume );
	}

	public static GameObject SpawnAudioSource( AudioClip clip, Vector3 point, float pitch = 1, float volume = 1 )
	{
		GameObject source = Instantiate( Resources.Load( "Prefabs/Audio Source" ) ) as GameObject;
		{
			source.transform.position = point;

			AudioSource audio = source.GetComponent<AudioSource>();
			audio.clip = clip;
			audio.pitch = pitch;
			audio.volume = volume;
			audio.Play();

			Destroy( source, clip.length + 0.1f );
		}
		return source;
	}
	#endregion

	#region Input
	public void OnLeftUseActivationStateChanged( bool change )
	{
		if ( change )
		{
			foreach ( var input in FindObjectsOfType<InputReceiver>() )
			{
				input.OnInternalUse( "Left" );
			}
		}
	}

	public void OnRightUseActivationStateChanged( bool change )
	{
		if ( change )
		{
			foreach ( var input in FindObjectsOfType<InputReceiver>() )
			{
				input.OnInternalUse( "Right" );
			}
		}
	}

	public static void VibrateController( bool left, float intensity = 1, float duration = 0.005f )
	{
		// TODO FIX
		//FindObjectOfType<XRNodeHapticPulser>().Node = left ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand;
		//FindObjectOfType<XRNodeHapticPulser>().Intensity = intensity;
		//FindObjectOfType<XRNodeHapticPulser>().Duration = duration;
		//FindObjectOfType<XRNodeHapticPulser>().Begin();
	}
	#endregion

	#region Events
	public void OnTeleported()
	{
		Debug.Log( "ON TELEPORT" );
		StartCoroutine( DelayedFollow() );
	}
	IEnumerator DelayedFollow()
	{
		yield return new WaitForEndOfFrame();

		GameObject playarea = GameObject.Find( "TeleportFollower" );
		GameObject headset = Camera.main.gameObject;
		playarea.transform.position = new Vector3( headset.transform.position.x, playarea.transform.position.y, headset.transform.position.z );
		playarea.transform.eulerAngles = new Vector3( 0, headset.transform.eulerAngles.y, 0 );
	}
	#endregion
}

using System.Collections;
using UnityEngine;
using VRTK;

public class PolaroidCamera : MonoBehaviour
{
	[Header( "Variables" )]
	public float LerpSpeed = 5;
	public float ButtonLerpSpeed = 5;
	public float BodyHitAngle = 30;

	[Header( "References" )]
	public Transform Button;

	[Header( "Assets" )]
	public RenderTexture Texture;

    private ImgurClient imgurClient;

	private void OnEnable()
	{
		string id = ( Resources.Load( "Text Assets/imgur_client" ) as TextAsset ).text;
		imgurClient = new ImgurClient( id );
		imgurClient.OnImageUploaded += ImgurClient_OnImageUploaded;

		// VRTK input here
		var interact = GetComponent<VRTK_InteractableObject>();
		interact.InteractableObjectUsed += OnUseWhileHeld;
	}

	private void Update()
	{
		// Visual lerp
		Button.localScale = Vector3.Lerp( Button.localScale, Vector3.one, Time.deltaTime * ButtonLerpSpeed );
		transform.GetChild( 0 ).localScale = Vector3.Lerp( transform.GetChild( 0 ).localScale, Vector3.one, Time.deltaTime * LerpSpeed );
		transform.GetChild( 0 ).localRotation = Quaternion.Lerp( transform.GetChild( 0 ).localRotation, Quaternion.Euler( Vector3.zero ), Time.deltaTime * LerpSpeed );

		// Debug input
		if ( Input.GetKeyDown( KeyCode.P ) )
		{
			TakePhoto();
		}
	}

	private void ImgurClient_OnImageUploaded(object sender, ImgurClient.OnImageUploadedEventArgs e)
    {
        Debug.Log( e.response.data.link );
		mono_gmail.SendMail( "New Image!", e.response.data.link );
    }

	public void TakePhoto()
	{
		// Logic
		Texture2D tex = new Texture2D( Texture.width, Texture.height );
		{
			RenderTexture.active = Texture;
			tex.ReadPixels( new Rect( 0, 0, Texture.width, Texture.height ), 0, 0 );
			tex.Apply();
		}
		string base64 = System.Convert.ToBase64String( tex.EncodeToPNG() );
		imgurClient.UploadImage( base64 );

		// Audio effects
		MyTownQuest.SpawnResourceAudioSource( "camera", transform.position, 1, 5 );

		// Visual effects
		Button.localScale = new Vector3( 1.5f, 0.1f, 1.5f );
		transform.GetChild( 0 ).localScale = new Vector3( 1.1f, 1.1f, 1.1f );
		transform.GetChild( 0 ).localEulerAngles = new Vector3( Random.Range( -1, 1.0f ), Random.Range( -1, 1.0f ), Random.Range( -1, 1.0f ) ) * BodyHitAngle;

		// Light flash
		StartCoroutine( Flash() );
	}

	public void OnUseWhileHeld( object sender, InteractableObjectEventArgs e )
	{
		TakePhoto();
	}

	public IEnumerator Flash()
	{
		float dur = 0.6f;

		FindObjectOfType<VRTK_HeadsetFade>().Fade( new Color( 1, 1, 1, 0.6f ), dur );

		yield return new WaitForSeconds( dur );

		FindObjectOfType<VRTK_HeadsetFade>().Unfade( dur );
	}
}

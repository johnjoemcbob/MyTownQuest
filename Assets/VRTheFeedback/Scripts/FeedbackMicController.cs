using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeedbackMicController : MonoBehaviour
{
	VRTK.VRTK_InteractableObject interactable;

	[Header( "Variables" )]
	public float waitTimeForStartRecording = 4;
	public float LerpSpeed = 5;
	public float HeadLerpSpeed = 5;
	public float HeadLerpDist = 0.05f;

	[Header( "References" )]
	public FeedbackMetadataProvider feedbackMetadataProvider;
	public VRTheFeedbackManager feedbackManager;
	public AudioSource audioSource;
	public Text infoText;
	public GameObject RecordingIndicator;
	public Transform MicHead;

	[Header( "Assets" )]
	public AudioClip openClip;
	public AudioClip successClip;
	public AudioClip errorClip;
	public AudioClip confirmationClip;

	public bool isGrabbed = false;
	private bool canRecordFeedback = true;
	private bool shouldUploadFeedback = false;
	private float timeGrabbed;
	private float micRecordingTime;

	private Vector3 InitialPos;
	private Quaternion InitialRot;
	private Vector3 InitialHeadScale;
	private int LastBeep = 0;

	void Start()
	{
		interactable = GetComponent<VRTK.VRTK_InteractableObject>();
		interactable.InteractableObjectGrabbed += Interactable_InteractableObjectGrabbed;
		interactable.InteractableObjectUngrabbed += Interactable_InteractableObjectUngrabbed;
		feedbackManager.FeedbackSuccessfullyUploaded += FeedbackManager_FeedbackSuccessfullyUploaded;
		feedbackManager.FeedbackFailedDueToError += FeedbackManager_FeedbackFailedDueToError;

		RecordingIndicator.SetActive( false );
		InitialPos = transform.localPosition;
		InitialRot = transform.localRotation;
		InitialHeadScale = MicHead.localScale;
	}

	public void Update()
	{
		// Mic head animation
		if ( feedbackManager.isRecording )
		{
			float hor = Mathf.Abs( Mathf.Sin( Time.time * HeadLerpSpeed ) ) * HeadLerpDist;
			MicHead.localScale = InitialHeadScale + new Vector3( hor, 0, hor );
		}
		else
		{
			MicHead.localScale = InitialHeadScale;
		}

		// Recording countdown/start logic, text changes
		if ( isGrabbed && canRecordFeedback )
		{
			if ( !feedbackManager.isRecording )
			{
				if ( timeGrabbed + waitTimeForStartRecording < Time.time )
				{
					infoText.text = "Speak now\nRelease to send your feedback!";

					audioSource.clip = openClip;
					audioSource.Play();

					micRecordingTime = Time.time;
					feedbackManager.RecordFeedback();

					shouldUploadFeedback = true;
					canRecordFeedback = false;

					RecordingIndicator.SetActive( true );
				}
				else
				{
					int time = Mathf.CeilToInt( waitTimeForStartRecording + ( timeGrabbed - Time.time ) );
					infoText.text = "Start speaking in " + time.ToString( "0" ) + "...";

					if ( time < LastBeep )
					{
						MyTownQuest.SpawnResourceAudioSource( "beep", transform.position, 0.8f + LastBeep / 20.0f, 0.2f );
						LastBeep = time;
					}
				}
			}
		}

		// Lerp home
		if ( !isGrabbed )
		{
			transform.localPosition = Vector3.Lerp( transform.localPosition, InitialPos, Time.deltaTime * LerpSpeed );
			transform.localRotation = Quaternion.Lerp( transform.localRotation, InitialRot, Time.deltaTime * LerpSpeed );
		}
	}

	private void Interactable_InteractableObjectGrabbed( object sender, VRTK.InteractableObjectEventArgs e )
	{
		isGrabbed = true;
		timeGrabbed = Time.time;
		LastBeep = 5;

		if ( Application.isMobilePlatform && !PermissionHelper.CheckForPermission( PermissionHelper.Permissions.RECORD_AUDIO ) )
		{
			PermissionHelper.RequestPermission( PermissionHelper.Permissions.RECORD_AUDIO );
		}
	}

	private void Interactable_InteractableObjectUngrabbed( object sender, VRTK.InteractableObjectEventArgs e )
	{
		isGrabbed = false;
		if ( shouldUploadFeedback )
		{
			infoText.text = "Uploading feedback...";
			shouldUploadFeedback = false;

			audioSource.clip = confirmationClip;
			audioSource.Play();

			var metadata = feedbackMetadataProvider.GetFeedbackMetadata();
			metadata.Add( "mic-holding-time", ( Time.time - micRecordingTime ).ToString() );
			feedbackManager.SaveFeedback( metadata );
		}
		else
		{
			infoText.text = "Grab me to\nrecord voice feedback";
		}

		RecordingIndicator.SetActive( false );
		transform.position = InitialPos;
	}

	private void FeedbackManager_FeedbackFailedDueToError( object sender, VRTheFeedbackManager.VRTheFeedbackEventArgs e )
	{
		infoText.text = "Something went wrong.\nPlease try again.";

		audioSource.clip = errorClip;
		audioSource.Play();

		canRecordFeedback = true;
	}

	private void FeedbackManager_FeedbackSuccessfullyUploaded( object sender, VRTheFeedbackManager.VRTheFeedbackEventArgs e )
	{
		infoText.text = "Thank you!\nGrab again to record another message.";

		audioSource.clip = successClip;
		audioSource.Play();

		canRecordFeedback = true;
	}
}

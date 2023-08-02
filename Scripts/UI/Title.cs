using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Title : MonoBehaviour, IPointerDownHandler
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private Canvas globalCanvas;
	[SerializeField] private Sprite pressedSprite;
	[SerializeField] private Sprite releasedSprite;
	[SerializeField] private Image buttonSprite;
	[SerializeField] private Image transitionImage;
	[SerializeField] private AudioClip musicAudioClip;
	[SerializeField] private AudioClip clickAudioClip;
	[SerializeField] private AudioClip transitionAudioClip;

	[Header("Settings")]
	[SerializeField] private string newState;

	private string currentState;
	private Animator transitionAnimator;

	void Start()
	{
		// Check if transfer instance exists.
		if (Transfer.scriptInstance != null)
		{
			// Destroy the object and skip the title animation to level selection.
			Destroy(Transfer.scriptInstance.gameObject);
			globalCanvas.GetComponent<Animator>().Play(newState, 0, 1);
		}

		// Play the transition sound as one shot to avoid the sound being overridden.
		AudioManager.scriptInstance.transitionInSource.PlayOneShot(transitionAudioClip);

		// Enable the transition animation object and extract animator component from the transition object, finally play the transition animation once the scene is loaded.
		transitionImage.gameObject.SetActive(true);
		transitionAnimator = transitionImage.GetComponent<Animator>();
		transitionAnimator.Play("Unload");

		// Execute disable function after 1.2 seconds.
		Invoke("Disable", 1.2f);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (currentState != newState)
		{
			// Change the pitch of the sound to 1 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 1;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);

			// Get the animator component in the global canvas object and play the animation.
			globalCanvas.GetComponent<Animator>().Play(newState);
			currentState = newState;
			// Change the sprite of the button.
			buttonSprite.sprite = pressedSprite;

			// Execute disable function after 0.1 second.
			Invoke("Reset", 0.1f);
		}
	}

	void Reset()
	{
		// Change the sprite of the button.
		buttonSprite.sprite = releasedSprite;
	}

	void Disable()
	{
		// Disable the transition object.
		transitionImage.gameObject.SetActive(false);
	}
}

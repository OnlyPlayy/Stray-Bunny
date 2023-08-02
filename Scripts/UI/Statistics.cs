using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Statistics : MonoBehaviour, IPointerDownHandler
{
	// Object variables.
	[Header("Settings")]
	[SerializeField] private string[] planetNames;

	[Header("Objects")]
	[SerializeField] private Canvas statisticsCanvas;
	[SerializeField] private RectTransform backgroundPanel;
	[SerializeField] private Text deathsText;
	[SerializeField] private Text jumpsText;
	[SerializeField] private Text additionalJumpsText;
	[SerializeField] private Text totalStarsText;
	[SerializeField] private AudioClip clickAudioClip;
	[SerializeField] private Sprite pressedSprite;
	[SerializeField] private Sprite releasedSprite;
	[SerializeField] private RectTransform dataButton;
	[SerializeField] private Sprite canDeleteSprite;
	[SerializeField] private Sprite cannotDeleteSprite;

	private bool isOpen, hasFinished = true;
	private Animator canvasAnimator;
	private int collectedStarsCount;

	void Start()
	{
		// Extract animator component from the statistics canvas object.
		canvasAnimator = statisticsCanvas.GetComponent<Animator>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (hasFinished)
		{
			// Execute close function and change the sprite of the button.
			Close(false);

			gameObject.GetComponent<Image>().sprite = pressedSprite;

			// Execute reset sprite function after 0.1 second.
			Invoke("ResetSprite", 0.1f);
		}
	}

	void ResetSprite()
	{
		// Change the sprite of the button.
		gameObject.GetComponent<Image>().sprite = releasedSprite;
	}

	public void Background()
	{
		// Execute close function when background gets pressed.
		Close(true);
	}

	void Close(bool backgroundPress)
	{
		if (isOpen && hasFinished)
		{
			// Change the pitch of the sound to 5 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 5;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);

			// Play the close animtion.
			isOpen = false;
			hasFinished = false;
			canvasAnimator.Play("Close");

			StartCoroutine(AnimatorState());
		}
		else if (!isOpen && !backgroundPress && hasFinished)
		{
			// Change the pitch of the sound to 3 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 3;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);

			// Enable the background animation object and play the open animation.
			backgroundPanel.gameObject.SetActive(true);
			isOpen = true;
			hasFinished = false;
			canvasAnimator.Play("Open");
			collectedStarsCount = 0;

			StartCoroutine(AnimatorState());

			// Get the total amount of collected stars.
			for (int i = 0; i < planetNames.Length; i++)
			{
				collectedStarsCount =+ PlayerPrefs.GetInt(planetNames[i] + "TotalStars");
			}

			// Change text to given values from the saved player prefs.
			deathsText.text = PlayerPrefs.GetInt("Deaths").ToString();
			jumpsText.text = PlayerPrefs.GetInt("Jumps").ToString();
			additionalJumpsText.text = PlayerPrefs.GetInt("Additional").ToString();
			totalStarsText.text = collectedStarsCount.ToString();
		}
	}

	IEnumerator AnimatorState()
	{
		// Check if statistics animation has finished.
		do
		{
			yield return null;
		}
		while (canvasAnimator.GetCurrentAnimatorStateInfo(0).length > canvasAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

		// Wait 0.2 second before continuing.
		yield return new WaitForSeconds(0.2f);
		hasFinished = true;

		if (!isOpen)
		{
			// Disable the blocker image once the statistics menu is closed.
			backgroundPanel.gameObject.SetActive(false);
		}
	}

	public void Delete()
	{
		// If the statistics menu animation has finished and any of the given values exits when pressed, delete all save player prefs.
		if (hasFinished && PlayerPrefs.HasKey("Deaths") || PlayerPrefs.HasKey("Jumps") || PlayerPrefs.HasKey("Additional") || PlayerPrefs.HasKey("TotalStars") || PlayerPrefs.HasKey("MusicMuted") || PlayerPrefs.HasKey("EffectsMuted"))
		{
			// Delete all existing player prefs.
			PlayerPrefs.DeleteAll();
			// Change the pitch of the sound to 1 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 1;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);

			// Play the close animation and begin loading the main menu scene.
			hasFinished = false;
			canvasAnimator.Play("Close");

			GetComponent<Load>().Loading("Main");
		}
	}

	void Update()
	{
		// If any of these values exist, change the sprite of the button to allow the player to completely reset the game.
		if (PlayerPrefs.HasKey("Deaths") || PlayerPrefs.HasKey("Jumps") || PlayerPrefs.HasKey("Additional") || PlayerPrefs.HasKey("TotalStars") || PlayerPrefs.HasKey("MusicMuted") || PlayerPrefs.HasKey("EffectsMuted"))
		{
			dataButton.GetComponent<Image>().sprite = canDeleteSprite;
		}
		else if (hasFinished)
		{
			// Change the sprite of the button after the statistics menu has closed.
			dataButton.GetComponent<Image>().sprite = cannotDeleteSprite;
		}
	}
}

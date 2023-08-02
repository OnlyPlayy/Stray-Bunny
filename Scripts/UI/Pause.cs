using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class Pause : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private RectTransform pauseCanvas;
	[SerializeField] private Text timeTargetText;
	[SerializeField] private Text allowedDeathsText;
	[SerializeField] private AudioClip clickAudioClip;
	[SerializeField] private Button pauseButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Sprite pressedPauseSprite;
	[SerializeField] private Sprite releasedPauseSprite;
	[SerializeField] private Sprite pressedButtonSprite;
	[SerializeField] private Sprite releasedButtonSprite;
	[SerializeField] private Sprite pressedExitSprite;
	[SerializeField] private Sprite releasedExitSprite;
	[SerializeField] private Sprite pressedRestartSprite;
	[SerializeField] private Sprite releasedRestartSprite;

	public static Pause scriptInstance;
	private bool gamePaused;
	private string sceneName;

	void Awake()
	{
		// Create a referencing instance if null.
		if (scriptInstance == null)
		{
			scriptInstance = this;
		}
	}

	void Start()
	{
		// Set time target text.
		timeTargetText.text = "BEAT TIME: 00:00:00";

		// Check if required pass time is less than or equal to 0.
		if (StarsManager.scriptInstance.passTime <= 0)
		{
			// Change the opacity of the text to 10% if true.
			timeTargetText.color = new Color(1, 1, 1, 0.1f);
		}

		// Get the name of the current scene.
		sceneName = SceneManager.GetActiveScene().name;
		// Change text to given values from the script instances.
		timeTargetText.text = "BEAT TIME: " + TimeSpan.FromSeconds(StarsManager.scriptInstance.passTime).ToString("mm':'ss':'ff");
		allowedDeathsText.text = "ALLOWED DEATHS: " + StarsManager.scriptInstance.passDeaths.ToString();
	}

	public void Switch()
	{
		if (gamePaused)
		{
			// Unpause the game by changing the time scale to 1, disable the pause canvas object and set the pause button sprite to released.
			Time.timeScale = 1;
			pauseCanvas.gameObject.SetActive(false);
			pauseButton.GetComponent<Image>().sprite = releasedPauseSprite;

			// Change the pitch of the sound to 5 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 5;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);
		}
		else
		{
			// Pause the game by changing the time scale to 0, enable the pause canvas object and set the pause button sprite to pressed.
			Time.timeScale = 0;
			pauseCanvas.gameObject.SetActive(true);
			pauseButton.GetComponent<Image>().sprite = pressedPauseSprite;

			// Change the pitch of the sound to 3 and play the button click sound as one shot to avoid the sound being overridden.
			AudioManager.scriptInstance.buttonSource.pitch = 3;
			AudioManager.scriptInstance.buttonSource.PlayOneShot(clickAudioClip);
		}

		// Change bool to opposite.
		gamePaused = !gamePaused;
	}

	public void Restart()
	{
		// Change the sprite of the button and the child image of the button.
		restartButton.GetComponent<Image>().sprite = pressedButtonSprite;
		restartButton.transform.GetChild(0).GetComponent<Image>().sprite = pressedRestartSprite;
		// Start loading sequence.
		StartCoroutine("ResetSprite", true);
		GetComponent<Load>().Loading(sceneName);

		// Check if transfer instance exists.
		if (Transfer.scriptInstance != null)
		{
			// Destroy the object to avoid multiple copies of the same script instance.
			Destroy(Transfer.scriptInstance.gameObject);
		}
	}

	public void Exit()
	{
		// Change the sprite of the button and the child image of the button.
		exitButton.GetComponent<Image>().sprite = pressedButtonSprite;
		exitButton.transform.GetChild(0).GetComponent<Image>().sprite = pressedExitSprite;
		// Start loading sequence.
		StartCoroutine("ResetSprite", false);
		GetComponent<Load>().Loading("Main");
	}

	IEnumerator ResetSprite(bool buttonIndex)
	{
		// Wait 0.1 second while ignoring the time scale.
		yield return new WaitForSecondsRealtime(0.1f);

		// Check if the button pressed was the restart or the exit button.
		if (buttonIndex)
		{
			// Change the sprite of the button and the child image of the button.
			restartButton.GetComponent<Image>().sprite = releasedButtonSprite;
			restartButton.transform.GetChild(0).GetComponent<Image>().sprite = releasedRestartSprite;

			// Wait 0.1 second while ignoring the time scale.
			yield return new WaitForSecondsRealtime(0.1f);
		}
		else
		{
			// Change the sprite of the button and the child image of the button.
			exitButton.GetComponent<Image>().sprite = releasedButtonSprite;
			exitButton.transform.GetChild(0).GetComponent<Image>().sprite = releasedExitSprite;
		}
	}
}

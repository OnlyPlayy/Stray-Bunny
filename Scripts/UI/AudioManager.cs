using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	public AudioSource musicSource;
	public AudioSource buttonSource;
	public AudioSource transitionInSource;
	public AudioSource transitionOutSource;
	public AudioSource pickUpSource;
	public AudioSource deathSource;
	public AudioSource walkSource;
	public AudioSource singleJumpSource;
	public AudioSource doubleJumpSource;
	public AudioSource hitSource;
	public AudioSource shootSource;
	public AudioSource breakSource;
	[SerializeField] Button musicButton;
	[SerializeField] Button effectsButton;
	[SerializeField] Sprite musicSprite;
	[SerializeField] Sprite musicMutedSprite;
	[SerializeField] Sprite effectsSprite;
	[SerializeField] Sprite effectsMutedSprite;

	[HideInInspector] public static AudioManager scriptInstance;

	private void Awake()
	{
		// Begin fading in the audio once the scene becomes active.
		StartCoroutine(AudioManager.FadeIn(1));

		// Create a referencing instance if null.
		if (scriptInstance == null)
		{
			scriptInstance = this;
		}
	}

	void Start()
	{
		// Change the button sprite and mute the music source if true.
		if (PlayerPrefs.GetInt("MusicMuted") == 1)
		{
			musicButton.GetComponent<Image>().sprite = musicMutedSprite;
			musicSource.mute = !musicSource.mute;
		}
		else
		{
			musicButton.GetComponent<Image>().sprite = musicSprite;
		}

		// Change the button sprite and mute all sounds related to effects if true.
		if (PlayerPrefs.GetInt("EffectsMuted") == 1)
		{
			effectsButton.GetComponent<Image>().sprite = effectsMutedSprite;
			buttonSource.mute = !buttonSource.mute;
			transitionInSource.mute = !transitionInSource.mute;
			transitionOutSource.mute = !transitionOutSource.mute;
			pickUpSource.mute = !pickUpSource.mute;
			deathSource.mute = !deathSource.mute;
			walkSource.mute = !walkSource.mute;
			singleJumpSource.mute = !singleJumpSource.mute;
			doubleJumpSource.mute = !doubleJumpSource.mute;
			hitSource.mute = !hitSource;
			shootSource.mute = !shootSource.mute;
			breakSource.mute = !breakSource.mute;
		}
		else
		{
			effectsButton.GetComponent<Image>().sprite = effectsSprite;
		}
	}

	public void Music()
	{
		// Save player settings to player prefs.
		if (PlayerPrefs.GetInt("MusicMuted") == 0)
		{
			PlayerPrefs.SetInt("MusicMuted", 1);
		}
		else if (PlayerPrefs.GetInt("MusicMuted") == 1)
		{
			PlayerPrefs.SetInt("MusicMuted", 0);
		}

		// Change bool to opposite.
		musicSource.mute = !musicSource.mute;
	}

	public void Effects()
	{
		// Save player settings to player prefs.
		if (PlayerPrefs.GetInt("EffectsMuted") == 0)
		{
			PlayerPrefs.SetInt("EffectsMuted", 1);
		}
		else if (PlayerPrefs.GetInt("EffectsMuted") == 1)
		{
			PlayerPrefs.SetInt("EffectsMuted", 0);
		}

		// Change bool to opposite.
		buttonSource.mute = !buttonSource.mute;
		transitionInSource.mute = !transitionInSource.mute;
		transitionOutSource.mute = !transitionOutSource.mute;
		pickUpSource.mute = !pickUpSource.mute;
		deathSource.mute = !deathSource.mute;
		walkSource.mute = !walkSource.mute;
		singleJumpSource.mute = !singleJumpSource.mute;
		doubleJumpSource.mute = !doubleJumpSource.mute;
		hitSource.mute = !hitSource.mute;
		shootSource.mute = !shootSource.mute;
		breakSource.mute = !breakSource.mute;
	}

	public static IEnumerator FadeOut(float fadeTime)
	{
		// Get the current audio listener volume.
		float startVolume = AudioListener.volume;

		// Execute the fade out function as long as the audio listener volume is above 0 for the length of the fade time.
		while (AudioListener.volume > 0)
		{
			AudioListener.volume -= startVolume * Time.unscaledDeltaTime / fadeTime;

			yield return null;
		}
	}

	public static IEnumerator FadeIn(float fadeTime)
	{
		// Set the start volume to 0.25 and audio listener volume to 0.
		float startVolume = 0.25f;

		AudioListener.volume = 0;

		// Execute the fade in function as long as the audio listener volume is lower than 1 for the length of the fade time.
		while (AudioListener.volume < 1)
		{
			AudioListener.volume += startVolume * Time.deltaTime / fadeTime;

			yield return null;
		}
	}
}

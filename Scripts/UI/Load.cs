using UnityEngine;

public class Load : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private AudioClip transitionAudioClip;

	public void Loading(string sceneName)
	{
		// Pass the scene name to scene script instance.
		Scene.scriptInstance.Transition(sceneName);
		// Play the transition sound as one shot to avoid the sound being overridden while fading out the audio.
		AudioManager.scriptInstance.transitionOutSource.PlayOneShot(transitionAudioClip);

		StartCoroutine(AudioManager.FadeOut(1));
	}
}

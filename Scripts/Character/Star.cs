using UnityEngine;
using UnityEngine.SceneManagement;

public class Star : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private AudioClip pickUpAudioClip;

	void Start()
	{
		// Get the name of the current scene.
		string sceneName = SceneManager.GetActiveScene().name;

		if (PlayerPrefs.GetInt(sceneName + "StarCollected") == 1)
		{
			// If the star has been collected before, disable it.
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		// If the collided object was a player, play collected animation, assign variables in script instances and play the pick up sound as one shot to avoid the sound being overridden.
		if (collision.gameObject.tag == "Player")
		{
			GetComponent<Collider2D>().enabled = false;
			GetComponent<Animator>().Play("Collected");
			StarsManager.scriptInstance.Star();
			Finish.scriptInstance.starCollected = true;
			AudioManager.scriptInstance.pickUpSource.PlayOneShot(pickUpAudioClip);
		}
	}
}

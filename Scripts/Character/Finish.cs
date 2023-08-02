using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Finish : MonoBehaviour
{
	// Object variables.
	[Header("Settings")]
	[SerializeField] private string planetName;
	[SerializeField] private string nextStage;

	public static Finish scriptInstance;
	[HideInInspector] public bool starCollected;
	private Animator bunnyHoleAnimator, gameplayAnimator;

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
		// Extract animator component from the finish object.
		bunnyHoleAnimator = GetComponent<Animator>();
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		// If the collided object was a player, begin the finish sequence of saving the stage progress and playing the animations.
		if (collision.gameObject.name == "Player")
		{
			// Extract box collider component from the finish object and the name of the current scene.
			GetComponent<BoxCollider2D>().enabled = false;
			string sceneName = SceneManager.GetActiveScene().name;

			if (starCollected)
			{
				PlayerPrefs.SetInt(sceneName + "StarCollected", 1);
			}

			int collectedStars = StarsManager.scriptInstance.Collected();

			if (collectedStars > PlayerPrefs.GetInt(sceneName + "CollectedStars"))
			{
				PlayerPrefs.SetInt(sceneName + "CollectedStars", StarsManager.scriptInstance.collectedStars);
			}

			if (PlayerPrefs.GetInt(sceneName + "Completed") == 0)
			{
				PlayerPrefs.SetInt(sceneName + "Completed", 1);
				PlayerPrefs.SetInt(planetName, PlayerPrefs.GetInt(planetName) + 1);
			}

			GameObject.Find("HUD").GetComponent<Animator>().Play("Death");
			StartCoroutine(AnimatorState());
		}
	}

	IEnumerator AnimatorState()
	{
		// Check if the player character has stopped moving.
		do
		{
			yield return null;
		}
		while (GameObject.Find("Player").GetComponent<Character>().idleTimer <= 0.25f);

		// Start the player character and bunny hole animations.
		bunnyHoleAnimator.Play("Finish");
		GameObject.Find("Player").GetComponent<Character>().ChangeAnimationState("JumpingIn");

		// Check if the bunny hole animation has finished.
		do
		{
			yield return null;
		}
		while (bunnyHoleAnimator.GetCurrentAnimatorStateInfo(0).length - 1.935f > bunnyHoleAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

		// Check if transfer instance exists and destroy it.
		if (Transfer.scriptInstance != null && nextStage != "")
		{
			Destroy(Transfer.scriptInstance.gameObject);
		}

		// If no next scene has been set to load, load main menu scene.
		// Otherwise, load specified scene.
		if (nextStage == "")
		{
			GetComponent<Load>().Loading("Main");
		}
		else
		{
			GetComponent<Load>().Loading(nextStage);
		}
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Scene : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private Image transitionImage;
	public static Scene scriptInstance;

	private Animator transitionAnimator;

	void Awake()
	{
		// Extract animator component from the scene object.
		transitionAnimator = transitionImage.GetComponent<Animator>();

		// Create a referencing instance if null.
		if (scriptInstance == null)
		{
			scriptInstance = this;
		}
	}

	public async void Transition(string sceneName)
	{
		// Disable the scene from showing up when it gets loaded for as long as the transition animation is playing.
		var sceneToLoad = SceneManager.LoadSceneAsync(sceneName);
		sceneToLoad.allowSceneActivation = false;
		// Enable the transition animation object and play the transition animation.
		transitionImage.gameObject.SetActive(true);
		transitionAnimator.Play("Load");

		// Check if transition animation has finished.
		do
		{
			await Task.Delay(1);
			continue;
		}
		while (AnimatorState());

		// Once the animation has finished, enable the scene and set time scale to 1 incase if the game was previously paused.
		sceneToLoad.allowSceneActivation = true;
		Time.timeScale = 1;
	}

	bool AnimatorState()
	{
		// Check if the transition animation is still playing.
		return transitionAnimator.GetCurrentAnimatorStateInfo(0).length > transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}
}

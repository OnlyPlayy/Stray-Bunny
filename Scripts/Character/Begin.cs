using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Begin : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private GameObject bunnyHole;
    [SerializeField] private Image transitionImage;
    [SerializeField] private Canvas gameplayCanvas;
    [SerializeField] private AudioClip transitionAudioClip;

    [Header("Settings")]
    [SerializeField] private float startDelay = 0.5f;

    private Animator bunnyHoleAnimator, transitionAnimator, gameplayAnimator;
    private GameObject playerObject;
    private float moveTimer = 0.55f;

    void Awake()
	{
        // Find object named player.
        playerObject = GameObject.Find("Player");

        if (playerObject != null)
		{
            // If found, disable visibility.
            playerObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

	void Start()
    {
        // Set time scale to 1 incase if the game was previously paused.
        Time.timeScale = 1;
        // Play the transition sound as one shot to avoid the sound being overridden while fading in the audio.
        AudioManager.scriptInstance.transitionInSource.PlayOneShot(transitionAudioClip);

        StartCoroutine(AudioManager.FadeIn(1));
        // Enable the transition animation object and extract animator component from the transition object, finally play the transition animation once the scene loads.
        transitionAnimator = transitionImage.GetComponent<Animator>();
        transitionImage.gameObject.SetActive(true);
        transitionAnimator.Play("Unload");
        // Extract animator components from the objects.
        bunnyHoleAnimator = bunnyHole.GetComponent<Animator>();
        gameplayAnimator = gameplayCanvas.GetComponent<Animator>();

        StartCoroutine(AnimatorState());
    }

    IEnumerator AnimatorState()
	{
        // Check if transition animation has finished.
        do
        {
            yield return null;
        }
        while (transitionAnimator.GetCurrentAnimatorStateInfo(0).length > transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        // Once the animation has finished and the function has waited for a set amount of time, play the start animation.
        yield return new WaitForSeconds(startDelay);
        bunnyHoleAnimator.Play("Start");

        // Check if start animation has finished.
        do
        {
            yield return null;
        }
        while (bunnyHoleAnimator.GetCurrentAnimatorStateInfo(0).length > bunnyHoleAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        // Once the animation has finished, wait for 1 second before continuing.
        yield return new WaitForSeconds(1);

        if (playerObject != null)
		{
            // If player object exists, enable visibility and play the jumping out animation.
            playerObject.GetComponent<SpriteRenderer>().enabled = true;
            playerObject.GetComponent<Character>().ChangeAnimationState("JumpingOut");
		}

        // Move the player object in a set direction for a set amount of time.
        while (moveTimer >= 0)
        {
            if (playerObject != null)
            {
                playerObject.transform.Translate(Vector2.right * Time.deltaTime);
                moveTimer -= Time.deltaTime;
            }

            yield return null;
        }
        // Once the object has finished moving, wait for 0.25 second before continuing and playing the gameplay animation.
        yield return new WaitForSeconds(0.25f);
        gameplayAnimator.Play("Gameplay");
        // Wait 1 second before continuing.
        yield return new WaitForSeconds(1);

        if (playerObject != null)
		{
            // If player object exists, set idle timer to 0 and start playing the idle animation.
            playerObject.GetComponent<Character>().idleTimer = 0;
            playerObject.GetComponent<Character>().ChangeAnimationState("Idle");
            // Call begin function in canvas object to start the stage timer.
            StarsManager.scriptInstance.Begin();
        }
        // Wait for 0.25 second before finally disabling the transition animation object.
        yield return new WaitForSeconds(0.25f);
        transitionImage.gameObject.SetActive(false);
    }
}

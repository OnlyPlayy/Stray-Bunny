using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class StarsManager : MonoBehaviour
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private GameObject[] starObjects;
	[SerializeField] private Sprite collectedStar;
	[SerializeField] private Sprite emptyStar;
	[SerializeField] private Text stageTimerText;
	[SerializeField] private Text deathCounterText;

	[Header("Settings")]
	public float passTime;
	public int passDeaths;

	[HideInInspector] public static StarsManager scriptInstance;
	[HideInInspector] public bool hasStarted;
	[HideInInspector] public int collectedStars;
	private TimeSpan timePlaying;
	private bool isPlaying, targetFailed;
	private float elapsedTime;

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
		if (Transfer.scriptInstance.deathsCounter > passDeaths && !targetFailed)
		{
			// If the player died more than set, remove one star and change the deaths counter text opacity to 10%.
			deathCounterText.color = new Color(1, 1, 1, 0.1f);
			targetFailed = true;
			Lost();
		}

		// Change text to given values from the script instances.
		deathCounterText.text = Transfer.scriptInstance.deathsCounter.ToString();
		stageTimerText.text = "00:00:00";

		if (passTime <= 0)
		{
			// Change stage timer text opacity to 10% if the pass time has not been set.
			stageTimerText.color = new Color(1, 1, 1, 0.1f);
		}
	}

	public void Begin()
	{
		if (passTime > 0)
		{
			// Start the stage timer if the pass time has been set.
			isPlaying = true;
			elapsedTime = 0;
			StartCoroutine(Timer());
		}
	}

	public void Stop()
	{
		// Stop the stage timer.
		isPlaying = false;
	}

	IEnumerator Timer()
	{
		// Start the timer and convert the time to string.
		while (isPlaying)
		{
			elapsedTime += Time.deltaTime;
			timePlaying = TimeSpan.FromSeconds(elapsedTime);
			string timerTime = timePlaying.ToString("mm':'ss':'ff");
			stageTimerText.text = timerTime;

			// If stage timer runs over, change the text opacity to 10% and remove one star.
			if (elapsedTime >= passTime)
			{
				isPlaying = false;
				stageTimerText.color = new Color(1, 1, 1, 0.1f);
				Lost();
			}

			yield return null;
		}
	}

	public void Star()
	{
		for (int i = 0; starObjects.Length > 0; i++)
		{
			// Look for the first empty star object when the player collects the star.
			if (starObjects[i].GetComponent<Image>().sprite == emptyStar)
			{
				starObjects[i].GetComponent<Image>().sprite = collectedStar;
				break;
			}
		}
	}

	public void Lost()
	{
		for (int i = starObjects.Length; i > 0; i--)
		{
			// Look for the first collected star object when the player loses a star.
			if (starObjects[i - 1].GetComponent<Image>().sprite == collectedStar)
			{
				starObjects[i - 1].GetComponent<Image>().sprite = emptyStar;
				break;
			}
		}
	}

	public int Collected()
	{
		for (int i = starObjects.Length; i > 0; i--)
		{
			// Return the number of collected stars when the player reaches the finish.
			if (starObjects[i - 1].GetComponent<Image>().sprite == collectedStar)
			{
				collectedStars++;
			}
		}

		return collectedStars;
	}
}

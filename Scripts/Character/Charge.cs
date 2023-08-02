using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Charge : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
	// Object variables.
	[Header("Objects")]
	[SerializeField] private Sprite pressedSprite;
	[SerializeField] private Sprite releasedSprite;

	[Header("Settings")]
	[SerializeField] private float fullJump = 20;
	[SerializeField] private float normalJump = 10;

	private GameObject playerCharacter;
    private bool canJump, isCharging;
    private float jumpHeight, chargeTimer, materialOpacity;
	private Transform chargeBar;

	public void OnPointerDown(PointerEventData eventData)
	{
		// Find a player object.
		playerCharacter = GameObject.FindGameObjectWithTag("Player");

		if (playerCharacter.GetComponent<Character>().additionalJumps != playerCharacter.GetComponent<Character>().jumpCount)
		{
			// Change the sprite of the button and start charging if the player character can jump.
			// Otherwise, double jump.
			if (playerCharacter.GetComponent<Character>().canJump)
			{
				isCharging = true;
				canJump = true;
				GetComponent<Image>().sprite = pressedSprite;
			}
			else
			{
				canJump = true;
				GetComponent<Image>().sprite = pressedSprite;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		// Execute stop function and set the opacity of the charging bar to 0%.
		Stop();
		materialOpacity = 0;
	}

    void Stop()
	{
		// Change the sprite of the button and disable to charging bar object.
		isCharging = false;
		GetComponent<Image>().sprite = releasedSprite;
		chargeBar.gameObject.SetActive(false);

		if (canJump)
		{
			// Check for how long the button was held down.
			// If more than 1 second, full jump.
			// If less than 0.25 second, normal jump.
			// Anything in between, jump by a set amount.
			if (chargeTimer >= 1)
			{
				if (playerCharacter != null)
				{
					playerCharacter.GetComponent<Character>().Up(fullJump);
					canJump = false;
				}

				chargeTimer = 0;
			}
			else if (chargeTimer <= 0.25)
			{
				if (playerCharacter != null)
				{
					playerCharacter.GetComponent<Character>().Up(normalJump);
					canJump = false;
				}

				chargeTimer = 0;
			}
			else
			{
				if (playerCharacter != null)
				{
					playerCharacter.GetComponent<Character>().Up(jumpHeight);
					canJump = false;
				}

				chargeTimer = 0;
			}
		}
	}

	void Start()
	{
		// Get the second child object in player object.
		chargeBar = GameObject.Find("Player").transform.GetChild(1);
	}

    void Update()
	{
		if (isCharging)
		{
			// When charge button is being held down, start charging.
			chargeTimer += Time.deltaTime % 60;
			jumpHeight = Mathf.Lerp(10, 20, chargeTimer);
			// Get the second child of the charge bar object and change the fill amount of the charge bar child.
			chargeBar.transform.GetChild(1).localScale = new Vector2(chargeTimer, 1);

			// If the charge timer is equal to or above 0.25, start changing the opacity of the children of the charge bar object.
			if (chargeTimer >= 0.25f)
			{
				materialOpacity += Mathf.Lerp(0, 1, Time.deltaTime);
				chargeBar.gameObject.SetActive(true);
				chargeBar.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, materialOpacity);
				chargeBar.gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, materialOpacity);
			}

			// Once the charge timer reaches one, full jump and set the opacity of the charge bar to 0%.
			if (chargeTimer >= 1)
			{
				Stop();
				materialOpacity = 0;
			}
		}

		// When player jumps and presses the button again and holds it down, start charging again after landing.
		// When pressed again while in the air, double jump.
		if (canJump && playerCharacter.GetComponent<Character>().canJump)
		{
			isCharging = true;
		}
		else
		{
			isCharging = false;
			chargeTimer = 0;
			materialOpacity = 0;
			chargeBar.gameObject.SetActive(false);
		}
	}
}

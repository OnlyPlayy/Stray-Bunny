using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Direction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite releasedSprite;

    [Header("Settings")]
    [SerializeField] private bool facingRight = true;
    private GameObject playerCharacter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if the button is held down with the left mouse button or one finger at least.
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
		{
            // Find a player object.
            playerCharacter = GameObject.FindGameObjectWithTag("Player");

            if (playerCharacter != null)
            {
                // Change the sprite of the button.
                GetComponent<Image>().sprite = pressedSprite;

                // Check in which direction the player should move.
                if (facingRight)
                {
                    Right();
                }
                else
                {
                    Left();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Change the sprite of the button.
        GetComponent<Image>().sprite = releasedSprite;

        if (playerCharacter != null)
        {
            // Call stop function in player object when the button is not being held down.
            playerCharacter.GetComponent<Character>().Stop();
        }
    }

    void Left()
    {
        if (playerCharacter != null)
        {
            // Call left function in player object to move the character left.
            playerCharacter.GetComponent<Character>().Left();
        }
    }

    void Right()
    {
        if (playerCharacter != null)
        {
            // Call right function in player object to move the character right.
            playerCharacter.GetComponent<Character>().Right();
        }
    }
}

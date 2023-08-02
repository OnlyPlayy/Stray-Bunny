using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DoubleButton : MonoBehaviour, IPointerDownHandler
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite releasedSprite;
    [SerializeField] private Sprite pressedSecondSprite;
    [SerializeField] private Sprite releasedSecondSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        // If pointer is being held down, check which sprite is currently set.
		if (GetComponent<Image>().sprite == releasedSprite)
		{
            // Change the sprite of the button and reset it by activating coroutine.
            GetComponent<Image>().sprite = pressedSprite;
            StartCoroutine("ResetSprite");
        }
        else
		{
            // Change the sprite of the button and reset it by activating coroutine.
            GetComponent<Image>().sprite = pressedSecondSprite;
            StartCoroutine("ResetSprite");
        }
	}

    IEnumerator ResetSprite()
    {
        // Wait 0.1 second while ignoring the time scale.
        yield return new WaitForSecondsRealtime(0.1f);

        // Check which sprite is currently set.
        if (GetComponent<Image>().sprite == pressedSprite)
        {
            // Change the sprite of the button.
            GetComponent<Image>().sprite = releasedSecondSprite;
        }
        else
        {
            // Change the sprite of the button.
            GetComponent<Image>().sprite = releasedSprite;
        }
    }
}

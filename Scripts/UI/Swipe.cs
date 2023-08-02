using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
	// Object variables.
	[Header("Settings")]
	[SerializeField] [Range(1, 10)] private int panelCount = 1;
	[SerializeField] [Range(0, 500)] private int panelOffset;
	[SerializeField] [Range(0, 20)] private float snapSpeed;
	[SerializeField] [Range(0, 50)] private float scaleOffset;
	[SerializeField] [Range(0, 50)] private float scaleSpeed;
	[SerializeField] private string[] planetNamesText;
	[SerializeField] private string[] planetNames;
	[SerializeField] private int[] planetStages;

	[Header("Objects")]
	[SerializeField] private GameObject selectionObject;
	[SerializeField] private ScrollRect scrollView;

	private GameObject[] panelObjects;
	private Vector2[] panelPosition, panelScale;
	private RectTransform contentRect;
	private Vector2 contentVector;
	private int selectedPanelID;
	private bool isScrolling;

	void Awake()
	{
		// Check if player prefs key exists.
		if (!PlayerPrefs.HasKey("KittyLand"))
		{
			PlayerPrefs.SetInt("KittyLand", 1);
		}
	}

	void Start()
	{
		// Assign variables based on given values.
		contentRect = GetComponent<RectTransform>();
		panelObjects = new GameObject[panelCount];
		panelPosition = new Vector2[panelCount];
		panelScale = new Vector2[panelCount];

		for (int i = 0; i < panelCount; i++)
		{
			// Create a given amount of planet prefabs and transfer variables from given values.
			panelObjects[i] = Instantiate(selectionObject, transform, false);
			panelObjects[i].GetComponent<Planet>().planetText.text = planetNamesText[i];
			panelObjects[i].GetComponent<Planet>().stagesName = planetNames[i];
			panelObjects[i].GetComponent<Planet>().completedStages = PlayerPrefs.GetInt(planetNames[i]);
			panelObjects[i].GetComponent<Planet>().stagesCount = planetStages[i];
			panelObjects[i].name = planetNames[i];

			// Get the position of the current panel object in the scroll view.
			if (i == 0)
			{
				continue;
			}

			panelObjects[i].transform.localPosition = new Vector2(panelObjects[i - 1].transform.localPosition.x + selectionObject.GetComponent<RectTransform>().sizeDelta.x + panelOffset, panelObjects[i].transform.localPosition.y);
			panelPosition[i] = -panelObjects[i].transform.localPosition;
		}
	}

	void LateUpdate()
	{
		// If not scrolling and is further than or closer than position of the panel object, disable smooth drag.
		if (contentRect.anchoredPosition.x >= panelPosition[0].x && !isScrolling || contentRect.anchoredPosition.x <= panelPosition[panelPosition.Length - 1].x && !isScrolling)
		{
			scrollView.inertia = false;
		}

		float nearestPosition = float.MaxValue;

		for (int i = 0; i < panelCount; i++)
		{
			float panelDistance = Mathf.Abs(contentRect.anchoredPosition.x - panelPosition[i].x);

			// Change panel ID after certain point is crossed for centering the panel on the scroll view.
			if (panelDistance < nearestPosition)
			{
				nearestPosition = panelDistance;
				selectedPanelID = i;
			}

			float currentScale = Mathf.Clamp(1 / (panelDistance / 30) * scaleOffset, 0.5f, 1);
			panelScale[i].x = Mathf.SmoothStep(panelObjects[i].transform.localScale.x, currentScale, scaleSpeed * Time.fixedDeltaTime);
			panelScale[i].y = Mathf.SmoothStep(panelObjects[i].transform.localScale.y, currentScale, scaleSpeed * Time.fixedDeltaTime);
			panelObjects[i].transform.localScale = panelScale[i];

			// Change the panel opacity based on the distance to the currently displayed panel.
			float currentOpacity = Mathf.Clamp(1 / (panelDistance / 30) * scaleOffset, 0.25f, 1);
			panelObjects[i].GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(panelObjects[i].GetComponent<CanvasGroup>().alpha, currentOpacity, scaleSpeed * Time.fixedDeltaTime);
		}

		// Get the current drag velocity.
		float scrollVelocity = Mathf.Abs(scrollView.velocity.x);

		if (scrollVelocity < 5 && !isScrolling)
		{
			// Disable smooth drag when not scrolling and velocity is lower than 5.
			scrollView.inertia = false;
		}

		if (panelObjects[selectedPanelID].GetComponent<CanvasGroup>().alpha < 0.99f)
		{
			// Play switch animation when the opacity of the panel is almost 100%.
			panelObjects[selectedPanelID].GetComponent<Animator>().Play("Switch", -1, 0);
		}

		if (isScrolling || scrollVelocity > 5)
		{
			return;
		}

		// Move the panel object to the center of the screen at a set speed when not scrolling and velocity is lower than 5.
		contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, panelPosition[selectedPanelID].x, snapSpeed * Time.fixedDeltaTime);
		contentRect.anchoredPosition = contentVector;
	}

	public void Scrolling(bool scrollingSelection)
	{
		// If scrolling, enable smooth drag.
		isScrolling = scrollingSelection;

		if (scrollingSelection)
		{
			scrollView.inertia = true;
		}
	}
}

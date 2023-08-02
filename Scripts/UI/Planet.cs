using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    public Text planetText;
    public Text collectedStars;
    [SerializeField] private string[] planetStages;
    [SerializeField] private Text completedPercentage;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Button stageOne;
    [SerializeField] private Button stageTwo;
    [SerializeField] private Button stageThree;
    [SerializeField] private Button stageFour;
    [SerializeField] private Button stageFive;
    [SerializeField] private Button stageSix;
    [SerializeField] private GameObject[] starObjects;
    [SerializeField] private Sprite collectedStar;

    [HideInInspector] public string stagesName;
    [HideInInspector] public int completedStages;
    [HideInInspector] public int stagesCount;
    private Button[] stageButton = new Button[6];
    private int collectedStarsCount;

	void Start()
    {
        // Check if completed stages is bigger than 0.
        if (completedStages > 0)
		{
            // Set the planet completion percentage.
            completedPercentage.text = ((completedStages - 1) * 100 / 6) + "%";
        }

		for (int i = 0; i < planetStages.Length; i++)
		{
            // Get the amount of total collected stars for the given planet.
            collectedStarsCount += PlayerPrefs.GetInt(planetStages[i] + "CollectedStars");
		}

        // Assign buttons to index.
        stageButton[0] = stageOne;
        stageButton[1] = stageTwo;
        stageButton[2] = stageThree;
        stageButton[3] = stageFour;
        stageButton[4] = stageFive;
        stageButton[5] = stageSix;

        for (int i = 0; i < completedStages && i < stagesCount; i++)
        {
            // Enable stage button for each completed stage.
            stageButton[i].interactable = true;
            stageButton[i].GetComponent<Image>().sprite = unlockedSprite;
            stageButton[i].transform.GetChild(0).GetComponent<Text>().gameObject.SetActive(true);
            stageButton[i].transform.GetChild(2).GetComponent<RectTransform>().gameObject.SetActive(true);
        }

        if (completedStages != stagesCount + 1 && completedStages != 0)
        {
            // Check if completed stages is not equal to 7 and not equal to 0 before enabling the first stage button.
            stageButton[completedStages - 1].transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);
        }

        for (int i = 0; i < starObjects.Length;  i++)
		{
            // Get the length of all star holders.
            GameObject starsHolder = starObjects[i];

            for (int x = 0; x < PlayerPrefs.GetInt(planetStages[i] + "CollectedStars"); x++)
			{
                // Change the sprite of the star child for each collected star in the stars holder.
                starsHolder.transform.GetChild(x).GetComponent<Image>().sprite = collectedStar;
			}
		}

        if (stageOne.interactable == true)
        {
            // Set the amount of total collected stars.
            collectedStars.text = collectedStarsCount + "/18";
        }

        PlayerPrefs.SetInt(stagesName + "TotalStars", collectedStarsCount);
    }
}

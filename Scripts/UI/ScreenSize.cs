using UnityEngine;

public class ScreenSize : MonoBehaviour
{
    // Object variables.
    [Header("Objects")]
    [SerializeField] private Canvas displayedCanvas;

    private RectTransform safeScreenSize;
    private Rect currentSafeArea = new Rect();
    private ScreenOrientation currentScreenOrientation = ScreenOrientation.AutoRotation;

    void Start()
    {
        // Get the current panel size, screen orientation and safe area display size.
        safeScreenSize = GetComponent<RectTransform>();
        currentScreenOrientation = Screen.orientation;
        currentSafeArea = Screen.safeArea;
        UpdateScreenSize();
    }

    void UpdateScreenSize()
	{
        // If no safe area exists, return.
        if (safeScreenSize == null)
		{
            return;
		}

        // Get the current display size safe area.
        Rect screenSize = Screen.safeArea;
        Vector2 anchorMin = screenSize.position;
        Vector2 anchorMax = screenSize.position + screenSize.size;

        // Update values.
        anchorMin.x /= displayedCanvas.pixelRect.width;
        anchorMin.y /= displayedCanvas.pixelRect.height;
        anchorMax.x /= displayedCanvas.pixelRect.width;
        anchorMax.y /= displayedCanvas.pixelRect.height;
    
        safeScreenSize.anchorMin = anchorMin;
        safeScreenSize.anchorMax = anchorMax;

        currentScreenOrientation = Screen.orientation;
        currentSafeArea = Screen.safeArea;
	}

	void Update()
	{
        // If any of these values change, update the canvas safe area size.
		if ((currentScreenOrientation != Screen.orientation) || (currentSafeArea != Screen.safeArea))
		{
            UpdateScreenSize();
		}
	}
}

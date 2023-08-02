using UnityEngine;

public class Transfer : MonoBehaviour
{
	// Object variables.
	public static Transfer scriptInstance;
	[HideInInspector] public int deathsCounter;

	void Awake()
	{
		// Create a referencing instance if null.
		if (scriptInstance == null)
		{
			// Create an object which will be persistent across scenes.
			scriptInstance = this;
			DontDestroyOnLoad(scriptInstance);
		}
	}
}

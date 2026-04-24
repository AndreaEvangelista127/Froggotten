using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private int totalLevels = 8;
    [SerializeField] private string[] levelSceneNames;

    private const string UnlockKeyPrefix = "Level_Unlocked_";

    private int _currentLevelIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this) // Ensure only one instance of LevelManager exists
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerPrefs.DeleteAll();
        UnlockLevel(0); // Ensure the tutorial level is unlocked by default
    }

    public void UnlockNextLevel()
    {
        UnlockLevel(_currentLevelIndex + 1);
    }

    public void LoadLevel(int levelIndex)
    {
        {
            //Saving the current level index to keep track of which level is currently being played, which can be useful for unlocking the next level after completion
            _currentLevelIndex = levelIndex;
            // Using the list of scene instead of loading the scene by index to avoid issues if the build settings change
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= totalLevels)
        {
            Debug.LogError($"Level index {levelIndex} is out of range. Valid range is 0 to {totalLevels - 1}.");
            return false;
        }
        return PlayerPrefs.GetInt(UnlockKeyPrefix + levelIndex, 0) == 1; // If the level exists and is unlocked, it will return 1, otherwise it defaults to 0 (locked)
    }

    public void UnlockLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= totalLevels)
        {
            Debug.LogError($"Level index {levelIndex} is out of range. Valid range is 0 to {totalLevels - 1}.");
            return;
        }
        PlayerPrefs.SetInt(UnlockKeyPrefix + levelIndex, 1); // Mark the level as unlocked by setting it to 1
        PlayerPrefs.Save(); // Save the changes to PlayerPrefs
    }

}

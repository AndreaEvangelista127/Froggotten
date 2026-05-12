using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons; // Array of buttons corresponding to each level
    [SerializeField] private Sprite lockedSprite; // Sprite to indicate a locked level

    private Sprite[] _originalSprites; // Array to store the original sprites of the buttons for unlocked levels

    private void Awake()
    {
       if( levelButtons == null)
        {
            Debug.Log("Level buttons not assigned in the inspector.");
            return;
        }

        // Store the original sprites of the buttons to restore them when levels are unlocked
        _originalSprites = new Sprite[levelButtons.Length];
        for (int i = 0; i < levelButtons.Length; i++)
        {
            Image buttonImage = levelButtons[i].GetComponent<Image>();
            _originalSprites[i] = buttonImage.sprite;
        }
    }

    // This method is called when the LevelSelectUI GameObject is enabled, ensuring that the buttons are refreshed every time the level selection screen is shown.
    private void OnEnable()
    {
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i; // Starting from 0 for the tutorial level, and incrementing for each subsequent level

            // Check if the level is unlocked using the LevelManager
            bool unlocked = LevelManager.Instance.IsLevelUnlocked(levelIndex);

            // Update the button's appearance and interactivity based on whether the level is unlocked
            Image buttonImage = levelButtons[i].GetComponent<Image>();

            /*Without this line the transition checkbox in the button component  with animation will cause the button to show the unlocked sprite even if the
            level is locked, because the animation will override the sprite change.
            By disabling the animator when the level is locked, we ensure that the button shows the correct locked sprite.*/
            if (levelButtons[i].TryGetComponent<Animator>(out Animator anim))
                anim.enabled = unlocked;

            if (unlocked)
            {
                buttonImage.sprite = _originalSprites[i];

            }
            else
            {
                buttonImage.sprite = lockedSprite;

            }

            levelButtons[i].interactable = unlocked;
        }
    }

    //When the player clicks on a level button, this method is called with the corresponding level index, and it instructs the LevelManager to load the selected level.
    public void LoadLevel(int levelIndex)
    {
        LevelManager.Instance.LoadLevel(levelIndex);
    }
}

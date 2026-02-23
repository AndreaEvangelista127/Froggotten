using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUi : MonoBehaviour
{
    [Header("Heart Sprites")]
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _halfHeart;
    [SerializeField] private Sprite _emptyHeart;

    [Header("Heart Prefab")]
    [SerializeField] private GameObject _heartPrefab;

    private List<Image> _heartImages = new List<Image>();

    /// <summary>
    /// Instantiates the correct number of heart icons based on max health and sets their initial sprites.
    /// Each full heart represents 1.0 HP; a remainder of 0.5 adds an extra half-heart slot.
    /// </summary>
    /// <param name="maxHealth">The player's maximum health value.</param>
    public void InitializeHealthUi(float maxHealth)
    {
        if (_heartPrefab == null)
        {
            Debug.LogWarning("HealthUi: Heart prefab not assigned!");
            return;
        }

        int numberOfHearts = (int)maxHealth;  // here we store only the integer part

        // if there is a half heart needed
        if (maxHealth % 1 != 0)
        {
            numberOfHearts = numberOfHearts + 1; // add one more heart for the half
        }

        // Istanciate hearts
        for (int i = 0; i < numberOfHearts; i++)
        {
            GameObject heart = Instantiate(_heartPrefab, transform);
            Image heartImage = heart.GetComponent<Image>();
            if (heartImage != null)
                _heartImages.Add(heartImage);
            else
                Debug.LogWarning("HealthUi: Heart prefab is missing an Image component!");
        }

        UpdateHeartContainer(maxHealth);
    }

    /// <summary>
    /// Updates each heart sprite to reflect the player's current health.
    /// Full hearts are shown first, followed by a half heart if needed, then empty hearts.
    /// </summary>
    /// <param name="currentHealth">The player's current health value.</param>
    public void UpdateHeartContainer(float currentHealth)
    {
        int fullHearts = (int)currentHealth;  

        bool hasHalfHeart = false;
        if (currentHealth % 1 != 0)
        {
            hasHalfHeart = true;
        }

        // Update every heart
        for (int i = 0; i < _heartImages.Count; i++)
        {
            if (i < fullHearts) // if the player still has full hearts
            {
                _heartImages[i].sprite = _fullHeart;
            }
            else if (i == fullHearts && hasHalfHeart == true) //As soon as u finish checking the full hearts, check if there is a half heart
            {
                _heartImages[i].sprite = _halfHeart;
            }
            else
            {
                _heartImages[i].sprite = _emptyHeart; //otherwise empty heart
            }
        }
    }
}

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


    private void Start()
    {

    }

    public void InitializeHealthUi(float maxHealth)
    {

        // Ogni cuore pieno = 1.0, mezzo cuore = 0.5
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
            _heartImages.Add(heartImage);
        }

        UpdateHeartContainer(maxHealth);
    }

    public void UpdateHeartContainer(float currentHealth)
    {
        int fullHearts = (int)currentHealth;  

        bool hasHalfHeart = false;
        if (currentHealth % 1 != 0)
        {
            hasHalfHeart = true;
        }

        // Aggiorna ogni cuore
        for (int i = 0; i < _heartImages.Count; i++)
        {
            if (i < fullHearts) // if the player still has full hearts
            {
                // Cuore pieno
                _heartImages[i].sprite = _fullHeart;
            }
            else if (i == fullHearts && hasHalfHeart == true) //As soon as u finish checking the full hearts, check if there is a half heart
            {
                // Mezzo cuore
                _heartImages[i].sprite = _halfHeart;
            }
            else
            {
                // Cuore vuoto
                _heartImages[i].sprite = _emptyHeart; //otherwise empty heart
            }
        }
    }
}

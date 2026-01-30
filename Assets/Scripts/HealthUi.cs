
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUi : MonoBehaviour
{
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _halfHeart;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private GameObject _heartPrefab;

    private List<Image> _heartImages = new List<Image>();
    
    

    private void Start()
    {
        
    }

    public void InitializeHealthUi(int maxHealth)
    {
        _heartImages = new List<Image>();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(_heartPrefab, gameObject.transform); // Spawning the heart in the parent position and then pushing the others with Horizontal Layout Group
            if(heart.TryGetComponent(out Image heartImage))
            {
                _heartImages.Add(heartImage);
            }

        }
    }

    public void UpdateHeartContainer(float currentHealth)
    {
        int maxHealth = _heartImages.Count;

        int fullHeart = (int)currentHealth;

        bool hasHalf = false;

        if (currentHealth % 2 > 0)
        { // we are in odd so half heart
            hasHalf = true;
        }

        for (int i = 0; i < maxHealth; i ++)
        {
            for(int j = 0;  j < fullHeart; j++)
            {
                _heartImages[j].sprite = _fullHeart;
                i++;

            }
            if (hasHalf)
            {
                _heartImages[i].sprite = _halfHeart;

            }
        }






    }
}

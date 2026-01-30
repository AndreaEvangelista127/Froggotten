using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private float _currHealth;

    [SerializeField] public HealthUi healthUi;

    private int _totalHits = 0;



    private void Awake()
    {
        _currHealth = _maxHealth;
        
    }

    public void Start()
    {
        if (healthUi != null)
        {
            healthUi.InitializeHealthUi(_maxHealth);
        }
    }

    private void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _currHealth -= damage;
        _totalHits++;

        if(_currHealth < 0)
        {
            _currHealth = 0;
        }

        //UpdateHeartContainer();
    }

    public void PlayerIsDead()
    {
        Debug.Log("ur dead bro");
    }
}

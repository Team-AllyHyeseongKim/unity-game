using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{

    public UnityAction onDie;
    public UnityAction<float, GameObject> onDamaged;


    float maxHealth = 10f;
    public float currentHealth { get; private set; }
    float damage = 3f;
    bool m_IsDead = false;
    
   


    public void TakeDamage(GameObject damageSource)
    {
        float healthBefore = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);


        // call OnDamage action
        float trueDamageAmount = healthBefore - currentHealth;
        if (trueDamageAmount > 0f && onDamaged != null)
        {
            onDamaged.Invoke(trueDamageAmount, damageSource);
        }


        HandleDeath();
    }

    private void HandleDeath()
    {
        if (m_IsDead)
            return;

        // call OnDie action
        if (currentHealth <= 0f)
        {
            Debug.Log("으앙 듀금");
            if (onDie != null)
            {
                m_IsDead = true;
                onDie.Invoke();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    [Header("受伤无敌")]
    [Tooltip("无敌帧时间")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    [Tooltip("无敌状态判定")]
    public bool invulnerable;

    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <= 0)
            invulnerable = false;
        }
    }

    public void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
        return;
        currentHealth -= attacker.damage;
        if (currentHealth >= 0)
        {
            TriggerInvulnerable();
        }
        else
        {
            //执行死亡事件
            currentHealth = 0;
        }
    }
    private void TriggerInvulnerable()
    {
        if(!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}

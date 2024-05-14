using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Unity.VisualScripting.Member;

public class PlayerStats : MonoBehaviour
{
    // Combat Stats
    [Header("Combat Stats")]
    public float baseHp = 0;
    public float maxHp = 0;
    public float currentHp = 0;
    public float baseAttack = 0;
    public float attack = 0;
    public float atkSpd = 1.0f;
    public float baseDefense = 0;
    public float defense = 0;
    public float critRate = 5.0f;
    public float critDmg = 150.0f;
    public float staggerDmg = 110f;
    public float speed = 1.0f;   
    public float hpRecovery = 1f;
    public float cdReduction = 0f;

    // Bonus Stats
    [Header("Bonus Stats")]
    public float atkMulti = 0f;
    public float speedMulti = 0f;
    public float atkSpeedMulti = 0f;
    public float defenseSpeedMulti = 0f;
    public float critRateMulti = 0f;
    public float hpMulti = 0f;
    public float critDmgMulti = 0f;  
    public float hpRecoveryMulti = 0f;
    
    // Level Stats
    [Header("Level Stats")]
    public float lvl = 1;
    public float currentExp = 0;  
    public float maxExp;

    // Variables
    [Header("Variables")]
    private bool levelUp = false;
    private float prevHp;
    public float timeSince = 0;
    public Skills playerSkills;
    public float hpRecCd = 3f;
    public float hpRecCdCur = 0;
    public float hitIndicator = 0f;
    private Color baseColor;

    // Upgrade Variables
    private System.Random random = new System.Random();

    // PlayTime
    [Header("PlayTime")]
    public float playTime = 0.0f;

    // Sound
    [Header("Sound")]
    public AudioSource source;
    public AudioClip clip;

    // Popup
    [SerializeField] private DamagePopup damagePopup;

    // Spawner & Boss Objects
    public GameObject bossObj;

    public float timeScaleMod = 1.0f;

    void Start()
    {
        maxExp = lvl * 100;

        maxHp = (int)(100f * Mathf.Pow(1.1f, lvl - 1));
        currentHp = maxHp;
        prevHp = currentHp;
        baseHp = maxHp;

        attack = (int)(20f * Mathf.Pow(1.1f, lvl - 1));
        baseAttack= attack;
        defense = (int)(10f * (int)Mathf.Pow(1.1f, lvl - 1));
        baseDefense= defense;

        baseColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        playTime += Time.deltaTime;
        HandleHitIndicator();
    }
    void FixedUpdate()
    {
        timeSince += Time.deltaTime;
        hpRecCdCur += Time.deltaTime;
        if (prevHp != currentHp)
        {
            timeSince = 0;
            prevHp = currentHp;
        }
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        if (hpRecCdCur >= hpRecCd && currentHp < maxHp)
        {
            currentHp += hpRecovery;
            hpRecCdCur= 0;
        }
    }
    private void HandleHitIndicator()
    {
        if (hitIndicator > 0)
        {
            hitIndicator -= Time.deltaTime;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = baseColor;
        }
    }

    public void IncreaseExp(float exp)
    {
        currentExp += exp;
    }
    public void LevelUp()
    {
        lvl++;
        currentExp = currentExp - maxExp;
        maxExp = Mathf.Pow(lvl, 1.35f) * 100f;
        IncreaseStats(lvl);
    }
    public void IncreaseStats(float lvl_)
    {        
        maxHp += (int)(baseHp * 0.1f);
        baseHp *= 1.1f;
        attack += (int)(baseAttack * 0.1f);
        baseAttack *= 1.1f;
        defense += (int)(baseDefense * 0.1f);
        baseDefense *= 1.1f;
        currentHp = maxHp;
    }
    public void TakeDamage(int damage, bool isCrit)
    {
        if (damage < 1)
            damage = 1;
        source.Stop();
        currentHp -= damage;
        damagePopup.isPlayer = true;
        damagePopup.Create(transform.position, 1f, damage, isCrit);
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        hitIndicator = 0.15f;
        source.PlayOneShot(clip);
    }
}

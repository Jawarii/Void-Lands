using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    // Combat Stats
    [Header("Combat Stats")]
    public float maxHp = 100;
    public float hp = 100;
    public float attack = 10;
    public float defense = 10;
    public float critRate = 5.0f;
    public float critDmg = 150.0f;
    public float staggerDmgMulti = 110f;
    public float speedMulti = 0f;
    public float atkSpeedMulti = 0f;

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
        prevHp = hp;
        maxExp = lvl * 6;
    }

    void Update()
    {
        playTime += Time.deltaTime;
    }
    void FixedUpdate()
    {
        timeSince += Time.deltaTime;
        hpRecCdCur += Time.deltaTime;
        if (prevHp != hp)
        {
            timeSince = 0;
            prevHp = hp;
        }
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
        if (hpRecCdCur >= hpRecCd && hp < maxHp)
        {
            hp++;
            hpRecCdCur= 0;
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
        maxExp = lvl * 6f;
        IncreaseStats(lvl);
    }
    public void IncreaseStats(float lvl_)
    {
        hp = hp + ((int)(hp / maxHp * (lvl_)));
        maxHp += lvl_;
        attack += 1;
        defense += 1;
    }
    public void TakeDamage(int damage, bool isCrit)
    {
        hp -= damage;
        damagePopup.isPlayer = true;
        damagePopup.Create(transform.position, damage, isCrit);
    }
}

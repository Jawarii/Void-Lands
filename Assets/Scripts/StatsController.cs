using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsController : MonoBehaviour
{
    public GameObject player;
    public PlayerStats playerStats;
    public TMP_Text offenseValue; //Offense
    public TMP_Text defenseValue; //Defense & Utility
    [Header("Offense Stats")]
    public float minAttack;
    public float maxAttack;
    public float critChance;
    public float critDmg;
    public float atkSpd;
    public float staggerDmg;
    [Header("Defense & Utility Stats")]
    public float armor;
    public float health;
    public float recovery;
    public float speed;
    public float cdRed;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        SetStats();
    }

    void FixedUpdate()
    {
        SetStats();
        SetStatsInUI();
    }

    public void SetStats()
    {
        playerStats = player.GetComponent<PlayerStats>();
        //Offense
        minAttack = playerStats.attack * 0.9f;
        maxAttack = playerStats.attack * 1.1f;
        critChance = playerStats.critRate;
        critDmg = playerStats.critDmg;
        atkSpd = playerStats.atkSpd;
        staggerDmg = playerStats.staggerDmg;
        //Defense & Utility
        armor = playerStats.defense;
        health = playerStats.maxHp;
        recovery = playerStats.hpRecovery;
        speed = playerStats.speed;
        cdRed = playerStats.cdReduction;
    }

    public void SetStatsInUI()
    {
        //Resetting Placeholders
        offenseValue.text = "";
        defenseValue.text = "";
        //Setting Offense Stats
        offenseValue.text += ((int) minAttack).ToString() + "~" + ((int) maxAttack).ToString() + "\n";
        offenseValue.text += critChance.ToString("0.0") + "%\n";
        offenseValue.text += critDmg.ToString("0.0") + "%\n";
        offenseValue.text += atkSpd.ToString("0.00") + "\n";
        offenseValue.text += staggerDmg.ToString("0.0") + "%";
        //Setting Defense & Utility Stats
        defenseValue.text += armor.ToString() + "\n";
        defenseValue.text += health.ToString() + "\n";
        defenseValue.text += recovery.ToString() + "\n";
        defenseValue.text += speed.ToString("0.00") + "\n";
        defenseValue.text += cdRed.ToString("0.0") + "%";
    }
}

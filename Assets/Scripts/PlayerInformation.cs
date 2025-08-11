using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerInformation : MonoBehaviour
{
    // Unlocks
    public bool lvl1IsComplete = false;
    public bool lvl1ArenaIsComplete = false;
    public bool lvl1BossIsComplete = false;

    public bool lvl2IsComplete = false;
    public bool lvl2ArenaIsComplete = false;
    public bool lvl2BossIsComplete = false;

    public bool lvl3IsComplete = false;  
    public bool lvl3ArenaIsComplete = false;  
    public bool lvl3BossIsComplete = false;

    public GameObject reaperBossPortal;
    public GameObject level1ArenaPortal;

    public bool finishedIntro = false;
    private void FixedUpdate()
    {
        if ( lvl1IsComplete )
        {
            UnlockLevel1Boss();
            UnlockLevel1Arena();
        }
    }

    public void UnlockLevel1Boss()
    {
        if (reaperBossPortal == null)
            return;
        reaperBossPortal.SetActive(true);
    }
    public void UnlockLevel1Arena()
    {
        if (level1ArenaPortal == null)
            return;
        level1ArenaPortal.SetActive(true);
    }
}

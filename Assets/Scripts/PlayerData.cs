using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float level;
    public float currentExp;
    public float maxExp;
    public float[] position;

    public PlayerData(PlayerStats playerStats)
    {
        level = playerStats.lvl;
        currentExp= playerStats.currentExp;
        maxExp = playerStats.maxExp;

        position = new float[3];
        position[0] = playerStats.transform.position.x;
        position[1] = playerStats.transform.position.y;
        position[2] = playerStats.transform.position.z;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterHUDController : MonoBehaviour
{
    private float checkInterval = 0.01f; // Interval for periodic checks
    private List<GameObject> detectedEnemies = new List<GameObject>();
    private GameObject chosenGameObject;

    private float timeSinceLastDetection = 0f; // Timer to track how long since last detection
    private bool enemyDetected = false; // Flag to track if an enemy was detected
    private float detectionRadius = 0.15f; // Radius around the mouse to detect enemies
    private Color originalLevelTextColor; // Store the original level text color

    private float currentDistance;
    void Start()
    {
        StartCoroutine(CheckMouseHover());

        // Capture the original color of the LevelText at the start
        Transform levelParent = transform.Find("MonsterLevel");
        TMP_Text levelText = levelParent?.Find("LevelText")?.GetComponent<TMP_Text>();
        if (levelText != null)
        {
            originalLevelTextColor = levelText.color; // Store the original color
        }
    }

    private IEnumerator CheckMouseHover()
    {
        while (true)
        {
            detectedEnemies.Clear(); // Clear the list for fresh checks
            enemyDetected = false; // Reset the detection flag

            // Get the mouse position in world coordinates
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Find all colliders within the detection radius
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mousePosition, detectionRadius);

            foreach (var hitCollider in hitColliders)
            {
                GameObject hoveredObject = hitCollider.gameObject;

                // Check if the hovered object is tagged as "Enemy" and is not a boss
                if (hoveredObject.CompareTag("Enemy"))
                {
                    EnemyStats enemyStats = hoveredObject.GetComponent<EnemyStats>();
                    if (enemyStats != null && !enemyStats.isBoss && !enemyStats.isDead)
                    {
                        detectedEnemies.Add(hoveredObject);
                        enemyDetected = true; // Enemy detected, set the flag
                    }
                }
            }

            // Reset the timer if an enemy is detected
            if (enemyDetected)
            {
                timeSinceLastDetection = 0f;
            }

            // Find the closest game object to the center of the circle
            GameObject newChosenGameObject = FindClosestEnemy(mousePosition);

            // Update the chosenGameObject only if it has changed
            if (newChosenGameObject != chosenGameObject)
            {
                chosenGameObject = newChosenGameObject;
                EnableHUD(chosenGameObject);
            }

            yield return new WaitForSeconds(checkInterval); // Wait for the next check
        }
    }

    private GameObject FindClosestEnemy(Vector2 mousePosition)
    {
        GameObject closestEnemy = chosenGameObject;
        float shortestDistance = float.MaxValue;

        foreach (GameObject enemy in detectedEnemies)
        {
            float distance = Vector2.Distance(mousePosition, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    public void EnableHUD(GameObject chosenMonster)
    {
        if (chosenMonster == null)
        {
            gameObject.GetComponent<Canvas>().enabled = false;
            return;
        }

        // Update HP targets
        GetComponentInChildren<DelayedHPEnemy>().target = chosenMonster.transform;
        GetComponentInChildren<LiveHPEnemy>().target = chosenMonster.transform;

        GameObject liveHpGo = transform.Find("LiveHPMain").gameObject;
        LiveHPEnemy liveHPEnemy = liveHpGo.GetComponent<LiveHPEnemy>();
        GameObject delayedHpGo = transform.Find("DelayedHPMain").gameObject;
        DelayedHPEnemy delayedHPEnemy = delayedHpGo.GetComponent<DelayedHPEnemy>();

        liveHPEnemy.SetVariables();
        delayedHPEnemy.SetVariables();
        delayedHPEnemy.SetSlider();

        // Update MonsterName TMP_Text
        TMP_Text nameText = transform.Find("MonsterName")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            EnemyStats enemyStats = chosenMonster.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                if (!enemyStats.isElite) // Normal Monster
                {
                    // Temporary solution for naming normal monsters
                    if (chosenMonster.name.Contains("Slime"))
                    {
                        nameText.text = "Swamp Slime";
                    }
                    else if (chosenMonster.name.Contains("Melee"))
                    {
                        nameText.text = "Goblin Warrior";
                    }
                    else if (chosenMonster.name.Contains("Sniper"))
                    {
                        nameText.text = "Goblin Sniper";
                    }
                    else if (chosenMonster.name.Contains("Golem"))
                    {
                        nameText.text = "Enchanted Golem";
                    }
                    else
                    {
                        nameText.text = chosenMonster.name; // Fallback to GameObject's name
                    }
                }
                else // Elite Monster
                {
                    Transform enemyNameTransform = chosenMonster.transform.Find("Canvas (2)/EnemyName");
                    TMP_Text enemyNameText = enemyNameTransform?.GetComponent<TMP_Text>();
                    if (enemyNameText != null)
                    {
                        nameText.text = enemyNameText.text + " (Elite)"; // Copy the name and add " (Elite)"
                    }
                }

                // Toggle EliteHPBorder and NormalHPBorder
                Transform eliteHPBorder = transform.Find("EliteHPBorder");
                Transform normalHPBorder = transform.Find("NormalHPBorder");

                if (eliteHPBorder != null && normalHPBorder != null)
                {
                    eliteHPBorder.gameObject.SetActive(enemyStats.isElite);
                    normalHPBorder.gameObject.SetActive(!enemyStats.isElite);
                }
            }
        }

        // Update MonsterLevel TMP_Text
        Transform levelParent = transform.Find("MonsterLevel");
        TMP_Text levelText = levelParent?.Find("LevelText")?.GetComponent<TMP_Text>();
        if (levelText != null)
        {
            EnemyStats enemyStats = chosenMonster.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                levelText.text = $"{enemyStats.enemyLvl}";

                // Update LevelText color based on level difference
                PlayerStats playerStats = FindObjectOfType<PlayerStats>(); // Find the player's stats
                if (playerStats != null)
                {
                    int levelDifference = (enemyStats.enemyLvl - (int)playerStats.lvl);
                    if (levelDifference >= 3)
                    {
                        levelText.color = Color.red; // Red for 3 or more levels higher
                    }
                    else if (levelDifference == 2)
                    {
                        levelText.color = new Color(1f, 0.5f, 0f); // Orange for 2 levels higher
                    }
                    else if (levelDifference == 1)
                    {
                        levelText.color = Color.yellow; // Yellow for 1 level higher
                    }
                    else
                    {
                        levelText.color = originalLevelTextColor; // Original color for same level or lower
                    }
                }
            }
        }
        gameObject.GetComponent<Canvas>().enabled = true;
    }

    void Update()
    {
        // Increment the timer if no enemies are detected
        if (!enemyDetected)
        {
            timeSinceLastDetection += Time.deltaTime;
        }

        // Reset the HUD and chosenGameObject if the timer exceeds 2 seconds
        if (timeSinceLastDetection > 0.01f)
        {
            chosenGameObject = null;
            EnableHUD(null); // Ensure the HUD is disabled
        }

        // Check if the chosenGameObject is still valid
        if (chosenGameObject != null)
        {
            EnemyStats enemyStats = chosenGameObject.GetComponent<EnemyStats>();
            if (enemyStats == null || enemyStats.hp <= 0 || enemyStats.isDead)
            {
                chosenGameObject = null;
                EnableHUD(null); // Disable the HUD
            }
        }
    }
}

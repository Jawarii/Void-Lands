using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfoSO", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfoSO : ScriptableObject
{
    // Basic Info
    public int stackSize;
    public int itemId;
    public string itemName;
    public string itemDescription;
    public string itemType;
    public int itemLvl;
    public string itemQuality;
    public int upgradeLevel;

    // Special Visual Info
    public int currentStackSize = 1;
    public Sprite itemIcon;
    public Color textColor; // Color property

    // Weapon Special Info
    [System.Serializable]
    public class WeaponMainStats
    {
        public int baseAttack;
        public int attack;
        public int minAttack;
        public int maxAttack;
    }

    public WeaponMainStats weaponMainStats = new WeaponMainStats();

    [System.Serializable]
    public class WeaponBonusStats
    {
        public int attack;
        public float critChance;
        public float critDamage;
        public float atkSpeed;
        public float staggerDmg;
    }

    public WeaponBonusStats weaponBonusStats = new WeaponBonusStats();

    // Gear Special Info
    [System.Serializable]
    public class GearMainStats
    {
        public int baseAttack;
        public int baseHp;
        public int baseArmor;
        public int attack;
        public int hp;
        public int armor;
    }

    public GearMainStats gearMainStats = new GearMainStats();

    [System.Serializable]
    public class GearBonusStats
    {
        public int attack;
        public float critChance;
        public float critDamage;
        public float atkSpeed;
        public float staggerDmg;
        public int hp;
        public int armor;
        public float cdRed;
        public float moveSpeed;
        public int recovery;
    }

    public GearBonusStats gearBonusStats = new GearBonusStats();

    private System.Random rand = new System.Random();

    public void UpgradeWeapon()
    {
        if (upgradeLevel < 9)
        {
            upgradeLevel++;
            float upgradeMultiplier = 1 + (0.05f * upgradeLevel);

            weaponMainStats.attack = Mathf.RoundToInt(weaponMainStats.baseAttack * upgradeMultiplier);
            weaponMainStats.minAttack = Mathf.RoundToInt(weaponMainStats.attack * 0.9f);
            weaponMainStats.maxAttack = Mathf.RoundToInt(weaponMainStats.attack * 1.1f);

            UpdateItemName();
        }
        else
        {
            Debug.Log("Weapon has reached the maximum upgrade level.");
        }
    }

    public void UpgradeGear()
    {
        if (upgradeLevel < 9)
        {
            upgradeLevel++;
            float upgradeMultiplier = 1 + (0.05f * upgradeLevel);

            gearMainStats.attack = Mathf.RoundToInt(gearMainStats.baseAttack * upgradeMultiplier);
            gearMainStats.hp = Mathf.RoundToInt(gearMainStats.baseHp * upgradeMultiplier);
            gearMainStats.armor = Mathf.RoundToInt(gearMainStats.baseArmor * upgradeMultiplier);

            UpdateItemName();
        }
        else
        {
            Debug.Log("Gear has reached the maximum upgrade level.");
        }
    }

    private void UpdateItemName()
    {
        itemName = itemName.StartsWith("+") ? itemName.Substring(itemName.IndexOf(" ") + 1) : itemName;
        itemName = $"+{upgradeLevel} {itemName}";
    }

    public Color GetTextColorFromString(string colorString)
    {
        ColorUtility.TryParseHtmlString($"#{colorString}", out Color color);
        return color;
    }

    public ItemInfoData ToData()
    {
        return new ItemInfoData
        {
            stackSize = stackSize,
            itemId = itemId,
            itemName = itemName,
            itemDescription = itemDescription,
            itemType = itemType,
            itemLvl = itemLvl,
            itemQuality = itemQuality,
            upgradeLevel = upgradeLevel,
            currentStackSize = currentStackSize,
            itemIcon = itemIcon ? itemIcon.texture.EncodeToPNG() : null,
            textColor = new ColorData(textColor),
            weaponMainStats = weaponMainStats,
            weaponBonusStats = weaponBonusStats,
            gearMainStats = gearMainStats,
            gearBonusStats = gearBonusStats
        };
    }
    public void FromData(ItemInfoData data)
    {
        stackSize = data.stackSize;
        itemId = data.itemId;
        itemName = data.itemName;
        itemDescription = data.itemDescription;
        itemType = data.itemType;
        itemLvl = data.itemLvl;
        itemQuality = data.itemQuality;
        upgradeLevel = data.upgradeLevel;
        currentStackSize = data.currentStackSize;
        if (data.itemIcon != null)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(data.itemIcon);
            itemIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        textColor = data.textColor.ToColor();
        weaponMainStats = data.weaponMainStats;
        weaponBonusStats = data.weaponBonusStats;
        gearMainStats = data.gearMainStats;
        gearBonusStats = data.gearBonusStats;
    }

}

using UnityEngine;

[System.Serializable]
public class CropDetails
{
    [ItemCodeDescription] // This custom property will display the item code description in the editor
    public int seedItemCode; // This is the item code for the corresponding seed for this crop
    public bool doesNeedWater; // This bool determines whether or not the seed needs water to grow! I added this line so that crops do need water, and trees don't
    public int[] growthDays; // Array with the total days of growth required for each stage of the crop (i.e. [0,1,2,3,5] corresponds to 1,1,1,2 days of growth for each stage). The last element is the total number of days required!
    public GameObject[] growthPrefab; // List of prefabs to use when instantiating each growth stage (could be the same)
    public Sprite[] growthSprite; // List of sprites for the growth stages (i.e. seed -> sapling -> small tree -> medium tree -> tree)
    public Season[] growthSeasons; // List of seasons this crop can grow in
    public Sprite harvestedSprite; // Sprite to use once the crop has been harvested

    [ItemCodeDescription]
    public int harvestedTransformItemCode; // If the item transforms into another item when harvested, this item code will be populated (i.e. treeCrop -> harvest -> stumpCrop -> harvest -> wood left)
    public bool hideCropBeforeHarvestedAnimation; // If the crop should be disabled before the harvested animation (i.e. a carrot?)
    public bool disableCropCollidersBeforeHarvestedAnimation; // If colliders on the crop should be disabled to avoid the harvested animation affecting any other game objects
    public bool isHarvestedAnimation; // True if harvested animation is to be played on the final growth stage prefab (i.e. hitting tree with axe makes it wobble)
    public bool isHarvestActionEffect = false; // Flag to determine whether there is a harvest action effect (i.e. leaves fall off tree when hit with axe)
    public bool spawnCropProducedAtPlayerPosition; // Determines if a crop should be spawned at the players position, so the player will automatically pick it up
    public HarvestActionEffect harvestActionEffect; // The harvest action effect for the crop that will be played (i.e. leaves fall off tree when hit with axe)
    public SoundName harvestSound; // The harvest sound that will be played when we harvest a crop

    [ItemCodeDescription]
    public int[] harvestToolItemCode; // The array of item codes for the tools that can harvest the crop, or 0 array elements if no tool is required (for example - tree can be cut with axe, brass axe, gold axe)
    public int[] requiredHarvestActions; // Number of harvest actions required to complete to harvest the crop, for the corresponding tool in the harvestToolItemCode array. (for example - axe takes 5 hits, brass 3, and gold 1)

    [ItemCodeDescription] 
    public int[] cropProducedItemCode; // Array of item codes produced for the harvested crop (i.e. cut down tree drops wood, acord, sticks..)
    public int[] cropProducedMinQuantity; // Array of the minimum quantities produced for the harvested crop, in the same order as the cropProducedItemCode array
    public int[] cropProducedMaxQuantity; // If max quantity > min Quantity, then a random number of crops between the two are produced. If they are the same, it just produces that many
    public int daysToRegrow; // Days to regrow the next crop, or a -1 if only a single harvest occurs.


    /// <summary>
    /// Returns true if the tool item code can be used to harvest this crop, else returns false.
    /// </summary>
    public bool CanUseToolToHarvestCrop(int toolItemCode)
    {
        // This method will return the number of actions required for the given tool to harvest the crop, or -1
        // If this tool can't harvest it
        if (requiredHarvestActionsForTool(toolItemCode) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// Returns -1 if the tool can't be used to harvest this crop, else returns the number of harest actions required by this tool to harvest the crop.
    /// </summary>
    public int requiredHarvestActionsForTool(int toolItemCode)
    {
        // Loop through all of the elements of harvestToolItemCode, which defines the tools that can harvest this crop
        for (int i = 0; i < harvestToolItemCode.Length; i++)
        {
            if (harvestToolItemCode[i] == toolItemCode)
            {
                // If one of the possible items matches the item we are questioning, return the corresponding element of requiredHarvestActions, that is the
                // number of actions required to harvest
                return requiredHarvestActions[i];
            }
        }
        return -1;
    }
}
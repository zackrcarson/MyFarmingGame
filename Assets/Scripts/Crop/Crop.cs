using System.Collections;
using UnityEngine;

// Attached to all crops in the scene. This stores the grid position of the crop, and 
public class Crop : MonoBehaviour
{
    // This keeps track of how many actions have been made towards harvest
    private int harvestActionCount = 0;

    [HideInInspector]
    public Vector2Int cropGridPosition;
    
    
    // This method will determine if the player has used the correct number of harvest actions, and harvest the crop if so, if not the number of actions increases
    // by 1 and we can try again
    public void ProcessToolAction(ItemDetails equippedItemDetails)
    {
        // Get the grid property details for the crops grid position, quit out if they don't exist!
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        if (gridPropertyDetails == null)
        {
            return;
        }

        // Get seed item details from the grid square in question, quit out if they don't exist!
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if (seedItemDetails == null)
        {
            return;
        }

        // Get crop details from the seedItemDetails planted in that square, quit out if they don't exist!
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if (seedItemDetails == null)
        {
            return;
        }

        // Get the required harvest actions for the tool in question, quit out if this tool can't harvest this crop (-1)
        // Although, we already know we have a valid grid cursor (or else we couldn't access this) - so this is
        // just for extra safety
        int requiredHarvestActions = cropDetails.requiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
        {
            return;
        }

        // Increment the harvest action count, assuming we've passed all the above tests
        harvestActionCount += 1;

        // Check if we've reached the required harvest actions yet
        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails);
        }
    }


    // This method will clear out all of the crop properties in the gridPropertyDetails (i.e. seedItemCode, growthDays, DaysSinceWatered, DaysSinceLastHarvest),
    // and then spawn the harvested materials to pick up, and then destroy the original crop object
    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // Delete the crop from the grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // This method will spawn the harvested materials, and destroy the crop gameObject
        HarvestActions(cropDetails, gridPropertyDetails);
    }


    // This method will spawn the harvested items for the player to pickup, and then destroy the Crop gameObject this Crop script is attached to
    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // This method will spawn the corresponding harvested items depending on it's CropDetails
        SpawnHarvestedItems(cropDetails);

        Destroy(gameObject);
    }


    // This method will spawn the harvested items that come from the crop, as described by the crops CropDetails
    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        // Spawn the item(s) to be produced. Loop through the array of different items this crop produces on harvest
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;

            // Calculate how many crops to produce of this type, based on the min and max quantites in the cropProducedMinQuantity/cropProducedMaxQuantity arrays for this
            // produced item in the loop

            // If min and max are the same, or if max < min, spawn exactly the min quantity
            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            // Else, produce a random number between the min and max quantites
            else
            {
                // + 1 to make it inclusive for the max quantity
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            // Loop through the number of crop i to produced, to instantiate cropsToProduce of them
            for (int j = 0; j < cropsToProduce; j++)
            {
                Vector3 spawnPosition;

                // If the CropDetails for this crop has spawnCropProducedAtPlayerPosition = true, just add it to the players inventory (like pulling
                // up a parsnip)
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    // Directly add the item to the players inventory
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                // If it's false (like chopping a tree), we will drop it in a random space surrounding the crop we harvested for the player to pick up
                else
                {
                    // Spawn it at a random position around the crops position
                    spawnPosition = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(-1f, 1f), 0f);
                    SceneItemsManager.Instance.InstantiateSceneItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }
}

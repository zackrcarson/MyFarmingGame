using System.Collections;
using UnityEngine;

// Attached to all crops in the scene. This stores the grid position of the crop, and 
public class Crop : MonoBehaviour
{
    // This keeps track of how many actions have been made towards harvest
    private int harvestActionCount = 0;

    // This is the transform position for the harvest action effect (like where the leaves fall from when we chop a tree), populated
    // in the editor, from the child transform object
    [Tooltip("This should be populated from the child transform gameObject, showing the harvest effect spawn point.")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    // This is the SpriteRenderer that the animator will grab and manipulate for the harvest animation (tooltip appears when you hover over it!)
    [Tooltip("This should be populated from the child gameObject.")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;
    
    
    // This method will determine if the player has used the correct number of harvest actions, and harvest the crop if so, if not the number of actions increases
    // by 1 and we can try again. Once harvested, we harvest it and play the crop harvested animation
    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp )
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

        // Get the crop animator if present
        Animator animator = GetComponentInChildren<Animator>();

        // If the animator exists, trigger the tool animation parameters based on the tool use direction
        // The controller will use these parameters to initiate the proper animation sequence for lifting the crop out of the ground
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        // Thigger the harvest partical effect on the crop, if it is marked in the so_cropDetailsList for this crop 
        if (cropDetails.isHarvestActionEffect)
        {
            // Publish this event, which will be picked up by subscribers who will play the effect at the given position (transform populated in editor),
            // With the harvestActionEffect (ie leaves falling, rocks crumbling, etc) described in the so_cropDetailsList for this crop
            EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position, cropDetails.harvestActionEffect);
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
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }
    }


    // This method will clear out all of the crop properties in the gridPropertyDetails (i.e. seedItemCode, growthDays, DaysSinceWatered, DaysSinceLastHarvest),
    // and then spawn the harvested materials to pick up, and then destroy the original crop object
    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        // Check if there is a harvested animation (i.e. parsnip lifting out of dirt on harvest), and an animator exists. 
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            // Next see if the crop in question has a harvested sprite (like veggie crops - i.e. a parsnip), and then check if there is a cropHarvestedSpriteRenderer, which is controlled by the
            // harvest animation controller
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    // Set the sprite in the cropHarvestedSpriteRenderer object to be the crops harvestedSprite
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }   
            }

            if (isUsingToolRight || isUsingToolUp)
                {
                    animator.SetTrigger("harvestright");
                }
                else
                {
                    animator.SetTrigger("harvestleft");
                }
        }

        // Delete the crop from the grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // Check if the crop should be hidden before the harvested animation (e.g. for a parsnip, this is true - so once we pull the parsnip, the planted version in
        // the dirt dissapears while we pull it up. Else, we would see the crop still in the ground as the animation lifts a second copy of it up into the air)
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            // If we do hide it, just disable the spriteRenderer
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // Check if we need to disable the colliders on the harvest crop resource, and then disable them if so (or else, for example, the harvested rock
        // will push the player out of the way as it animates upwards!
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            // load up all of the box colliders in the children game components
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            
            // Disable all of the found colliders
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // If there is a harvested animation on this crop, wait until the animation is complete before spawning the harvested
        // materials, and destroying the crop gameObject. Else, go straight to that
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            // This coroutine will delay HarvestActions (spawn the harvested materials, and destroy the crop gameObject)
            // until the harvest animation is complete
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {
            // This method will spawn the harvested materials, and destroy the crop gameObject
            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }


    //
    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        // The harvest animation controller has a final state of "Harvested" once it's done. We will keep on returning null every frame
        // until the animation is complete, before continuing
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        // Now that the animation is completed, we can spawn the harvested materials, and destroy the crop gameObject
        HarvestActions(cropDetails, gridPropertyDetails);
    }


    // This method will spawn the harvested items for the player to pickup, and then destroy the Crop gameObject this Crop script is attached to
    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // This method will spawn the corresponding harvested items depending on it's CropDetails
        SpawnHarvestedItems(cropDetails);

        // Check if this crop transforms into another crop once harvested (like tree -> stump)
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            // This method will create the new transformed crop instance at the same location as the original crop
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }

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


    // This method will create the new transformed crop after harvesting the original crop (i.e. tree -> stump)
    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // Update the crop details in the grid properties, with the info from the harvestedTransformItemCode info in the cropDetails, and the default undug, 
        // unwatered, and ungrown gridProperties
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display the new transformed crop as set up in the gridPropertyDetails
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }
}

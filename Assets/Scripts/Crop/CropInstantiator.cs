using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a crop prefab to set the values (for pre-instantiated crop items like trees at various stages of growth) in the grid property dictionary!
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private Grid grid;

    // These are all the properties we can manipulate to pre-instantate crops in various stages of growth
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;


    // Subscribe the InstantiateCropPrefabs method to the InstantiateCropPrefabsEvent, so that when It published the event, we can instantiate all of the proper crops
    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;
    }


    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;
    }


    // This method is triggered when the InstantiateCropPrefabsEvent is published, and it finds the crops (that this script is attached to) location,
    // and sets up the grid properties for it, then destroys it (so the game can instantiate the proper-growth form for this tree on load)
    private void InstantiateCropPrefabs()
    {
        // Get the grid gameObject that the crops live on
        grid = GameObject.FindObjectOfType<Grid>();

        // Get the grid position for the crop this script lives on
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        // Set up the crop grid properties and instantiate the crop in the proper growth stage
        SetCropGridProperties(cropGridPosition);

        // Destroy this gameObject, so the new instantiated crop with the proper growth stage will live
        Destroy(gameObject);
    }


    // This method will simply update the gridPropertDetails for the grid square that this crop we want to instantiate will live on
    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {   
        // Check if we've specified a seed ItemCode in the editor for this crop we want to instantiate (with this script on it)
        if (seedItemCode > 0)
        {
            GridPropertyDetails gridPropertyDetails;
            
            // Get the current gridPropertyDetails on that square (initialize them if they don't exist for some reason)
            gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

            if (gridPropertyDetails != null)
            {
                gridPropertyDetails = new GridPropertyDetails();
            }

            // Set the daysSinceDug, daysSinceWatered, seedItemCode, and growthDays with those populated in the editor that we want to instantiate
            gridPropertyDetails.daysSinceDug = daysSinceDug;
            gridPropertyDetails.daysSinceWatered = daysSinceWatered;
            gridPropertyDetails.seedItemCode = seedItemCode;
            gridPropertyDetails.growthDays = growthDays;

            // Set up the above updated gridPropertyDetails for the given square
            GridPropertiesManager.Instance.SetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y, gridPropertyDetails);
        }
    }
}

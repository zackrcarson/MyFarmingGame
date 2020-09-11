using System.Collections.Generic;
using UnityEngine;

// This script will create a asset menu to create a scriptable object to store a list CropDetails for every crop in the game
[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField]
    public List<CropDetails> cropDetails;


    // This method will simply get the crop details in the SO, given an itemCode of the seed you want the details for
    public CropDetails GetCropDetails(int seedItemCode)
    {
        // This is a predicate - Basically find x, where x's member variable seedItemCode is the seedItemCode, and return the seed item code
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}

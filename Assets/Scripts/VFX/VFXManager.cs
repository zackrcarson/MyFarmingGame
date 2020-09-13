using System.Collections;
using UnityEngine;

// This Singleton game manager will listen out for the HarvestActionEffectEvent, which publishes whenever something has been harvested.
// This class will check what has been harvested, and play the corresponding particle effect!
public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    // Delay time in the coroutine to play the harvest effect for
    private WaitForSeconds twoSeconds;

    // Populated in the editor with the prefab for the reaping particle effect
    [SerializeField] private GameObject reapingPrefab = null;

    // Populated in the editor with the prefab for the deciduous Leaves Falling particle effect
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;


    protected override void Awake()
    {
        base.Awake();

        twoSeconds = new WaitForSeconds(2f);
    }


    // subscribe the method displayHarvestActionEffect to the HarvestActionEffectEvent, which is published whenever something is harvested.
    // This method will check what the harvest action was (i.e. a reaping/chopping/breaking/... effect), and if so it will display the corresponding particle effect
    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += displayHarvestActionEffect;
    }
    
    
    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= displayHarvestActionEffect;
    }


    private IEnumerator DisableHarvestActionEffect(GameObject effectsGameObject, WaitForSeconds secondsToWait)
    {
        yield return secondsToWait;
        effectsGameObject.SetActive(false);
    }


    // This method checkes what the harvest action was (chopping, reaping, breaking, etc), and then initiates the proper coroutine to display the corresponding particle effect
    private void displayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {   
        // Check what the harvestActionEffect was!
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.deciduousLeavesFalling:
                // If it was a deciduousLeavesFalling harvest, Grab the reaping GameObject from the PoolManager deciduousLeavesFalling queue, set it to active (This starts the 
                // particle effect), and then initiate the coroutine to disable the gameObject after two seconds
                GameObject deciduousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                deciduousLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(deciduousLeavesFalling, twoSeconds));
                break;


            case HarvestActionEffect.reaping:
                // If it was a reaping harvest, Grab the reaping GameObject from the PoolManager reaping queue, set it to active (This starts the 
                // particle effect), and then initiate the coroutine to disable the gameObject after two seconds
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                break;

            // If anything else, just do nothing
            case HarvestActionEffect.none:
                break;

            default:
                break;
        }
    }
}

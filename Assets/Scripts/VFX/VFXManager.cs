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

    // Populated in the editor with the prefab for the pine cones Falling particle effect
    [SerializeField] private GameObject pineConesFallingPrefab = null;

    // Populated in the editor with the prefab for the chopping tree trunk particle effect
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;

    // Populated in the editor with the prefab for the breaking stone particle effect
    [SerializeField] private GameObject breakingStonePrefab = null;


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
            // Deciduous leaves falling from the tree as you chop it
            case HarvestActionEffect.deciduousLeavesFalling:
                // If it was a deciduousLeavesFalling harvest (ie chopping the tree), Grab the deciduousLeavesFalling GameObject from the PoolManager 
                // deciduousLeavesFalling queue, set it to active (This starts the particle effect in the prefab), and then initiate the 
                // coroutine to disable the gameObject after two seconds
                GameObject deciduousLeavesFalling =  PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);

                deciduousLeavesFalling.SetActive(true);

                StartCoroutine(DisableHarvestActionEffect(deciduousLeavesFalling, twoSeconds));

                break;

            // Pine cones falling from the tree as you chop it
            case HarvestActionEffect.pineConesFalling:
                // If it was a pineConesFalling harvest (ie chopping the spruce tree), Grab the pineConesFalling GameObject from the PoolManager 
                // pineConesFalling queue, set it to active (This starts the particle effect in the prefab), and then initiate the 
                // coroutine to disable the gameObject after two seconds
                GameObject pineConesFalling =  PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);

                pineConesFalling.SetActive(true);

                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, twoSeconds));

                break;
            
            // Wood splinters bursting as you chop a trunk
            case HarvestActionEffect.choppingTreeTrunk:
                // If it was a choppingTreeTrunk harvest (ie chopping the trunk), Grab the choppingTreeTrunk GameObject from the PoolManager 
                // choppingTreeTrunk queue, set it to active (This starts the particle effect in the prefab), and then initiate the 
                // coroutine to disable the gameObject after two seconds
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);

                choppingTreeTrunk.SetActive(true);

                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeconds));

                break;

            // rock chunks spray as you break a stone
            case HarvestActionEffect.breakingStone:
                // If it was a breakingStone harvest (ie breaking rocks), Grab the breakingStone GameObject from the PoolManager 
                // breakingStone queue, set it to active (This starts the particle effect in the prefab), and then initiate the 
                // coroutine to disable the gameObject after two seconds
                GameObject breakingStone = PoolManager.Instance.ReuseObject(breakingStonePrefab, effectPosition, Quaternion.identity);

                breakingStone.SetActive(true);

                StartCoroutine(DisableHarvestActionEffect(breakingStone, twoSeconds));

                break;

            // Grass particles flying as you reap grass
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

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    // These serialized fields will let us in the editor populate the player GameObject, and the SO with an array of animation type overrides
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    // Dictionaries of <animationClip, SO asset type>, and <composite string, SO asset type> so we can reference the SO objects easier
    // The SO_AnimationType is the SO that contains public animationClip, animationName, characterPart, partVariantColor, partVariantType;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        // Initialize the animation type dictionary keyed by animation clip
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            // Populate the first dictionary with the animation clips as the keys, and the SO object itself as the value!
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // Initialize animation type dictionary keyed by composite string
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            // Populate this with a custom composite string key, composed of values from the SO objects, that belong in the CharacterAttribute class.
            string key = item.characterPart.ToString() + item.partVariantColor.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    // Pass in a list of character attributes, which are structs containing the character part, part variant color, and the anim type
    public void ApplyCharacterCustomizationParameters(List<CharacterAttribute> characterAttributesList)
    {
        // Stopwatch s1 = Stopwatch.StartNew();

        // Loop through all of the character attributes and set the animation override controller for each of them
        foreach (CharacterAttribute characterAttribute in characterAttributesList)
        {
            Animator currentAnimator = null;

            // Create a list of key value pairs that we will later occupy with currentClip : replacementClip
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // The name of the character body part from the characterAttribute struct found in the characterAttributeList given to this method.
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            // Find animators in the player that match scriptable object animator type
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            // Loop through all the animators in the animators array from the players children body parts.
            // If the name of the found animator matches the currently iterated character attribute body part, set the current animator to that one
            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            // Initialize a new animator override controller with the base current animations for the current animator
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);

            // This returns a list of all animations in the current runtime animator
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            // Loop through the animations clip found in the animations list
            foreach (AnimationClip animationClip in animationsList)
            {
                // Find animation in dictionary1, i.e. if it's found in the list of SO objects that we populated via the editor
                SO_AnimationType SO_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out SO_AnimationType);

                // If we DO have a valid animation to be swapped with
                if (foundAnimation)
                {
                    // This is our composite key build up from the characterAttributes struct
                    // i.e., an example key would be ArmsNoneCarryWalkLeft, or LegsNoneNoneIdleUp, etc
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColor.ToString() + characterAttribute.partVariantType.ToString() + SO_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;

                    // Check if that key exists in the second dictionary that matches those keys, to a swapped override animation
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    // If we DID find a valid animation to be swapped for
                    if (foundSwapAnimation)
                    {
                        // The new swappedAnimationClip can be found from the value from dictionary2 which is the SO animationType.animationClip
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;

                        // Add the currentAnimationClip, swapAnimationClip to the key-value-pair list to be applied to the override controller
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            // Apply the animation overrides list that is now completed to the animation override controller and then update the animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }

        // s1.Stop();
    }
}

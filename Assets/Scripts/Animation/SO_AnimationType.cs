using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will create an asset menu to create a SO asset! This SO object contains the actual animation clip, the animations name (run right, walk down, etc), 
// the part of the player it applies to (arms, tools, etc), the part variant color (if the animations differ based on color), and the part variant type (none, carry, swing, etc)
[CreateAssetMenu(fileName = "so_AnimationType", menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationType : ScriptableObject
{
    public AnimationClip animationClip;
    public AnimationName animationName;
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;
}

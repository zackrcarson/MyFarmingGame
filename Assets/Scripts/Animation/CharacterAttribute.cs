// This struct is for a character attribute to define the swappable animations. To find the correct animations, 
// we need the character part to be animated, the part variant color (if we have different color animations),
// and the type of animation (carry, swing, etc). These are all enum values;
[System.Serializable]
public struct CharacterAttribute
{
    public CharacterPartAnimator characterPart;
    public PartVariantColor partVariantColor;
    public PartVariantType partVariantType;

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantColor partVariantColor, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantColor = partVariantColor;
        this.partVariantType = partVariantType;
    }
}

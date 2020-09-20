using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class simply stores the from color and to color for any color swap that we'll be using
[System.Serializable]
public class colorSwap
{
    public Color fromColor;
    public Color toColor;

    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}


public class ApplyCharacterCustomization : MonoBehaviour
{
    // INPUT TEXTURES
    // Input Textures to be populated in the editor. The first two are the "naked farmer" textures that we will be drawing clothes over (for now, male = female, but later
    // we can add a female one). The next one is the set of shirt textures (i.e. green and red - maybe more in the future!) that we will be drawing over the naked farmer. The next one
    // is the set of all possible hairstyles the player can pick. The last one is the base texture for the "naked farmer". This will be set to the male or female one based on what we've 
    // selected. A texture is the sprite sheet filled with the sprites for every player direction etc
    [Header("Base Textures")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    private Texture2D farmerBaseTexture;

    // OUTPUT CREATED TEXTURES
    // Created textures. The first one is the target final texture that we have created with the character customizer, and will be used to draw over the naked base farmer.
    // The next one will be a texture (sprite sheet) of the customized shirts to be drawn over each of the naked player positions at the same sprite sheet locations. 
    // The next one is the final, customized hair texture that the player chose, and colored to what the player picked. The next one is the 
    // set of shirts that we've selected (i.e. the red one or the green one.). The last one is the texture containing the shirt in all facing directions that the player chose.
    // The farmerBaseCustomized will be updated in this class, and is the one that is used by the animator to draw the player!
    [Header("Output Base Texture To Be Used For Animation")]
    [SerializeField] private Texture2D farmerBaseCustomized = null;
    [SerializeField] private Texture2D hairCustomized = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D selectedShirt;

    // CUSTOMIZATION OPTIONS
    // Select the shirt style with a slider (0 - green, 1 - red), populated in the editor
    [Header("Select Shirt Style: 0 = red, 1 = green")]
    [Range(0, 1)]
    [SerializeField] private int inputShirtStyleNo = 0;

    // Select the hair style with a slider (0 - styled, 1 - spiky, 2 - bald), populated in the editor
    // The bald hairstyle will simply grab empty sprites from the base hair texture - so it will show up as bald
    [Header("Select Hair Style: 0 = styled, 1 = spiky, 2 = bald")]
    [Range(0, 2)]
    [SerializeField] private int inputHairStyleNo = 0;

    // Select the hair color from an RGB color picker
    [Header("Select Hair Color")]
    [SerializeField] private Color inputHairColor = Color.black;

    // Select the skin color from an RGB color picker
    [Header("Select Skin Color")]
    [Range(0, 3)]
    [SerializeField] private int inputSkinType = 0;


    // Select the gender (0 - male, 1 - female), populated in the editor (right now both male and female are the same)
    [Header("Select Sex: 0 = Male, 1 = Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    // Select the trouser color from an RGB color picker
    [Header("Select Trouser Color")]
    [SerializeField] private Color inputTrouserColor = Color.blue;

    // 2D Array of enums storing the different directions the player could be facing, so we can always apply the correct shirt over it
    // Also a 2D array of Vector2Ints for the shirt offset to be drawn on the naked farmer, i.e. as he bobs up and down while running
    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;

    // Sprite sheet dimensions
    private int bodyRows = 21; // There are 21 total rows and 6 columns of farmer animations (although currently the bottom ~10 rows or so are greened out with nothing there
    private int bodyColumns = 6;

    private int farmerSpriteWidth = 16; // Each farmer sprite is 16x32 pixels
    private int farmerSpriteHeight = 32;

    private int shirtTextureWidth = 9; // Each sprite texture for an individual shirt is 9x36 pixels (each shirt orientation is 9x9, and there are 4 orientations stacked given a shirt choice
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9; // Each shirt sprite itself is 9x9 pixels
    private int shirtSpriteHeight = 9;
    private int shirtStylesInSpriteWidth = 16; // We can fit 16 different shirts (we are currently only using 2!!) across the texture (sprite sheet) width 

    private int hairTextureWidth = 16; // height and width of a selected hair texture (each hair style has 3 16x16 views of the same hairstyle, and can hold up to 6 in the vertical direction). There is room for 8 columns (8 hairstyles) horizontally
    private int hairTextureHeight = 96;
    private int hairStylesInSpriteWidth = 8;

    // List of color swaps we want to apply! We will loop through this list and apply all of the color swaps initiated there
    private List<colorSwap> colorSwapList;

    // Target arm colors for color replacement. The default arms have a sleeve fabric consisting of three colors: dark red, medium-dark-red, and light red,
    // Which are defined by the exact RGB values defined below. We will be finding all of these colors (fromColorSwap) and replacing them with a new color 
    // (toColorSwap) that the player customized. The colors below msut be exact so the code can find and replace them!
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255); // The darkest color on the default arm sleeve
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255); // The middle-dark color on the default arm sleeve
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255); // The lightest color on the default arm sleeve

    // Target skin colors for color replacement. The default skin consists of four colors: darkest, medium-high-dark. medium-low-dark, and light,
    // Which are defined by the exact RGB values defined below. We will be finding all of these colors (fromColorSwap) and replacing them with a new color 
    // (toColorSwap) that the player customized. The colors below msut be exact so the code can find and replace them!
    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255); // The darkest color on the default skin
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255); // The middle-high-dark color on the default skin
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255); // The middle-low-dark color on the default skin
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255); // The lightest color on the default skin

    // When the player GameObject is Awake (at the beginning of the game), we will imediately initialize the color swap list and process the customization
    private void Awake()
    {
        // Initialize the color swap list that we will fill with all of the color swaps we want to initiate
        colorSwapList = new List<colorSwap>();

        // Process the customization - process the gender, shirt, arms, and then merge them all together
        ProcessCustomization();
    }


    // I added this method to redo our player gender customization from the pause screen! This change gender method will be called from the change gender buttons to change our shirt color
    public void ChangeGender(int sexNo)
    {
        // Change the input gender number
        inputSex = sexNo;

        // Process the customization - process the gender, shirt, arms, trousers, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to redo our player shirt customization from the pause screen! This change shirt method will be called from the change shirt buttons to change our shirt color
    public void ChangeShirt(int shirtNo)
    {
        // Change the input shirt style
        inputShirtStyleNo = shirtNo;

        // Process the customization - process the gender, shirt, arms, trousers, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to redo our player hair customization from the pause screen! This change hair method will be called from the change hair buttons to change our hair style
    public void ChangeHair(int hairNo)
    {
        // Change the input shirt style
        inputHairStyleNo = hairNo;

        // Process the customization - process the gender, shirt, arms, trousers, hair and then merge them all together
        RedoCustomizations();
    }

    
    // I added this method to change the players red trouser color dynamically from a red slider in the pause menu customization tab 
    public void ChangeTrousersRed(System.Single newRed)
    {
        inputTrouserColor.r = newRed / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players green trouser color dynamically from a green slider in the pause menu customization tab 
    public void ChangeTrousersGreen(System.Single newGreen)
    {
        inputTrouserColor.g = newGreen / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players blue trouser color dynamically from a blue slider in the pause menu customization tab 
    public void ChangeTrousersBlue(System.Single newBlue)
    {
        inputTrouserColor.b = newBlue / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players red hair color dynamically from a red slider in the pause menu customization tab 
    public void ChangeHairRed(System.Single newRed)
    {
        inputHairColor.r = newRed / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and hair, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players green hair color dynamically from a green slider in the pause menu customization tab 
    public void ChangeHairGreen(System.Single newGreen)
    {
        inputHairColor.g = newGreen / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and hair, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players blue hair color dynamically from a blue slider in the pause menu customization tab 
    public void ChangeHairBlue(System.Single newBlue)
    {
        inputHairColor.b = newBlue / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, and hair, and then merge them all together
        RedoCustomizations();
    }


    // I added this method to redo all of the processing for our player customization from the pause screen! 
    // This method is called from the change shirt method, change trousers methods, etc after they change the customization variables from the pause screen
    // buttons and sliders
    public void RedoCustomizations()
    {
        // Initialize the color swap list that we will fill with all of the color swaps we want to initiate
        colorSwapList = new List<colorSwap>();

        // Process the customization - process the gender, shirt, arms, and then merge them all together
        ProcessCustomization();
    }


    // This method is in charge of processing all of the customizations that we can make, in turn
    private void ProcessCustomization()
    {
        // This method will find the base farmer texture based on the input gender selected, and then apply all of the contained pixels to the custom farmer texture,
        // ready to apply customizations from the following methods
        ProcessGender();

        // This method will process the user-selected shirt, and create a new Texture (sprite sheet) containing all of the proper shirts corresponding to each
        // player sprite in the base farmer texture, with the correct facing direction and x/y offset. This texture will later be drawn over the base
        // farmer texture to add the new shirt
        ProcessShirt();

        // This method will find all of the colors in the base farmer texture arm sprites that need to be recolored, and then apply
        // the swapped colors corresponding to the chosen shirt
        ProcessArms();

        // This will take care of recoloring the trousers to what the player customized, via a simply tint over the base gray trouser sprites
        ProcessTrousers();

        // This will create a new customized Hair texture containing only the users selected hairstyle, recolored to the user-selected color
        ProcessHair();

        // This will take care of recoloring the players skin (face and hands) using a color swap list
        ProcessSkin();

        // This method will simply take the new customized shirt texture (farmerBaseShirtsUpdated) and trousers, and merge them
        // into the base naked farmer texture to create our final farmer texture, farmerBaseCustomized, that will be used in gameplay, now
        // colored with new shirt, arms, trousers, etc.
        MergeCustomizations();
    }


    // Select the base farmer texture based on the input gender (for now both is male), and then apply all of the pixels within it 
    // to the customized farmer texture, to be the building block for all subsequent customizations
    private void ProcessGender()
    {
        // Set the base Sprite Sheet (farmerBaseTexture) that we will be drawing everything customized on top of. Select this by gender, as populated 
        // with male and female sprite sheets in the editor
        if (inputSex == 0)
        {
            farmerBaseTexture = maleFarmerBaseTexture;
        }
        else if (inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }

        // Get the base pixels in the farmerBaseTexture, and populate it in this Color array (pixel value at every location in the base texture (sprite sheet)
        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        // Set our new farmerBaseCustomized texture (this will be the finalized farmer texture) to the same pixels as the base texture, so we can update it in later methods
        farmerBaseCustomized.SetPixels(farmerBasePixels);
        farmerBaseCustomized.Apply();
    }


    // This method populates the facing directions and x/y offsets of each character sprite in the base farmer texture (sprite sheet), creates a new
    // shirt texture corresponding to the user-chosen shirt style, and then creates a new shirt texture sheet of the same size as the 
    // base farmer texture (sprite sheet) containing all of the corresponding shirts to draw over it, with the proper facing direction and x/y offset
    private void ProcessShirt()
    {
        // Initialize the body facing direction shirt array, with the size of body rows and columns declared at the beginning
        // Each element will be populated with the Facing enum determining which direction the player is facing in that sprite sheet element
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        // Populate the body facing shirt array (manually added the direction the player sprite is facing in each sprite in the 
        // 6x21 texture (sprite sheet)
        PopulateBodyFacingArray();

        // Initialize the body shirt x/y offset array, with the size of body rows and columns declared at the beginning
        // Each element will be populated with the y-offset of that particular sprite (i.e. for player bobbing up and down, etc.)
        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        // Populate the body shirt offset array (manually added the x and y offsets each player sprite has from it's sprite box, in the 6x21 texture (sprite sheet)
        PopulateBodyShirtOffsetArray();

        // Create the selected shirt texture (sprite sheet). This method goes to the shirts texture, and creates a new selectedShirt texture containing only
        // the 4 sprites corresponding to the shirt we selected (inputShirtStyleNo)
        AddShirtToTexture(inputShirtStyleNo);

        // Apply shirt texture to the base. This method will basically create a shirt texture (sprite sheet) with the same dimensions
        // as the base farmer texture, with the properly drawn facing directions and x/y offsets. This will later be drawn over the base farmer texture
        ApplyShirtTextureToBase();
    }


    // This method will decide which colors need to be swapped out in the arm sprites, and then change the base colors into the colorSwap toColors, and apply
    // them to the farmerBaseTexture.
    private void ProcessArms()
    {
        // Get the arm pixels that we want to to recolor from the base texture. This selects the entire block of pixels containing
        // arms that need to be recolored
        Color[] farmerPixelsToRecolor = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);

        // Populate the arm color swap list with the from and to colors we want to swap
        PopulateArmColorSwapList();

        // Change the arm colors. Given the block of arm pixels that we want to recolor, and the colorSwapList populated above,
        // this method will swap all of the colors in farmerPixelsToRecolor for the ones detailed in colorSwapList
        ChangePixelColors(farmerPixelsToRecolor, colorSwapList);

        // Set the recolored pixels to the updated farmerPixelsToRecolor
        farmerBaseCustomized.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolor);

        // Apply the texture changes to the farmer texture
        farmerBaseCustomized.Apply();
    }


    // This method changes the Trouser color to the one customized by the user directly on the final farmerBaseCustomized texture
    private void ProcessTrousers()
    {
        // Get the trouser pixels to recolor from the base naked farmer texture, by selecting the block containing all trouser sprites, put it into a Color array
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);

        // Change the trouser color to the customized color in the farmerTrouserPixels variable
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);

        // Set the changed trouser pixels onto the final customized Texture, farmerBaseCustomized, in the same Texture sprite locations as they were taken from the base farmer texture
        farmerBaseCustomized.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);

        // Apply the new texture changes to the farmerBaseCustomized texture, which is the one used by Unity to draw the character!
        farmerBaseCustomized.Apply();
    }


    // This method will grab the user-selected hairstyles from the base hairstyle sheet, add them to a new hairCustomized sheet that the game will use, and tint
    // Them all with the users selected hair color
    private void ProcessHair()
    {
        // Create the selected hair texture. Basically takes the user-selected hairstyles from the base hairstyle texture, and add them to a new customizedHairstyle texture that the game will use
        AddHairToTexture(inputHairStyleNo);

        // Get all of the hair pixels from the newly updated hairCustomized that we will need to recolor
        Color[] farmerSelectedHairPixels = hairCustomized.GetPixels();

        // Tint the hair pixels, like we did for the trousers
        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

        // Apply the colored, customizedhair styles to the hairCustomized Texture
        hairCustomized.SetPixels(farmerSelectedHairPixels);
        hairCustomized.Apply();
    }


    // This method will find all of the skin pixels to be recolored, then populates a color swap list for the 4 colors present in the 
    // Farmers skin, swap them for new colors, and then apply the color swaps to the selected skin sprites in the customized farmer texture
    private void ProcessSkin()
    {
        // Get the skin pixels that we want to to recolor from the base farmer texture. This selects the entire block of pixels containing
        // all skin that need to be recolored (heads, and arms)
        Color[] farmerPixelsToRecolor = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);

        // Populate the skin color swap list with the from and to colors we want to swap in the skin
        PopulateSkinColorSwapList(inputSkinType);

        // Change the skin colors. Given the block of arm pixels that we want to recolor, and the colorSwapList populated above,
        // this method will swap all of the colors in farmerPixelsToRecolor for the ones detailed in colorSwapList
        ChangePixelColors(farmerPixelsToRecolor, colorSwapList);

        // Set the recolored pixels to the updated farmerPixelsToRecolor
        farmerBaseCustomized.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolor);

        // Apply the texture changes to the farmer texture
       farmerBaseCustomized.Apply();
    }


    // This method takes the customized shirt and trouser Textures and merges them into the base naked farmer texture (sprite sheet) to add new shirts/trousers onto him when we play!
    private void MergeCustomizations()
    {
        // Get all of the farmer shirt pixels from the texture containing the 6x24 array of correctly-facing & x/y off-setted shirts, to be merged with the base naked farmer texture
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // Get the farmer trouser pixels, as updated in ProcessTrousers() into the farmerBaseCustomized sprite sheet (texture)
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomized.GetPixels(288, 0, 96, farmerBaseTexture.height);

        // Get the same farmer body pixels as the shirt ones above from the base farmer texture sheet - these are naked and we will merge the shirts ontop of them!
        Color[] farmerBodyPixels = farmerBaseCustomized.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // First merge the trouser pixels into the base naked farmer body pixels, and then the customized farmerShirtPixels we created earlier based on
        // player customization into the same base naked farmer texture. This will create a clothed customized farmer texture (sprite sheet)!
        MergeColorArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColorArray(farmerBodyPixels, farmerShirtPixels);

        // Paste the above merged pixels in farmerBodyPixels onto our final customized farmer texture, farmerBaseCustomized
        farmerBaseCustomized.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);

        // Apply the texture changes to the newly updated farmerBaseCustomized texture (sprite sheet)!
        farmerBaseCustomized.Apply();
    }


    // This method will simply manually populate the bodyFacingArray for every sprite in the 6x21 character array with the direction
    // the player is facing in that particular sprite (note there is no left - this will just be mirrored from right
    private void PopulateBodyFacingArray()
    {
        // Bottom row (row 0) of character sprites in the sprite sheet. The first 10 rows are all empty - so none
        bodyFacingArray[0, 0] = Facing.none;
        bodyFacingArray[1, 0] = Facing.none;
        bodyFacingArray[2, 0] = Facing.none;
        bodyFacingArray[3, 0] = Facing.none;
        bodyFacingArray[4, 0] = Facing.none;
        bodyFacingArray[5, 0] = Facing.none;

        // Next row up (row 1)
        bodyFacingArray[0, 1] = Facing.none;
        bodyFacingArray[1, 1] = Facing.none;
        bodyFacingArray[2, 1] = Facing.none;
        bodyFacingArray[3, 1] = Facing.none;
        bodyFacingArray[4, 1] = Facing.none;
        bodyFacingArray[5, 1] = Facing.none;

        // And so on and so forth..
        bodyFacingArray[0, 3] = Facing.none;
        bodyFacingArray[1, 3] = Facing.none;
        bodyFacingArray[2, 3] = Facing.none;
        bodyFacingArray[3, 3] = Facing.none;
        bodyFacingArray[4, 3] = Facing.none;
        bodyFacingArray[5, 3] = Facing.none;

        bodyFacingArray[0, 4] = Facing.none;
        bodyFacingArray[1, 4] = Facing.none;
        bodyFacingArray[2, 4] = Facing.none;
        bodyFacingArray[3, 4] = Facing.none;
        bodyFacingArray[4, 4] = Facing.none;
        bodyFacingArray[5, 4] = Facing.none;

        bodyFacingArray[0, 5] = Facing.none;
        bodyFacingArray[1, 5] = Facing.none;
        bodyFacingArray[2, 5] = Facing.none;
        bodyFacingArray[3, 5] = Facing.none;
        bodyFacingArray[4, 5] = Facing.none;
        bodyFacingArray[5, 5] = Facing.none;

        bodyFacingArray[0, 6] = Facing.none;
        bodyFacingArray[1, 6] = Facing.none;
        bodyFacingArray[2, 6] = Facing.none;
        bodyFacingArray[3, 6] = Facing.none;
        bodyFacingArray[4, 6] = Facing.none;
        bodyFacingArray[5, 6] = Facing.none;

        bodyFacingArray[0, 7] = Facing.none;
        bodyFacingArray[1, 7] = Facing.none;
        bodyFacingArray[2, 7] = Facing.none;
        bodyFacingArray[3, 7] = Facing.none;
        bodyFacingArray[4, 7] = Facing.none;
        bodyFacingArray[5, 7] = Facing.none;

        bodyFacingArray[0, 8] = Facing.none;
        bodyFacingArray[1, 8] = Facing.none;
        bodyFacingArray[2, 8] = Facing.none;
        bodyFacingArray[3, 8] = Facing.none;
        bodyFacingArray[4, 8] = Facing.none;
        bodyFacingArray[5, 8] = Facing.none;

        bodyFacingArray[0, 9] = Facing.none;
        bodyFacingArray[1, 9] = Facing.none;
        bodyFacingArray[2, 9] = Facing.none;
        bodyFacingArray[3, 9] = Facing.none;
        bodyFacingArray[4, 9] = Facing.none;
        bodyFacingArray[5, 9] = Facing.none;

        // The 11th row and up all have valid sprites in them, so now we just populate them with the direction
        // The player is facing in them. Note that the sheet only includes right (left is just mirrored from it)
        // To get these, just look at the base texture (sprite sheet), and just find the direction the player is facing in it
        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;

        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;

        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;
    }


    // This method manually populates all of the sprites in the 6x20 texture (sprite sheet) with the x/y-offsets that each player
    // sprite has (i.e. from the player bobbing while walking), so we can apply the same offset to the shirt we're drawing over it
    // The offsets are defined from the (0,0) pixel in the bottom left-corner of each sprite box, to the bottom left corner 
    // of the naked farmer body sprite in that same box
    private void PopulateBodyShirtOffsetArray()
    {
        // The first 10 elements are empty, so just add a 99,99 offset to make it clear
        bodyShirtOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyShirtOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 9] = new Vector2Int(99, 99);

        // The 11th and up rows have actual sprites, so simply add the x/y offsets for each sprite at the given locations
        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }


    // Given the user-selected shirt style number, this method finds the correct shirt sprites in the original shirts texture, and 
    // adds them to a new selectedShirt texture just containing the shirt sprites for the shirt style we selected
    private void AddShirtToTexture(int shirtStyleNo)
    {
        // Create the shirt texture which will hold the four 9x9 shirt sprites for the user-selected shirt style (so this texture is 9x36
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);

        // Set the filter mode so it doesn't add any anti-aliasing (pixel-perfect texture)
        selectedShirt.filterMode = FilterMode.Point;

        // Calculate the coordinates for the shirt pixels from the shirst sprite sheet (texture) that contains all of the customization options (so we can add JUST the selected
        // shirt to our new selectedShirt texture (sprite sheet)
        int y = (shirtStyleNo / shirtStylesInSpriteWidth) * shirtTextureHeight; // Calculate the row of this shirt style
        int x = (shirtStyleNo % shirtStylesInSpriteWidth) * shirtTextureWidth; // Calculate the column of this shirt style

        // Get the shirts pixels at the x,y position in the bottom left corner of the sprites, and then add the texture width and height
        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        // Apply the selected shirt pixels to the new selected shirt texture
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }


    // This method will create a new Texture farmerBaseShirtsUpdated containing a 6x21 grid of sprites containing only the selected shirt in the proper
    // facing direction and x/y offset found in the base farmer texture. This sheet will then later be drawn over the base farmer texture
    private void ApplyShirtTextureToBase()
    {
        // Create a new shirt base texture, containing all of the shirts (including the facing direction and (x,y) offset) for each naked farmer sprite in the
        // farmer Texture. This texture will be overlaid over the base naked farmer one, and it is the same size as it (of course)!!
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);

        // Pixel perfect rather than anti-aliased
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;

        // Set the shirt base texture to transparent so we can draw over it, and leave the surrounding pixels as clear once the other ones are drawn
        SetTextureToTransparent(farmerBaseShirtsUpdated);

        // Create color arrays for each of the front/back/right facing shirts
        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

        // Populate the color arrays for the front/back/right facing shirts, from the selectedShirt texture (sprite sheet) previously created
        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        // Loop through all of the the base texture sprite grid boxes, and apply the shirt pixels with the proper facing direction and x,y offset to each sprite in the 6x21 sprite array
        for (int x = 0; x < bodyColumns; x++)
        {
            for (int y = 0; y < bodyRows; y++)
            {
                // This calculates the actual x,y pixel value of the (x,y) sprite location in the grid, by multiplying them by the farmer sprite height/width
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                // If there is a x/y offset value for this sprite grid position, add it to our pixelX/pixelY values to add an offset to the shirt sprite we're drawing
                if (bodyShirtOffsetArray[x, y] != null)
                {
                    // If both the x/y offsets are 99, we know this is a null sprite - don't do anything
                    if (bodyShirtOffsetArray[x, y].x == 99 && bodyShirtOffsetArray[x, y].y == 99)
                    {
                        continue;
                    }

                    // Add the x/y offsets to our pixel location so we have the proper offset when drawing the shirt. This is the exact position we will be drawing the shirt sprite at
                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }

                // Check the facing direction for the current sprite we are looking at, and apply the proper shirt facing direction sprite to that grid box
                switch (bodyFacingArray[x, y])
                {
                    // If there is no facing direction (i.e. the blank sprites in the sheet), draw nothing
                    case Facing.none:
                        break;

                    case Facing.front:
                        // Populate the front-facing pixels with the frontShirtPixels at pixel x,y, with the proper shirt sprite width/height
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;

                    case Facing.back:
                        // Populate the back-facing pixels with the backShirtPixels at pixel x,y, with the proper shirt sprite width/height
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;

                    case Facing.right:
                        // Populate the right-facing pixels with the rightShirtPixels at pixel x,y, with the proper shirt sprite width/height
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;

                    default:
                        break;
                }
            }
        }

        // Apply the new shirt texture pixels that we updated above to the farmerBaseShirtsUpdated texture (sprite sheet)
        // This will be a sprite sheet (texture) containing all of the shirts (including proper facing direction and x/y offset) to be drawn over the base naked farmer texture
        farmerBaseShirtsUpdated.Apply();
    }


    // This method takes in a texture (i.e. the farmerBaseShirtsUpdated to contain all of the offsetted, and correctly-facing shirts to overlay on the 
    // base naked farmer), and fills it with all transparent pixels so we can draw on top of it, and leave the surrounding pixels to be clear
    private void SetTextureToTransparent(Texture2D texture2D)
    {
        // Create a new color array with the total number of pixels equal to the total in the passed-in texture
        Color[] fill = new Color[texture2D.height * texture2D.width];

        // Loop through the array and set every pixel to clear
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }

        // Set all the pixels with that clear fill array to the texture passed in, so now we have a completely clear texture (i.e. farmerBaseShirtsUpdated)
        texture2D.SetPixels(fill);
    }


    // This method just adds the three arm color swaps to add to the colorSwapList, which will be used to swap old base shirt colors to new customized shirt colors
    private void PopulateArmColorSwapList()
    {
        // clear out the color swap list
        colorSwapList.Clear();

        // Set up the replacement colors (toColor) in the colorSwap list, with the already-filled
        // fromColors (for the dark, medium, and light colors), and selected pixels from the 
        // selectedShirt sprite. This list will be used to swap the fromColor to the toColor.
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }


    // This method will loop through all of the pixels in the block of base farmer texture arms that we want to recolor, as well as our colorSwapList of colors to swap
    // and then swap the fromColors to the toColors if they match
    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        // Loop through all of the pixels in the base array (the block from the base farmer texture of arms that we want to recolor) 
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (colorSwapList.Count > 0)
            {
                // If we have a colorSwapList, loop through all of the colorSwaps we want to apply
                for (int j = 0; j < colorSwapList.Count; j++)
                {
                    // If the current pixel in the base array has the SAME color as the fromColor in the current colorSwapList entry,
                    // swap it out with the toColor in the colorSwapList entry
                    if (IsSameColor(baseArray[i], colorSwapList[j].fromColor))
                    {
                        baseArray[i] = colorSwapList[j].toColor;
                    }
                }
            }
        }
    }


    // This method simply checks if two colors are the same. It will be used to see if the base color matches the fromColor. If so, switch the baseColor to the toColor
    private bool IsSameColor(Color color1, Color color2)
    {
        // Check if the R, G, B, and alpha values match between the color1 and color2
        if ((color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b) && (color1.a == color2.a))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // This method will merge the mergeArray (i.e. the sheet of shirt sprites) onto the baseArray (i.e. the base naked farmer texture) to add shirt colors, etc onto it.
    private void MergeColorArray(Color[] baseArray, Color[] mergeArray)
    {
        // Loop through all of the pixels in the base array (i.e. the block from the base farmer texture of farmer naked bodies) 
        for (int i = 0; i < baseArray.Length; i++)
        {
            // If the merging array current pixel is transparent, do nothing for this pixel. If so, we will recolor the base array pixel
            if (mergeArray[i].a > 0)
            {
                // if the merge array pixel has alpha > 1 (it shouldn't! but this is a safety check), we will just fully replace the base pixel value with the merged on
                if (mergeArray[i].a >= 1)
                {
                    // Fully replace the base pixel with the merge pixel
                    baseArray[i] = mergeArray[i];
                }
                // Otherwise, if the merge array pixel is between 0 and 1 (which it should always be), Interpolate between the base and merge pixel colors to obtain the new value on the base array
                else
                {
                    float alpha = mergeArray[i].a;

                    // Blend the base and merge array colors at this pixel, scaled by the transparency of the merge array pixel
                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].b += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].g += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }


    // This method will tint a basePixelArray (i.e. the base gray farmer trousers in the base farmer texture) with a tintColor, which is
    // chosen by the user in the customization editor
    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        // Loop through all of the pixels in the basePixelArray
        for (int i = 0; i < basePixelArray.Length; i++)
        {
            // For each pixel in the array, multiply the current base RGB pixel values, to the tint Colors RGB values. This way every pixel in the trousers are
            // tinted with the tintColor
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }


    // This method is similar to the AddShirtTexture method, which will find all of the pixels corresponding to the players chosen hairstyle
    // from the base hairstyles texture, and then add them to a new blank texture for the customized hair that the game will use in play
    private void AddHairToTexture(int hairStyleNo)
    {
        // Calculate the coordinates for the hair pixels in the base hair textures
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight; // Calculate the row of this hair style
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth; // Calculate the column of this hair style

        // Get the hairs pixels into a color array at the x,y position in the bottom left corner of the sprites, and then add the texture width and height
        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);

        // Apply the selected shirt pixels to the new selected shirt texture hairCustomized, which is used in the game
        hairCustomized.SetPixels(hairPixels);
        hairCustomized.Apply();
    }


    // This method just adds the four skin color swaps to add to the colorSwapList, which will be used to swap old base skin colors to new customized farmer texture skin colors
    private void PopulateSkinColorSwapList(int skinType)
    {
        // clear out the color swap list
        colorSwapList.Clear();

        // Set up the replacement colors (toColor) in the colorSwap list, with the already-filled
        // fromColors (for the dark, medium-dark, medium-light and light skin colors), and then new colors
        // to replace them with depending on the skin color chosen by the player. This list will be used to swap the fromColor to the toColor in all of the skins 
        // in the final customized farmer texture
        switch (skinType)
        {
            // If the player chose skin type 0, just leave as the base (swap same colors)
            case 0:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
            // The other cases have new Colors to be swapped for other skins!
            case 1:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(213, 189, 167, 255)));
                break;
            case 2:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(145, 103, 26, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;
            case 3:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;
            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }
}

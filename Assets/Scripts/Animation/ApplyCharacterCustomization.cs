using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


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

[RequireComponent(typeof(GenerateGUID))]
public class ApplyCharacterCustomization : MonoBehaviour, ISaveable
{
    // Unique ID required by the ISaveable interface, will store the GUID attached to the CharacterCustomization gameObject
    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    // GameObjectSave required by the ISaveable interface, storesd the save data that is built up for every object that has the ISaveable interface attached
    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    // INPUT TEXTURES
    // Input Textures to be populated in the editor. The first two are the "naked farmer" textures that we will be drawing clothes over (for now, male = female, but later
    // we can add a female one). The next one is the set of shirt textures (i.e. green and red - maybe more in the future!) that we will be drawing over the naked farmer. The next one
    // is the set of all possible hairstyles the player can pick. The next one is the base texture for the "naked farmer". This will be set to the male or female one based on what we've 
    // selected. The next one is the base texture for the players different hat choices (currently only null and hat1). The last one is the base texture sheet containing all of the 
    // adornments - currently glasses and a beard. A texture is the sprite sheet filled with the sprites for every player direction etc
    [Header("Base Textures")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    [SerializeField] private Texture2D hatsBaseTexture = null;
    [SerializeField] private Texture2D adornmentsBaseTexture = null;
    private Texture2D farmerBaseTexture;

    // OUTPUT CREATED TEXTURES
    // Created textures. The first one is the target final texture that we have created with the character customizer, and will be used to draw over the naked base farmer.
    // The next one will be a texture (sprite sheet) of the customized shirts to be drawn over each of the naked player positions at the same sprite sheet locations. 
    // The next two are the final, customized hair and hat textures that the player chose, and colored to what the player picked. The next two are the 
    // set of shirts and adornments that we've selected (i.e. the red one or the green one.). The last two are the textures containing the shirt and adornment in all facing directions that the player chose.
    // The farmerBaseCustomized will be updated in this class, and is the one that is used by the animator to draw the player!
    [Header("Output Base Texture To Be Used For Animation")]
    [SerializeField] private Texture2D farmerBaseCustomized = null;
    [SerializeField] private Texture2D hairCustomized = null;
    [SerializeField] private Texture2D hatsCustomized = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D farmerBaseAdornmentsUpdated;
    private Texture2D selectedShirt;
    private Texture2D selectedAdornment;

    // List of toggles and sliders so we can control them from here when we load the game!
    [SerializeField] private Toggle[] sexToggles = null;
    [SerializeField] private Toggle[] shirtToggles = null;
    [SerializeField] private Toggle[] hairToggles = null;
    [SerializeField] private Toggle[] hatToggles = null;
    [SerializeField] private Toggle[] adornmentsToggles = null;

    [SerializeField] private Slider[] trouserSliders = null;
    [SerializeField] private Slider[] hairSliders = null;
    [SerializeField] private Slider[] skinSliders = null;

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
    
    // Select the hat style with a slider (0 - no hat, 1 - hat), populated in the editor
    // The no-hat option will simply grab empty sprites from the base hat texture - so it will show up as nothing on head
    [Header("Select Hat Style: 0 = no hat, 1 = hat")]
    [Range(0, 1)]
    [SerializeField] private int inputHatStyleNo = 0;

    // Select the adornment style with a slider (0 - none, 1 - glasses, 2 - beard), populated in the editor
    [Header("Select Adornments Style: 0 = no adornments, 1 = glasses, 2 = beard")]
    [Range(0, 2)]
    [SerializeField] private int inputAdornmentsStyleNo = 0;

    // Select the hair color from an RGB color picker
    [Header("Select Hair Color")]
    [SerializeField] private Color inputHairColor = Color.black;

    // Select the skin color from an RGB color picker
    [Header("Select Skin Color (if 4, use color picker)")]
    [Range(0, 4)]
    [SerializeField] private int inputSkinType = 4;

    [SerializeField]
    private Color inputSkinColor = new Color32(207, 166, 128, 255);

    // The ranges used in the skin sliders, so we can convert to the proper normalized values
    private int skinRangeR = 223 - 63;
    private int skinRangeG = 225 - 50;
    private int skinRangeB = 228 - 39;

    // Select the gender (0 - male, 1 - female), populated in the editor (right now both male and female are the same)
    [Header("Select Sex: 0 = Male, 1 = Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    // Select the trouser color from an RGB color picker
    [Header("Select Trouser Color")]
    [SerializeField] private Color inputTrouserColor = Color.blue;

    // 2D Array of enums storing the different directions the player could be facing, so we can always apply the correct shirt over it
    // Also, 2D arrays of Vector2Ints for the shirt offsets and adornments offsets to be drawn on the naked farmer, i.e. as he bobs up and down while running
    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;
    private Vector2Int[,] bodyAdornmentsOffsetArray;

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

    private int hatTextureWidth = 20; // height and width of a selected hat texture, in the final customized hat texture (each hat style has 4 20x20 views of the same hat, and can hold up to 6 in the vertical direction). There is room for 12 columns (12 hats) horizontally in the input, base, texture to add later, if wanted
    private int hatTextureHeight = 80;
    private int hatStylesInSpriteWidth = 12;

    private int adornmentTextureWidth = 16; // Texture width/height of a given adornment output sprite (column of 2 16x16 sprites, one for forward and one for side)
    private int adornmentTextureHeight = 32;
    private int adornmentStylesInSpriteWidth = 8; // number of adornments that can fit in the sprite sheet
    private int adornmentSpriteWidth = 16; // 16x16 adorment sizes
    private int adornmentSpriteHeight = 16;

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

        // Get the unique ID for the GameObject
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        // Initialize the GameObjectSave variable
        GameObjectSave = new GameObjectSave();
    }


    // On enable, this will just register this gameObject as an ISaveable, so that the SaveLoadManager can save/load the methods set up here
    private void OnEnable()
    {
        // Registers this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableRegister();
    }


    // Deregister from the iSaveableObjectList
    private void OnDisable()
    {
        // Deregisters this game object within the iSaveableObjectList, which is looped through in the SaveLoadManager for all objects to save/load the saved items
        ISaveableDeregister();
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


    // I added this method to redo our player hat customization from the pause screen! This change hat method will be called from the change hat buttons to change our hat style
    public void ChangeHat(int hatNo)
    {
        // Change the input hat style
        inputHatStyleNo = hatNo;

        // Process the customization - process the gender, shirt, arms, trousers, hair, skin, and hat and then merge them all together
        RedoCustomizations();
    }


    // I added this method to redo our player hat customization from the pause screen! This change adornments method will be called from the change adornments buttons to change our adornments style
    public void ChangeAdornments(int adornmentNo)
    {
        // Change the input hat style
        inputAdornmentsStyleNo = adornmentNo;

        // Process the customization - process the gender, shirt, arms, trousers, hair, skin, hat, and adornments, and then merge them all together
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


    // I added this method to change the players red skin color dynamically from a red slider in the pause menu customization tab 
    public void ChangeSkinRed(System.Single newRed)
    {
        inputSkinColor.r = newRed / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, hair, and skin and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players green skin color dynamically from a red slider in the pause menu customization tab 
    public void ChangeSkinGreen(System.Single newGreen)
    {
        inputSkinColor.g = newGreen / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, hair, and skin and then merge them all together
        RedoCustomizations();
    }


    // I added this method to change the players blue skin color dynamically from a red slider in the pause menu customization tab 
    public void ChangeSkinBlue(System.Single newBlue)
    {
        inputSkinColor.b = newBlue / 255f;

        // Process the customization - process the gender, shirt, arms, trousers, hair, and skin and then merge them all together
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

        // This will take care of changing the hat sprite on the player GameObject, depending on the players choice
        ProcessHat();

        // This method will take care of changing the adornment sprites on the player, depending on the players choice. These adornments 
        // are drawn directly on top of the base players customized texture that is displayed in game
        ProcessAdornments();

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
        // Get the skin pixels that we want to to recolor from the customized farmer texture (we use this one so we can grab the already-updated farmers sleeves.
        // This selects the entire block of pixels containing all skin that need to be recolored (heads, and arms)
        Color[] farmerPixelsToRecolor = farmerBaseCustomized.GetPixels(0, 0, 288, farmerBaseCustomized.height);

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


    // This method will process the players hat choice, extracting the proper hat sprites from the base hat texture, creating a new customized hats texture to be used in game
    private void ProcessHat()
    {
        // Create the selected hat texture to be used by the game with the players hat choice
        AddHatToTexture(inputHatStyleNo);
    }


    // This method will process the adornments that were selected by the user. It will first calculate the offsets of the players face in each sprite on the base farmer texture,
    // then create a new selectedAdornment texture with the user-selected adornment, and then it will overlay it ontop of the base customized player texture, so it will all be drawn together
    private void ProcessAdornments()
    {
        // Initialize the body adornments x/y offset array, with the size of body rows and columns declared at the beginning
        // Each element will be populated with the y-offset of that particular sprite (i.e. for player bobbing up and down, etc.)
        bodyAdornmentsOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        // Populate the body adornments offset array (manually added the x and y offsets each player sprite has from it's sprite box, in the 6x21 texture (sprite sheet)
        // This will allow us to correctly place the adornments on the players face
        PopulateBodyAdornmentsOffsetArray();

        // Create the selected adornments texture (sprite sheet). This method goes to the base adornments texture, and creates a new selectedAdornment texture containing only
        // the 2 sprites corresponding to the adornment we selected (inputAdornmentStyleNo)
        AddAdornmentsToTexture(inputAdornmentsStyleNo);

        // Create the adornments texture which will hold the 2 16x16 shirt sprites for the user-selected adornment style (so this texture is 16x32
        farmerBaseAdornmentsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);

        // Set the filter mode so it doesn't add any anti-aliasing (pixel-perfect texture)
        farmerBaseAdornmentsUpdated.filterMode = FilterMode.Point;

        // First set the entire farmerBaseAdornmentsUpdated texture to transparent so we can draw the selected adornments over it, and then write it over the 
        // customized farmer texture
        SetTextureToTransparent(farmerBaseAdornmentsUpdated);

        // Apply the selected adornment texture to the base customized texture. This method will basically create an adornment texture (sprite sheet) with the same dimensions
        // as the base customized farmer texture, with the properly drawn facing directions and x/y offsets. This will later be drawn over the base farmer customized texture
        ApplyAdornmentsTextureToBase();
    }


    // This method takes the customized shirt and trouser Textures and merges them into the base naked farmer texture (sprite sheet) to add new shirts/trousers onto him when we play!
    private void MergeCustomizations()
    {
        // Get all of the farmer shirt pixels from the texture containing the 6x24 array of correctly-facing & x/y off-setted shirts, to be merged with the base naked farmer texture
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // Get the farmer trouser pixels, as updated in ProcessTrousers() into the farmerBaseCustomized sprite sheet (texture)
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomized.GetPixels(288, 0, 96, farmerBaseTexture.height);

        // Get the farmer adornments pixels, as updated in ProcessAdornments() into the farmerBaseCustomized sprite sheet (texture)
        Color[] farmerAdornmentsPixelsSelection = farmerBaseAdornmentsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // Get the same farmer body pixels as the shirt ones above from the base farmer texture sheet - these are naked and we will merge the shirts ontop of them!
        Color[] farmerBodyPixels = farmerBaseCustomized.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        // First merge the trouser pixels into the base naked farmer body pixels, and then the customized farmerShirtPixels, and then the customized farmerAdornmentsPixels we created earlier based on
        // player customization or shirt, trousers, and adornmentsinto the same base naked farmer texture. This will create a clothed customized farmer texture (sprite sheet)!
        MergeColorArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColorArray(farmerBodyPixels, farmerShirtPixels);
        MergeColorArray(farmerBodyPixels, farmerAdornmentsPixelsSelection);

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
            // This cases uses the color picker!! Use these color swaps so we can change the base color (3) with the sliders, and the other 3 shades
            // change according to the value differences in the base skin sprite. We can only adjust the sliders so much so that these values don't loop back around to 255 when they get too low!
            // The min/max values allowable so this doesn't happen is set manually in the editor for the sliders
            case 4:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32((byte)((inputSkinColor.r * 255) - 62), (byte)((inputSkinColor.g * 255) - 49), (byte)((inputSkinColor.b * 255) - 38), 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32((byte)((inputSkinColor.r * 255) - 3), (byte)((inputSkinColor.g * 255) - 11), (byte)((inputSkinColor.b * 255) - 20), 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, inputSkinColor));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32((byte)((inputSkinColor.r * 255) + 31), (byte)((inputSkinColor.g * 255) + 29), (byte)((inputSkinColor.b * 255) + 26), 255)));
                break;
            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }



    // This method will simply take the hat sprites from the base hat texture containing all hat styles, corresponding to the players hat choice, and then create
    // a new customized hats texture containing only the hat sprites corresponding to that choice, to be used by the game
    private void AddHatToTexture(int hatStyleNo)
    {
        // Calculate the coordinates for the hat pixels that we have selected, in the base hat texture containing all of the different styles
        int y = (hatStyleNo / hatStylesInSpriteWidth) * hatTextureHeight; // Calculate the row of this hat style
        int x = (hatStyleNo % hatStylesInSpriteWidth) * hatTextureWidth; // Calculate the column of this hat style

        // Get the hat pixels from the base hat texture
        Color[] hatPixels = hatsBaseTexture.GetPixels(x, y, hatTextureWidth, hatTextureHeight);

        // Apply the selected hat pixels to the new customized hat texture, containing only the selected hat to be used in hame
        hatsCustomized.SetPixels(hatPixels);
        hatsCustomized.Apply();
    }


    // This method manually populates all of the sprites in the 6x20 texture (sprite sheet) with the x/y-offsets that each player
    // sprite has (i.e. from the player bobbing while walking), so we can apply the same offset to the shirt we're drawing over it
    // The offsets are defined from the (0,0) pixel in the bottom left-corner of each sprite box, to the bottom left corner 
    // of the naked farmer body sprite in that same box
    private void PopulateBodyAdornmentsOffsetArray()
    {
        // The first 10 elements are empty, so just add a 99,99 offset to make it clear
        bodyAdornmentsOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 0] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 1] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 2] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 2] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 3] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 3] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 4] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 5] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 5] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 6] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 6] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 7] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 7] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 8] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 8] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 9] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 9] = new Vector2Int(99, 99);

        // The 11th and up rows have actual sprites, so simply add the x/y offsets for each sprite at the given locations
        // The ones with 99,99 are backwards facing! No adornments to be drawn
        bodyAdornmentsOffsetArray[0, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 10] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 10] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[4, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[5, 10] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[1, 11] = new Vector2Int(0, 2 + 16);
        bodyAdornmentsOffsetArray[2, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[3, 11] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 11] = new Vector2Int(99, 11);
        bodyAdornmentsOffsetArray[5, 11] = new Vector2Int(99, 12);

        bodyAdornmentsOffsetArray[0, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 12] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 12] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[3, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[4, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 12] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 13] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 13] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[3, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentsOffsetArray[4, 13] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 13] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 14] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 14] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 14] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[4, 14] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 14] = new Vector2Int(0, 1 + 16);

        bodyAdornmentsOffsetArray[0, 15] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[1, 15] = new Vector2Int(0, -5 + 16);
        bodyAdornmentsOffsetArray[2, 15] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 15] = new Vector2Int(0, 1 + 16);
        bodyAdornmentsOffsetArray[4, 15] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 15] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 16] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 16] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[3, 16] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[4, 16] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 16] = new Vector2Int(0, 0 + 16);

        bodyAdornmentsOffsetArray[0, 17] = new Vector2Int(0, -3 + 16);
        bodyAdornmentsOffsetArray[1, 17] = new Vector2Int(0, -2 + 16);
        bodyAdornmentsOffsetArray[2, 17] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 17] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 17] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 17] = new Vector2Int(99, 99);

        bodyAdornmentsOffsetArray[0, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[1, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[2, 18] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[3, 18] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 18] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 18] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[4, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[5, 19] = new Vector2Int(0, -1 + 16);

        bodyAdornmentsOffsetArray[0, 20] = new Vector2Int(0, 0 + 16);
        bodyAdornmentsOffsetArray[1, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[2, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentsOffsetArray[3, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[4, 20] = new Vector2Int(99, 99);
        bodyAdornmentsOffsetArray[5, 20] = new Vector2Int(99, 99);
    }


    // Given the user-selected adornments style number, this method finds the correct adornments sprites in the original adornments texture, and 
    // adds them to a new selectedAdornments texture just containing the adornments sprites for the adornments style we selected
    private void AddAdornmentsToTexture(int adornmentStyleNo)
    {
        // Create the adornment texture which will hold the 2 9x9 adornment sprites for the user-selected adornment style (so this texture is 32x16)
        selectedAdornment = new Texture2D(adornmentTextureWidth, adornmentTextureHeight);

        // Set the filter mode so it doesn't add any anti-aliasing (pixel-perfect texture)
        selectedAdornment.filterMode = FilterMode.Point;

        // Calculate the coordinates for the adornment pixels from the adornment sprite sheet (texture) that contains all of the customization options (so we can add JUST the selected
        // adornment to our new selectedAdornment texture (sprite sheet)
        int y = (adornmentStyleNo / adornmentStylesInSpriteWidth) * adornmentTextureHeight; // Calculate the row of this adornment style
        int x = (adornmentStyleNo % adornmentStylesInSpriteWidth) * adornmentTextureWidth; // Calculate the column of this adornment style

        // Get the adornment pixels at the x,y position in the bottom left corner of the sprites, and then add the texture width and height. Add these into a Color array
        Color[] adornmentPixels = adornmentsBaseTexture.GetPixels(x, y, adornmentTextureWidth, adornmentTextureHeight);

        // If we have a beard, tint the beardhair pixels to the same color as our hair selection, like we did for the trousers
        if (adornmentStyleNo == 2)
        {
            // First swap the standard brown beard pixels for a grayscale set
            colorSwapList.Clear();
            colorSwapList.Add(new colorSwap(new Color32(94, 63, 5, 255), new Color32(71, 71, 71, 255)));
            colorSwapList.Add(new colorSwap(new Color32(143, 94, 4, 255), new Color32(187, 187, 187, 255)));
            ChangePixelColors(adornmentPixels, colorSwapList);

            // Then tint the grayscale beard with the input hair color!
            TintPixelColors(adornmentPixels, inputHairColor);
        }

        // Apply the selected adornment pixels to the new selected adornment texture
        selectedAdornment.SetPixels(adornmentPixels);
        selectedAdornment.Apply();
    }


    // This method will create a new Texture farmerBaseAdornmentsUpdated texture containing a 6x21 grid of sprites containing only the selected adornments in the proper
    // facing direction and x/y offset found in the base customized farmer texture. This sheet will then later be drawn over the base farmer texture
    private void ApplyAdornmentsTextureToBase()
    {
        // Create color arrays for each of the front/side facing shirts
        Color[] frontAdornmentsPixels;
        Color[] rightAdornmentsPixels;

        // Populate the color arrays for the front/back/right facing adornments, from the selectedAdornment texture (sprite sheet) previously created with the final adornment texture
        frontAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentSpriteHeight * 1, adornmentSpriteWidth, adornmentSpriteHeight);
        rightAdornmentsPixels = selectedAdornment.GetPixels(0, adornmentSpriteHeight * 0, adornmentSpriteWidth, adornmentSpriteHeight);

        // Loop through all of the the base texture sprite grid boxes, and apply the adornments pixels with the proper facing direction and x,y offset to each sprite in the 6x21 sprite array
        for (int x = 0; x < bodyColumns; x++)
        {
            for (int y = 0; y < bodyRows; y++)
            {
                // This calculates the actual x,y pixel value of the (x,y) sprite location in the grid, by multiplying them by the farmer sprite height/width
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;

                // If there is a x/y offset value for this sprite grid position, add it to our pixelX/pixelY values to add an offset to the adornment sprite we're drawing
                if (bodyAdornmentsOffsetArray[x, y] != null)
                {
                    //If both the x/y offsets are 99, we know this is a null sprite - don't do anything
                    if (bodyAdornmentsOffsetArray[x, y].x == 99 && bodyAdornmentsOffsetArray[x, y].y == 99)
                    {
                        continue;
                    }

                    // Add the x/y offsets to our pixel location so we have the proper offset when drawing the adornment. This is the exact position we will be drawing the adornment sprite at
                    pixelX += bodyAdornmentsOffsetArray[x, y].x;
                    pixelY += bodyAdornmentsOffsetArray[x, y].y;
                }

                // Check the facing direction for the current sprite we are looking at, and apply the proper adornment facing direction sprite to that grid box
                switch (bodyFacingArray[x, y])
                {
                    // If there is no facing direction (i.e. the blank sprites in the sheet), draw nothing
                    case Facing.none:
                        break;

                    case Facing.front:
                        // Populate the front-facing pixels with the frontAdornmentPixels at pixel x,y, with the proper Adornment sprite width/height
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentSpriteWidth, adornmentSpriteHeight, frontAdornmentsPixels);
                        break;

                    case Facing.right:
                        // Populate the right-facing pixels with the rightAdornmentixels at pixel x,y, with the proper Adornment sprite width/height
                        farmerBaseAdornmentsUpdated.SetPixels(pixelX, pixelY, adornmentSpriteWidth, adornmentSpriteHeight, rightAdornmentsPixels);
                        break;

                    default:
                        break;
                }
            }
        }

        // Apply the new adornments texture pixels that we updated above to the farmerBaseAdornmentsUpdated texture (sprite sheet)
        // This will be a sprite sheet (texture) containing all of the adornments (including proper facing direction and x/y offset) to be drawn over the base naked farmer texture
        farmerBaseAdornmentsUpdated.Apply();
    }


    // Required method by the ISaveable interface, which will be called OnEnable() of the CharacterCustomization GameObject, and it will 
    // Add an entry (of this gameObject) to the iSaveableObjectList in SaveLoadManager, which will then manage
    // Looping through all such items in this list to save/load their data
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }


    // Required method by the ISaveable interface, which will be called OnDisable() of the CharacterCustomization GameObject, and it will
    // Remove this item from the saveable objects list, as described above
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }


    // Required method by the ISaveable interface. This will get called from the SaveLoadManager, for each scene to save the dictionaries (GameObjectSave has a dict keyed by scene name)
    // This method will store the sceneData for the current scene (). It will then return a GameObjectSave, which just has a Dict of SceneSave data for each scene, keyed by scene name
    public GameObjectSave ISaveableSave()
    {
        // Delete the sceneData (dict of data to save in that scene, keyed by scene name) for the GameObject if it already exists in the persistent scene
        // which is where this data is going to be saved, so we can create a new one with updated dictionaries
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        // Create the SaveScene for this gameObject (keyed by the scene name, storing multiple dicts for bools, the scene the player ended in, the players location, the gridPropertyDetails,
        // the SceneItems, and the inventory items and quantities, and the gameYear, day, hour, minute, second, season, day of week)
        SceneSave sceneSave = new SceneSave();

        // Create a new int dictionary to store the customization parameters
        sceneSave.intDictionary = new Dictionary<string, int>();

        // Add values to the int dictionary for the different customization parameters, keyed so we can easily retrieve them in load
        sceneSave.intDictionary.Add("shirtStyleNo", inputShirtStyleNo);
        sceneSave.intDictionary.Add("hairStyleNo", inputHairStyleNo);
        sceneSave.intDictionary.Add("hatStyleNo", inputHatStyleNo);
        sceneSave.intDictionary.Add("adornmentsStyleNo", inputAdornmentsStyleNo);
        sceneSave.intDictionary.Add("skinType", inputSkinType);
        sceneSave.intDictionary.Add("sex", inputSex);
        sceneSave.intDictionary.Add("hairColorR", (int)(255f * inputHairColor.r));
        sceneSave.intDictionary.Add("hairColorG", (int)(255f * inputHairColor.g));
        sceneSave.intDictionary.Add("hairColorB", (int)(255f * inputHairColor.b));
        sceneSave.intDictionary.Add("skinColorR", (int)(255f * inputSkinColor.r));
        sceneSave.intDictionary.Add("skinColorG", (int)(255f * inputSkinColor.g));
        sceneSave.intDictionary.Add("skinColorB", (int)(255f * inputSkinColor.b));
        sceneSave.intDictionary.Add("trouserColorR", (int)(255f * inputTrouserColor.r));
        sceneSave.intDictionary.Add("trouserColorG", (int)(255f * inputTrouserColor.g));
        sceneSave.intDictionary.Add("trouserColorB", (int)(255f * inputTrouserColor.b));

        // Add the SceneSave data for the CharacterCustomization game object to the GameObjectSave, which is a dict storing all the dicts in a scene to be loaded/saved, keyed by the scene name
        // The time manager will get stored in the Persistent Scene
        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        // Return the GameObjectSave, which has a dict of the Saved stuff for the CharacterCustomization GameObject
        return GameObjectSave;
    }


    // This is a required method for the ISaveable interface, which passes in a GameObjectSave dictionary, and restores the current scene from it
    // The SaveLoadManager script will loop through all of the ISaveableRegister GameObjects (all registered with their ISaveableRegister methods), and trigger this 
    // ISaveableLoad, which will load that Save data (here for the persistent scene CharacterCustomization information, which includes the all of the customization parameters),
    // for each scene (GameObjectSave is a Dict keyed by scene name).
    public void ISaveableLoad(GameSave gameSave)
    {
        // gameSave stores a Dictionary of items to save keyed by GUID, see if there's one for this GUID (generated on the InventoryManager GameObject)
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            // Get the save data for the scene, if one exists for the PersistentScene (what the time info is saved under)
            if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                // If both the intDictionary (storing all of the character customization ints and colors)
                // exist, populate the saved values!
                if (sceneSave.intDictionary != null)
                {
                    // Check if the intDictionary contains entries for the customization parameters. If so, populate the them with the saved values
                    if (sceneSave.intDictionary.TryGetValue("shirtStyleNo", out int savedShirtStyleNo))
                    {
                        inputShirtStyleNo = savedShirtStyleNo;
                    }
                    if (sceneSave.intDictionary.TryGetValue("hairStyleNo", out int savedHairStyleNo))
                    {
                        inputHairStyleNo = savedHairStyleNo;
                    }
                    if (sceneSave.intDictionary.TryGetValue("hatStyleNo", out int savedHatStyleNo))
                    {
                        inputHatStyleNo = savedHatStyleNo;
                    }
                    if (sceneSave.intDictionary.TryGetValue("adornmentsStyleNo", out int savedAdornmentStyleNo))
                    {
                        inputAdornmentsStyleNo = savedAdornmentStyleNo;
                    }
                    if (sceneSave.intDictionary.TryGetValue("skinType", out int savedSkinStyleNo))
                    {
                        inputSkinType = savedSkinStyleNo;
                    }
                    if (sceneSave.intDictionary.TryGetValue("sex", out int savedSex))
                    {
                        inputSex = savedSex;
                    }
                    if (sceneSave.intDictionary.TryGetValue("hairColorR", out int savedHairColorR))
                    {
                        inputHairColor.r = (float) savedHairColorR / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("hairColorG", out int savedHairColorG))
                    {
                        inputHairColor.g = (float) savedHairColorG / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("hairColorB", out int savedHairColorB))
                    {
                        inputHairColor.b = (float) savedHairColorB / 255f;
                    }

                    if (sceneSave.intDictionary.TryGetValue("skinColorR", out int savedSkinColorR))
                    {
                        inputSkinColor.r = (float) savedSkinColorR / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("skinColorG", out int savedSkinColorG))
                    {
                        inputSkinColor.g = (float) savedSkinColorG / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("skinColorB", out int savedSkinColorB))
                    {
                        inputSkinColor.b = (float) savedSkinColorB / 255f;
                    }

                    if (sceneSave.intDictionary.TryGetValue("trouserColorR", out int savedTrouserColorR))
                    {
                        inputTrouserColor.r = (float) savedTrouserColorR / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("trouserColorG", out int savedTrouserColorG))
                    {
                        inputTrouserColor.g = (float) savedTrouserColorG / 255f;
                    }
                    if (sceneSave.intDictionary.TryGetValue("trouserColorB", out int savedTrouserColorB))
                    {
                        inputTrouserColor.b = (float) savedTrouserColorB / 255f;
                    }

                    // Process the customization - process the gender, shirt, arms, trousers, skin, hat, adornments and then merge them all together
                    RedoCustomizations();

                    // Set all of the toggles to the loaded values, depending on theier states
                    switch (inputSex)
                    { 
                        case 0:
                            sexToggles[0].isOn = true;
                            sexToggles[1].isOn = false;
                            break;
                        case 1:
                            sexToggles[0].isOn = false;
                            sexToggles[1].isOn = true;
                            break;
                    }

                    switch (inputShirtStyleNo)
                    {
                        case 0:
                            shirtToggles[0].isOn = true;
                            shirtToggles[1].isOn = false;
                            break;
                        case 1:
                            shirtToggles[0].isOn = false;
                            shirtToggles[1].isOn = true;
                            break;
                    }

                    switch (inputHairStyleNo)
                    {
                        case 1:
                            hairToggles[0].isOn = true;
                            hairToggles[1].isOn = false;
                            hairToggles[2].isOn = false;
                            break;
                        case 0:
                            hairToggles[0].isOn = false;
                            hairToggles[1].isOn = true;
                            hairToggles[2].isOn = false;
                            break;
                        case 2:
                            hairToggles[0].isOn = false;
                            hairToggles[1].isOn = false;
                            hairToggles[2].isOn = true;
                            break;
                    }

                    switch (inputHatStyleNo)
                    {
                        case 1:
                            hatToggles[0].isOn = true;
                            hatToggles[1].isOn = false;
                            break;
                        case 0:
                            hatToggles[0].isOn = false;
                            hatToggles[1].isOn = true;
                            break;
                    }

                    switch (inputAdornmentsStyleNo)
                    {
                        case 0:
                            adornmentsToggles[0].isOn = true;
                            adornmentsToggles[1].isOn = false;
                            adornmentsToggles[2].isOn = false;
                            break;
                        case 1:
                            adornmentsToggles[0].isOn = false;
                            adornmentsToggles[1].isOn = true;
                            adornmentsToggles[2].isOn = false;
                            break;
                        case 2:
                            adornmentsToggles[0].isOn = false;
                            adornmentsToggles[1].isOn = false;
                            adornmentsToggles[2].isOn = true;
                            break;
                    }

                    // Set the sliders to the saved values!
                    hairSliders[0].normalizedValue = inputHairColor.r;
                    hairSliders[1].normalizedValue = inputHairColor.g;
                    hairSliders[2].normalizedValue = inputHairColor.b;

                    // Skin sliders need extra care because the sliders don't encompass all 255 bytes! need to divide by the slider range rather than 255
                    skinSliders[0].normalizedValue = (float)(savedSkinColorR / skinRangeR);
                    skinSliders[1].normalizedValue = (float)(savedSkinColorG / skinRangeG);
                    skinSliders[2].normalizedValue = (float)(savedSkinColorB / skinRangeB);

                    trouserSliders[0].normalizedValue = inputTrouserColor.r;
                    trouserSliders[1].normalizedValue = inputTrouserColor.g;
                    trouserSliders[2].normalizedValue = inputTrouserColor.b;
                }
            }
        }
    }


    // Required method by the ISaveable interface, which will store all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableStoreScene(string sceneName)
    {
        // Nothing to store here since the CharacterCustomization is on a persistent scene - it won't get reset ever because we always stay on that scene
    }


    // Required method by the ISaveable interface, which will restore all of the scene data, executed for every item in the iSaveableObjectList. This let's us walk between
    // scenes and keep the stored stuff active with ISaveableRestoreScene 
    public void ISaveableRestoreScene(string sceneName)
    {
        // Nothing to restore here since the CharacterCustomization is on a persistent scene - it won't get reset ever because we always stay on that scene
    }
}
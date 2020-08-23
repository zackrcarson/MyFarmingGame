using UnityEngine;

public class Item : MonoBehaviour
{
    
    [ItemCodeDescriptionAttribute]
    [SerializeField]
    private int _itemCode;

    private SpriteRenderer spriteRenderer;

    public int ItemCode { get { return _itemCode; } set { _itemCode = value;} }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodeParam)
    {   
        // default item code is 0, so as long as it's been set up.
        if (itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;

            // Use the singleton monobehavious inventory manager class' instance method GetItemDetails to return all of the item details for the given item code
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            
            // Make sure the sprite is set up correctly based on the item details sprite!
            spriteRenderer.sprite = itemDetails.itemSprite;

            // If the item type is reapable (item type enum!), then add a nudgeable component
            if (itemDetails.itemType == ItemType.Reapable_scenary)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}

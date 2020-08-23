using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    // Override Unity's class GetPropertyHeight so that it will return double the height, so we can draw two different items!!
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Change the returned property height to be double to cater for the additional item code description that we will draw
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    // Override Unity's class for GUI drawing to draw our own custom things
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty/EndProperty on the parent property means that prefab override logic works on the entire property
        EditorGUI.BeginProperty(position, label, property);

        // Only do this if the property is serialized, and an integer!
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            // Start of check for changed values 
            EditorGUI.BeginChangeCheck();

            // Draw the item code with the first half of the overall height
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            // Draw the item description with the second half of the overall height, using the new method GetItemDescription below
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));
            
            // If item code value has changed, then set value to new value
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }
        
        // End the property!
        EditorGUI.EndProperty();

    }

    public string GetItemDescription(int itemCode)
    {
        // Return a string of the item description given the corresponding item code
        SO_ItemList so_itemList;

        // Load the scriptable object containing item details as a list
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Object Assets/Item/so_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;

        // Find the item code in the dict
        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);

        // Return the item description corresponding to the given item code, if it exists. Else return empty string!
        if (itemDetail != null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return "";
        }
    }
}

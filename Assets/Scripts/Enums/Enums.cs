// Count simply returns the integer that is equal to the length of actual items in the enum, because it's the last item in the enum (wchich starts counting at 0)
public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum ToolEffect 
{
    none,
    watering
}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum ItemType
{
    Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count
}
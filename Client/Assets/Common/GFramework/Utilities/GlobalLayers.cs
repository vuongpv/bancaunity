
/// <summary>
/// Object layer definations
/// </summary>
public class GlobalLayers
{
	/// <summary>UI layer</summary>
	public const int UI = 8;
	public const int UIMask = 1 << UI;

    /// <summary>Player layers</summary>
    public const int Players = 9;
    public const int PlayersMask = 1 << Players;

    /// <summary>NPC layers</summary>
    public const int NPCs = 10;
    public const int NPCsMask = 1 << NPCs;
    
    /// <summary>Guns, bullets objects layers</summary>
    public const int GunObjects = 11;
    public const int GunObjectsMask = 1 << GunObjects;

    /// <summary>Gun UI</summary>
    public const int GunUIObjects = 12;
    public const int GunUIObjectsMask = 1 << GunUIObjects;
}
using HarmonyLib;
using UI;
using UI.Dialogs;

namespace NyahahahahaMap;

[HarmonyPatch(typeof(UserInterface), "Awake")]
public static class UserInterfaceStartPatch
{
    public static void Prefix(UserInterface __instance)
    {
        MapManager.Instance.CreateMiniMap();
    }
}

[HarmonyPatch(typeof(UserInterface), "Update")]
public static class UserInterfaceUpdatePatch
{
    public static void Prefix(UserInterface __instance)
    {
        MapManager.Instance.HandleMap();
    }
}

[HarmonyPatch(typeof(UserInterface), "ShowMenu")]
public static class UserInterfaceShowMenuPatch
{
    public static void Prefix(UserInterface __instance)
    {
        MapManager.Instance.HideMap(false, false);
    }
}

[HarmonyPatch(typeof(InventoryPanel), "Show")]
public static class InventoryPanelShowPatch
{
    public static void Prefix(InventoryPanel __instance)
    {
        MapManager.Instance.HideMap(false, true);
    }
}

[HarmonyPatch(typeof(MainCharPanel), "Show")]
public static class MainCharPanelShowPatch
{
    public static void Prefix(MainCharPanel __instance)
    {
        MapManager.Instance.HideMap(false, true);
    }
}
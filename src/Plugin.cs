using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Cjayride.BackpackTombstone
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("vapok.mods.adventurebackpacks", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "cjayride.BackpackTombstone";
        public const string PluginName = "Backpack Tombstone";
        public const string PluginVersion = "1.0.1";

        private void Awake()
        {
            new Harmony(PluginGuid).PatchAll();
            Logger.LogInfo("Adventure Backpacks will not dump their contents on the ground when you die. The tombstone keeps the backpack.");
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
    internal static class DeathScope
    {
        internal static int Depth;

        private static void Prefix(Player __instance)
        {
            if (__instance != null && __instance == Player.m_localPlayer)
                Depth++;
        }

        private static void Postfix(Player __instance)
        {
            if (__instance != null && __instance == Player.m_localPlayer && Depth > 0)
                Depth--;
        }
    }

    /// <summary>
    /// Removing a backpack outside a normal inventory move empties it onto the ground
    /// and still lets the remove continue. The backpack item also keeps a saved copy of
    /// those contents. Taking it back out of the tombstone therefore dumps the meat and
    /// then loads that saved copy, so the bag fills up with duplicates. Skipping the dump
    /// while dying, and while the backpack is sitting in a tombstone, leaves the one copy.
    /// </summary>
    [HarmonyPatch]
    internal static class SkipYardSaleOnDeath
    {
        private static bool Prepare()
        {
            return TargetMethod() != null;
        }

        private static MethodBase TargetMethod()
        {
            return AccessTools.Method("AdventureBackpacks.Assets.Backpacks:PerformYardSale");
        }

        private static bool Prefix(ItemDrop.ItemData itemData, int numberItems, ref bool __result)
        {
            if (numberItems != 0)
                return true;

            if (DeathScope.Depth <= 0 && !InTombstone(itemData))
                return true;

            __result = true;
            return false;
        }

        private static bool InTombstone(ItemDrop.ItemData item)
        {
            if (item == null)
                return false;

            Container[] containers = Object.FindObjectsByType<Container>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < containers.Length; i++)
            {
                Container container = containers[i];
                if (container == null || container.GetComponent<TombStone>() == null)
                    continue;

                Inventory inventory = container.GetInventory();
                if (inventory != null && inventory.ContainsItem(item))
                    return true;
            }

            return false;
        }
    }
}

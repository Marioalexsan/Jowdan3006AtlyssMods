using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static ChatBehaviour;

namespace DyeCommands;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private static ProfileDyeConfig[] profileDyeConfigs;
    private static string chatColourHex = "#f5ce42";

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        profileDyeConfigs = new ProfileDyeConfig[7];

        for (int i = 0; i < profileDyeConfigs.Count(); i++)
        {
            profileDyeConfigs[i] = new ProfileDyeConfig();
            profileDyeConfigs[i].HelmDyeEnabled = Config.Bind(
                $"CharacterProfile{i}",
                "HelmDyeEnabled",
                true,
                "Whether Dyes are Enabled for this profiles Helm."
            );
            profileDyeConfigs[i].ChestpieceDyeEnabled = Config.Bind(
                $"CharacterProfile{i}",
                "ChestpieceDyeEnabled",
                true,
                "Whether Dyes are Enabled for this profiles Chestpiece."
            );
            profileDyeConfigs[i].LeggingsDyeEnabled = Config.Bind(
                $"CharacterProfile{i}",
                "LeggingsDyeEnabled",
                true,
                "Whether Dyes are Enabled for this profiles Leggings."
            );
            profileDyeConfigs[i].CapeDyeEnabled = Config.Bind(
                $"CharacterProfile{i}",
                "CapeDyeEnabled",
                true,
                "Whether Dyes are Enabled for this profiles Cape."
            );
        }

        new Harmony(MyPluginInfo.PLUGIN_NAME).PatchAll();
        Logger.LogInfo("Plugin DyeCommands is loaded!");
    }

    //[HarmonyPatch(typeof(ChatBehaviour), "Send_ChatMessage")]
    //public static class Send_ChatMessagePatch
    //{

    //}

    //TODO: Find a better method to patch into as this is called every frame.
    [HarmonyPatch(typeof(RaceModelEquipDisplay), "Apply_ArmorDisplay")]
    public static class Apply_ArmorDisplayPatch
    {
        [HarmonyPrefix]
        public static void Apply_ArmorDisplay_Prefix(ScriptableArmor _scriptArmor)
        {
            if (_scriptArmor.GetType() == typeof(ScriptableHelm) && !profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableChestpiece) && !profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableLeggings) && !profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableCape) && !profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else
            {
                _scriptArmor._canDyeArmor = true;
            }
        }
    }

    [HarmonyPatch(typeof(ChatBehaviour), "Cmd_SendChatMessage")]
    public static class AddCommandsPatch
    {
        [HarmonyPrefix]
        public static bool Cmd_SendChatMessage_Prefix(ChatBehaviour __instance, string _message, ChatChannel _chatChannel)
        {
            var messageParts = _message.ToLower().Split(' ');
            if (messageParts.FirstOrDefault() == "/dye")
            {
                __instance.New_ChatMessage(_message);
                PlayerVisual component = __instance.GetComponent<PlayerVisual>();
                PlayerAppearanceStruct playerAppearanceStruct = component._playerAppearanceStruct;

                ScriptableArmorDye[] scriptableArmorDyes = GameManager._current._statLogics._armorDyes;
                List<string> scriptableArmorDyeNames = scriptableArmorDyes.Select(x => x._itemName.Split(' ').FirstOrDefault()).ToList();

                if (messageParts.ElementAtOrDefault(1) == "help")
                {
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Type \"/dye <Armour> <Dye>\"</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Armour: Helm, Chest, Legs, Cape, All</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Dye: Grey, Blue, Green, Red, None</color>");
                    var extraDyes = scriptableArmorDyeNames.Except(["Grey", "Blue", "Green", "Red"]);
                    if (extraDyes.Any()) __instance.New_ChatMessage($"<color={chatColourHex}>[DC] ... {extraDyes.Aggregate((x, y) => $"{x}, {y}")}</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] This is not case sensitive.</color>");
                    return false;
                }

                int dyeIndex = -1;
                bool validDye = true;
                for (var i = 0; i < scriptableArmorDyeNames.Count(); i++)
                {
                    if (messageParts.ElementAtOrDefault(2) == scriptableArmorDyeNames[i].ToLower() || messageParts.ElementAtOrDefault(2) == i.ToString())
                    {
                        dyeIndex = i;
                        break;
                    }
                }
                if (dyeIndex == -1 && messageParts.ElementAtOrDefault(2) != "none") validDye = false;
                
                int armourIndex = -1;
                switch (messageParts.ElementAtOrDefault(1))
                {
                    case "helm" or "h" or "0":
                        armourIndex = 0;
                        break;
                    case "chest" or "c" or "1":
                        armourIndex = 1;
                        break;
                    case "legs" or "leg" or "l" or "2":
                        armourIndex = 2;
                        break;
                    case "cape" or "k" or "3":
                        armourIndex = 3;
                        break;
                    case "All" or "a" or "4":
                        armourIndex = 4;
                        break;
                }
                if (validDye && armourIndex >= 0)
                {
                    bool dyeEnabled = dyeIndex != -1;
                    switch (armourIndex)
                    {
                        case 0:
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._helmDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._helmDyeIndex;
                            break;
                        case 1:
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._chestDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._chestDyeIndex;
                            break;
                        case 2:
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._legsDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._legsDyeIndex;
                            break;
                        case 3:
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._capeDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._capeDyeIndex;
                            break;
                        case 4:
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = dyeEnabled;
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = dyeEnabled;
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = dyeEnabled;
                            profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = dyeEnabled;

                            playerAppearanceStruct._helmDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._helmDyeIndex;
                            playerAppearanceStruct._chestDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._chestDyeIndex;
                            playerAppearanceStruct._legsDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._legsDyeIndex;
                            playerAppearanceStruct._capeDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._capeDyeIndex;
                            break;
                    }
                    component.Network_playerAppearanceStruct = playerAppearanceStruct;
                    if (!dyeEnabled) __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Cleared {(AmourPart)armourIndex} Dye.</color>");
                    else __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Dyed {(AmourPart)armourIndex} {scriptableArmorDyeNames[dyeIndex]}.</color>");
                }
                else
                {
                    if (armourIndex < 0 && messageParts.Count() > 1)
                    {
                        __instance.New_ChatMessage($"<color={chatColourHex}>[DC] \"{messageParts.ElementAtOrDefault(1)}\" is not a valid amour piece.</color>");
                    }
                    if (!validDye && messageParts.Count() > 2)
                    {
                        __instance.New_ChatMessage($"<color={chatColourHex}>[DC] \"{messageParts.ElementAtOrDefault(2)}\" is not a valid dye colour.</color>");
                    }
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Type \"/dye help\" for details.</color>");
                }
                return false;
            }
            return true;
        }
    }
}

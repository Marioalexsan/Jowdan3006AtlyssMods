using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using static ChatBehaviour;

namespace DyeCommands;

[BepInPlugin("com.16mb.dyecommands", "DyeCommands", "0.0.1")]
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

        new Harmony("DyeCommands").PatchAll();
        Logger.LogInfo("Plugin DyeCommands is loaded!");
    }

    //[HarmonyPatch(typeof(ChatBehaviour), "Send_ChatMessage")]
    //public static class Send_ChatMessagePatch
    //{

    //}

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
    public static class AddCommandssPatch
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

                if (new[] { "help", "h" }.Contains(messageParts.ElementAtOrDefault(1)))
                {
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Type \"/dye <Armour> <Dye>\"</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Armour: Helm, Chest, Legs, Cape, All</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Dye: Grey, Blue, Green, Red, None</color>");
                    __instance.New_ChatMessage($"<color={chatColourHex}>[DC] This is not case sensitive.</color>");
                    return false;
                }

                var dyeIndex = -1;
                switch (messageParts.ElementAtOrDefault(2))
                {
                    case "grey" or "white" or "w" or "0":
                        dyeIndex = 0;
                        break;
                    case "blue" or "b" or "1":
                        dyeIndex = 1;
                        break;
                    case "green" or "g" or "2":
                        dyeIndex = 2;
                        break;
                    case "red" or "r" or "3":
                        dyeIndex = 3;
                        break;
                    case "none" or "n" or "4":
                        dyeIndex = 4;
                        break;
                    default:
                        break;
                }
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
                    default:
                        break;
                }
                if (dyeIndex >= 0 && armourIndex >= 0)
                {
                    if (dyeIndex == 4)
                    {
                        switch (armourIndex)
                        {
                            case 0:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = false;
                                break;
                            case 1:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = false;
                                break;
                            case 2:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = false;
                                break;
                            case 3:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = false;
                                break;
                            case 4:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = false;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = false;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = false;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = false;
                                break;
                            default:
                                break;
                        }
                        __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Cleared {(AmourPart)armourIndex} Dye.</color>");
                    }
                    else
                    {
                        switch (armourIndex)
                        {
                            case 0:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = true;
                                playerAppearanceStruct._helmDyeIndex = dyeIndex;
                                break;
                            case 1:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = true;
                                playerAppearanceStruct._chestDyeIndex = dyeIndex;
                                break;
                            case 2:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = true;
                                playerAppearanceStruct._legsDyeIndex = dyeIndex;
                                break;
                            case 3:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = true;
                                playerAppearanceStruct._capeDyeIndex = dyeIndex;
                                break;
                            case 4:
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].HelmDyeEnabled.Value = true;
                                playerAppearanceStruct._helmDyeIndex = dyeIndex;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].ChestpieceDyeEnabled.Value = true;
                                playerAppearanceStruct._chestDyeIndex = dyeIndex;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].LeggingsDyeEnabled.Value = true;
                                playerAppearanceStruct._legsDyeIndex = dyeIndex;
                                profileDyeConfigs[ProfileDataManager._current.SelectedFileIndex].CapeDyeEnabled.Value = true;
                                playerAppearanceStruct._capeDyeIndex = dyeIndex;
                                break;
                            default:
                                break;
                        }
                        component.Network_playerAppearanceStruct = playerAppearanceStruct;
                        __instance.New_ChatMessage($"<color={chatColourHex}>[DC] Dyed {(AmourPart)armourIndex} {(DyeColour)dyeIndex}.</color>");
                    }
                }
                else
                {
                    if (armourIndex < 0 && messageParts.Count() > 1)
                    {
                        __instance.New_ChatMessage($"<color={chatColourHex}>[DC] \"{messageParts.ElementAtOrDefault(1)}\" is not a valid amour piece.</color>");
                    }
                    if (dyeIndex < 0 && messageParts.Count() > 2)
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

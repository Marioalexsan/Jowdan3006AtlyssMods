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
    internal static Plugin Instance;

    internal static new ManualLogSource Logger;

    private static Dictionary<int, ProfileDyeConfig> profileDyeConfigs = [];
    private static string chatColourHex = "#f5ce42";

    private void Awake()
    {
        // Plugin startup logic
        Instance = this;
        Logger = base.Logger;

        new Harmony(MyPluginInfo.PLUGIN_NAME).PatchAll();
        Logger.LogInfo("Plugin DyeCommands is loaded!");
    }

    private static ProfileDyeConfig GetProfile(int index)
    {
        if (profileDyeConfigs.TryGetValue(index, out var existingProfile))
            return existingProfile;

        var config = profileDyeConfigs[index] = new ProfileDyeConfig();

        config.HelmDyeEnabled = Instance.Config.Bind(
            $"CharacterProfile{index}",
            "HelmDyeEnabled",
            true,
            "Whether Dyes are Enabled for this profiles Helm."
        );
        config.ChestpieceDyeEnabled = Instance.Config.Bind(
            $"CharacterProfile{index}",
            "ChestpieceDyeEnabled",
            true,
            "Whether Dyes are Enabled for this profiles Chestpiece."
        );
        config.LeggingsDyeEnabled = Instance.Config.Bind(
            $"CharacterProfile{index}",
            "LeggingsDyeEnabled",
            true,
            "Whether Dyes are Enabled for this profiles Leggings."
        );
        config.CapeDyeEnabled = Instance.Config.Bind(
            $"CharacterProfile{index}",
            "CapeDyeEnabled",
            true,
            "Whether Dyes are Enabled for this profiles Cape."
        );

        return config;
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
            var profile = GetProfile(ProfileDataManager._current.SelectedFileIndex);

            if (_scriptArmor.GetType() == typeof(ScriptableHelm) && !profile.HelmDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableChestpiece) && !profile.ChestpieceDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableLeggings) && !profile.LeggingsDyeEnabled.Value)
            {
                _scriptArmor._canDyeArmor = false;
            }
            else if (_scriptArmor.GetType() == typeof(ScriptableCape) && !profile.CapeDyeEnabled.Value)
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
                    case "all" or "a" or "4":
                        armourIndex = 4;
                        break;
                }
                if (validDye && armourIndex >= 0)
                {
                    var profile = GetProfile(ProfileDataManager._current.SelectedFileIndex);

                    bool dyeEnabled = dyeIndex != -1;
                    switch (armourIndex)
                    {
                        case 0:
                            profile.HelmDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._helmDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._helmDyeIndex;
                            break;
                        case 1:
                            profile.ChestpieceDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._chestDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._chestDyeIndex;
                            break;
                        case 2:
                            profile.LeggingsDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._legsDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._legsDyeIndex;
                            break;
                        case 3:
                            profile.CapeDyeEnabled.Value = dyeEnabled;
                            playerAppearanceStruct._capeDyeIndex = dyeEnabled ? dyeIndex : playerAppearanceStruct._capeDyeIndex;
                            break;
                        case 4:
                            profile.HelmDyeEnabled.Value = dyeEnabled;
                            profile.ChestpieceDyeEnabled.Value = dyeEnabled;
                            profile.LeggingsDyeEnabled.Value = dyeEnabled;
                            profile.CapeDyeEnabled.Value = dyeEnabled;

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

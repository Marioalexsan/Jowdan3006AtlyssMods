using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MoreBankTabs
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            new Harmony(MyPluginInfo.PLUGIN_NAME).PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static UnityEngine.UI.Button _storageTabButton_03, _storageTabButton_04, _storageTabButton_05;
        public static List<ItemData> _itemDatas_03 = new List<ItemData>();
        public static List<ItemData> _itemDatas_04 = new List<ItemData>();
        public static List<ItemData> _itemDatas_05 = new List<ItemData>();
        public static int[] _storageSizes_03 = new int[3];
        public static int[] _storageSizes_04 = new int[3];
        public static int[] _storageSizes_05 = new int[3];

        [HarmonyPatch(typeof(ItemStorageManager), "Awake")]
        public static class AwakePatch
        {
            private static ItemStorageManager itemStorageManager;

            [HarmonyPostfix]
            private static void AwakePostfix(ItemStorageManager __instance)
            {
                itemStorageManager = __instance;

                GameObject storageTabButton_00 = GameObject.Find("_GameUI_InGame/Canvas_DialogSystem/_dolly_storageBox/_button_storageTab_00");
                GameObject storageTabButton_01 = GameObject.Find("_GameUI_InGame/Canvas_DialogSystem/_dolly_storageBox/_button_storageTab_01");
                GameObject storageTabButton_02 = GameObject.Find("_GameUI_InGame/Canvas_DialogSystem/_dolly_storageBox/_button_storageTab_02");

                GameObject storageTabButton_03 = Object.Instantiate(storageTabButton_00, storageTabButton_00.transform, true);
                storageTabButton_03.transform.localPosition = new Vector3(132f, 0f, 0f);
                _storageTabButton_03 = storageTabButton_03.GetComponent<UnityEngine.UI.Button>();
                storageTabButton_03.GetComponentInChildren<Image>().color += new Color(0.35f, 0f, 0.1f);

                _storageTabButton_03.onClick.AddListener(SetStorageTab_03);
                _storageTabButton_03.onClick.AddListener(__instance.Clear_StorageEntries);
                _storageTabButton_03.onClick.AddListener(__instance.Init_StorageListing);

                GameObject storageTabButton_04 = Object.Instantiate(storageTabButton_01, storageTabButton_01.transform, true);
                storageTabButton_04.transform.localPosition = new Vector3(132f, 0f, 0f);
                _storageTabButton_04 = storageTabButton_04.GetComponent<UnityEngine.UI.Button>();
                storageTabButton_04.GetComponentInChildren<Image>().color += new Color(0.35f, 0f, 0.1f);

                _storageTabButton_04.onClick.AddListener(SetStorageTab_04);
                _storageTabButton_04.onClick.AddListener(__instance.Clear_StorageEntries);
                _storageTabButton_04.onClick.AddListener(__instance.Init_StorageListing);

                GameObject storageTabButton_05 = Object.Instantiate(storageTabButton_02, storageTabButton_02.transform, true);
                storageTabButton_05.transform.localPosition = new Vector3(132f, 0f, 0f);
                _storageTabButton_05 = storageTabButton_05.GetComponent<UnityEngine.UI.Button>();
                storageTabButton_05.GetComponentInChildren<Image>().color += new Color(0.35f, 0f, 0.1f);

                _storageTabButton_05.onClick.AddListener(SetStorageTab_05);
                _storageTabButton_05.onClick.AddListener(__instance.Clear_StorageEntries);
                _storageTabButton_05.onClick.AddListener(__instance.Init_StorageListing);
            }

            private static void SetStorageTab_03()
            {
                itemStorageManager._selectedStorageTab = 3;
                itemStorageManager.GetPrivateField<RectTransform>("_storageTabHighlight").transform.position = _storageTabButton_03.transform.GetChild(0).position;
            }

            private static void SetStorageTab_04()
            {
                itemStorageManager._selectedStorageTab = 4;
                itemStorageManager.GetPrivateField<RectTransform>("_storageTabHighlight").transform.position = _storageTabButton_04.transform.GetChild(0).position;
            }

            private static void SetStorageTab_05()
            {
                itemStorageManager._selectedStorageTab = 5;
                itemStorageManager.GetPrivateField<RectTransform>("_storageTabHighlight").transform.position = _storageTabButton_05.transform.GetChild(0).position;
            }
        }

        [HarmonyPatch(typeof(ItemStorageManager), "Begin_StorageListing")]
        public static class Begin_StorageListingPatch
        {
            [HarmonyPrefix]
            private static bool Begin_StorageListingPrefix(ItemStorageManager __instance)
            {
                if (__instance._selectedStorageTab > 2)
                {
                    ItemData[] array = null;
                    switch (__instance._selectedStorageTab)
                    {
                        case 3:
                            array = _itemDatas_03.ToArray();
                            break;
                        case 4:
                            array = _itemDatas_04.ToArray();
                            break;
                        case 5:
                            array = _itemDatas_05.ToArray();
                            break;
                    }
                    for (int i = 0; i < array.Length; i++)
                    {
                        ItemData itemData = array[i];
                        ScriptableItem scriptableItem = GameManager._current.LocateItem(itemData._itemName);
                        if (itemData._quantity <= 0 || !scriptableItem)
                        {
                            break;
                        }
                        __instance.Create_StorageEntry(itemData, scriptableItem, i, itemData._slotNumber);
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ItemStorageManager), "Create_StorageEntry")]
        public static class Create_StorageEntryPatch
        {
            [HarmonyPrefix]
            private static void Create_StorageEntryPrefix(ItemStorageManager __instance, ItemData _itemData, ScriptableItem _scriptItem, int _index, int _slotNumber)
            {
                switch (__instance._selectedStorageTab)
                {
                    case 3:
                        if (_storageSizes_03[(uint)_scriptItem._itemType] >= 48)
                        {
                            return;
                        }
                        _storageSizes_03[(uint)_scriptItem._itemType]++;
                        break;
                    case 4:
                        if (_storageSizes_04[(uint)_scriptItem._itemType] >= 48)
                        {
                            return;
                        }
                        _storageSizes_04[(uint)_scriptItem._itemType]++;
                        break;
                    case 5:
                        if (_storageSizes_05[(uint)_scriptItem._itemType] >= 48)
                        {
                            return;
                        }
                        _storageSizes_05[(uint)_scriptItem._itemType]++;
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(ItemStorageManager), "Delete_StorageEntry")]
        public static class Delete_StorageEntryPatch
        {
            [HarmonyPrefix]
            public static bool Delete_StorageEntryPrefix(ItemStorageManager __instance, ItemData _itemData)
            {
                if (__instance._selectedStorageTab > 2)
                {
                    ScriptableItem scriptableItem = GameManager._current.LocateItem(_itemData._itemName);
                    if (!scriptableItem)
                    {
                        return true;
                    }
                    switch (__instance._selectedStorageTab)
                    {
                        case 3:
                            _itemDatas_03.Remove(_itemData);
                            _storageSizes_03[(uint)scriptableItem._itemType]--;
                            break;
                        case 4:
                            _itemDatas_04.Remove(_itemData);
                            _storageSizes_04[(uint)scriptableItem._itemType]--;
                            break;
                        case 5:
                            _itemDatas_05.Remove(_itemData);
                            _storageSizes_05[(uint)scriptableItem._itemType]--;
                            break;
                    }
                    for (int i = 0; i < __instance._storageListEntries.Count; i++)
                    {
                        if (__instance._storageListEntries[i]._itemData == _itemData)
                        {
                            UnityEngine.Object.Destroy(__instance._storageListEntries[i].gameObject);
                            __instance._storageListEntries.RemoveAt(i);
                            break;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ItemStorageManager), "Update")]
        public static class UpdatePatch
        {
            [HarmonyPrefix]
            public static bool UpdatePrefix(ItemStorageManager __instance)
            {
                _storageTabButton_03.interactable = __instance._selectedStorageTab != 3;
                _storageTabButton_04.interactable = __instance._selectedStorageTab != 4;
                _storageTabButton_05.interactable = __instance._selectedStorageTab != 5;
                if (__instance._selectedStorageTab > 2)
                {
                    __instance._isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.STORAGE;
                    MenuElement storageWindowElement = __instance.GetPrivateField<MenuElement>("_storageWindowElement");
                    storageWindowElement.gameObject.SetActive(__instance._isOpen);
                    storageWindowElement.isEnabled = __instance._isOpen;
                    if (__instance._isOpen)
                    {
                        switch (__instance._selectedStorageTab)
                        {
                            case 3:
                                __instance.GetPrivateField<Text>("_counter_gearItemSize").text = $"{_storageSizes_03[0]}/48";
                                __instance.GetPrivateField<Text>("_counter_consumableItemSize").text = $"{_storageSizes_03[1]}/48";
                                __instance.GetPrivateField<Text>("_counter_tradeItemSize").text = $"{_storageSizes_03[2]}/48";
                                break;
                            case 4:
                                __instance.GetPrivateField<Text>("_counter_gearItemSize").text = $"{_storageSizes_04[0]}/48";
                                __instance.GetPrivateField<Text>("_counter_consumableItemSize").text = $"{_storageSizes_04[1]}/48";
                                __instance.GetPrivateField<Text>("_counter_tradeItemSize").text = $"{_storageSizes_04[2]}/48";
                                break;
                            case 5:
                                __instance.GetPrivateField<Text>("_counter_gearItemSize").text = $"{_storageSizes_05[0]}/48";
                                __instance.GetPrivateField<Text>("_counter_consumableItemSize").text = $"{_storageSizes_05[1]}/48";
                                __instance.GetPrivateField<Text>("_counter_tradeItemSize").text = $"{_storageSizes_05[2]}/48";
                                break;
                        }
                        __instance.GetPrivateField<UnityEngine.UI.Button>("_storageTabButton_00").interactable = true;
                        __instance.GetPrivateField<UnityEngine.UI.Button>("_storageTabButton_01").interactable = true;
                        __instance.GetPrivateField<UnityEngine.UI.Button>("_storageTabButton_02").interactable = true;
                        __instance.InvokePrivateMethod("Handle_TabVisibility");
                        return false;
                    }
                }
                return true;
            }
        }

        public static ItemStorage_Profile _itemStorageProfile_03, _itemStorageProfile_04, _itemStorageProfile_05;

        [HarmonyPatch(typeof(ProfileDataManager), "Load_ItemStorageData")]
        public static class Load_ItemStorageDataPatch
        {
            [HarmonyPostfix]
            private static void Load_ItemStorageDataPostfix(ProfileDataManager __instance)
            {
                string dataPath = __instance.GetPrivateField<string>("_dataPath");
                _itemStorageProfile_03 = new ItemStorage_Profile();
                _itemStorageProfile_03._heldItemStorage = [];
                _itemStorageProfile_04 = new ItemStorage_Profile();
                _itemStorageProfile_04._heldItemStorage = [];
                _itemStorageProfile_05 = new ItemStorage_Profile();
                _itemStorageProfile_05._heldItemStorage = [];

                if (File.Exists(dataPath + "/atl_itemBank_03"))
                {
                    ItemStorage_Profile itemStorageProfile_03 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(dataPath + "/atl_itemBank_03"));
                    _itemStorageProfile_03 = itemStorageProfile_03;
                }
                if (File.Exists(dataPath + "/atl_itemBank_04"))
                {
                    ItemStorage_Profile itemStorageProfile_04 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(dataPath + "/atl_itemBank_04"));
                    _itemStorageProfile_04 = itemStorageProfile_04;
                }
                if (File.Exists(dataPath + "/atl_itemBank_05"))
                {
                    ItemStorage_Profile itemStorageProfile_05 = JsonUtility.FromJson<ItemStorage_Profile>(File.ReadAllText(dataPath + "/atl_itemBank_05"));
                    _itemStorageProfile_05 = itemStorageProfile_05;
                }

                ItemStorageManager current = ItemStorageManager._current;
                if ((bool)current)
                {
                    _storageSizes_03[0] = 0;
                    _storageSizes_03[1] = 0;
                    _storageSizes_03[2] = 0;
                    _storageSizes_04[0] = 0;
                    _storageSizes_04[1] = 0;
                    _storageSizes_04[2] = 0;
                    _storageSizes_05[0] = 0;
                    _storageSizes_05[1] = 0;
                    _storageSizes_05[2] = 0;
                    _itemDatas_03.Clear();
                    _itemDatas_04.Clear();
                    _itemDatas_05.Clear();
                    _itemDatas_03.AddRange(_itemStorageProfile_03._heldItemStorage);
                    _itemDatas_04.AddRange(_itemStorageProfile_04._heldItemStorage);
                    _itemDatas_05.AddRange(_itemStorageProfile_05._heldItemStorage);
                }
            }
        }

        [HarmonyPatch(typeof(ProfileDataManager), "Save_ItemStorageData")]
        public static class Save_ItemStorageDataPatch
        {

            [HarmonyPostfix]
            private static void Save_ItemStorageDataPostfix(ProfileDataManager __instance)
            {
                string dataPath = __instance.GetPrivateField<string>("_dataPath");
                _itemStorageProfile_03._heldItemStorage = _itemDatas_03.ToArray();
                _itemStorageProfile_04._heldItemStorage = _itemDatas_04.ToArray();
                _itemStorageProfile_05._heldItemStorage = _itemDatas_05.ToArray();
                string contents_03 = JsonUtility.ToJson(_itemStorageProfile_03, prettyPrint: true);
                File.WriteAllText(dataPath + "/atl_itemBank_03", contents_03);
                string contents_04 = JsonUtility.ToJson(_itemStorageProfile_04, prettyPrint: true);
                File.WriteAllText(dataPath + "/atl_itemBank_04", contents_04);
                string contents_05 = JsonUtility.ToJson(_itemStorageProfile_05, prettyPrint: true);
                File.WriteAllText(dataPath + "/atl_itemBank_05", contents_05);
            }
        }

        [HarmonyPatch(typeof(ItemListDataEntry), "Init_PutItemIntoStorage")]
        public static class Init_PutItemIntoStoragePatch
        {
            [HarmonyPrefix]
            private static bool Init_PutItemIntoStoragePostfix(ItemListDataEntry __instance, int _setItemSlot)
            {
                if (ItemStorageManager._current._selectedStorageTab >= 2)
                {
                    if (__instance._entryType != 0)
                    {
                        return false;
                    }
                    bool flag = false;
                    switch (ItemStorageManager._current._selectedStorageTab)
                    {
                        case 3:
                            flag = _storageSizes_03[(uint)__instance._scriptableItem._itemType] >= 48;
                            break;
                        case 4:
                            flag = _storageSizes_04[(uint)__instance._scriptableItem._itemType] >= 48;
                            break;
                        case 5:
                            flag = _storageSizes_05[(uint)__instance._scriptableItem._itemType] >= 48;
                            break;
                    }
                    if (flag)
                    {
                        ErrorPromptTextManager.current.Init_ErrorPrompt("Storage Full");
                        __instance.Relocate_ToOriginSlot();
                        return false;
                    }
                    List<ItemListDataEntry> storageListEntries = ItemStorageManager._current._storageListEntries;
                    for (int i = 0; i < storageListEntries.Count; i++)
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            break;
                        }
                        if (storageListEntries[i]._scriptableItem == __instance._scriptableItem && storageListEntries[i]._itemData._quantity < storageListEntries[i]._itemData._maxQuantity
                            && __instance._itemData._quantity + storageListEntries[i]._itemData._quantity <= storageListEntries[i]._itemData._maxQuantity)
                        {
                            storageListEntries[i]._itemData._quantity += __instance._itemData._quantity;
                            Player._mainPlayer._pInventory.Remove_Item(__instance._itemData, 0);
                            __instance.InvokePrivateMethod("Init_SaveProfiles");
                            ProfileDataManager._current.Save_ItemStorageData();
                            return false;
                        }
                    }
                    ItemStorageManager._current._commandBuffer = 0.25f;
                    ItemStorageManager._current.Create_StorageEntry(__instance._itemData, __instance._scriptableItem, ItemStorageManager._current._storageListEntries.Count, _setItemSlot);
                    switch (ItemStorageManager._current._selectedStorageTab)
                    {
                        case 3:
                            _itemDatas_03.Add(__instance._itemData);
                            break;
                        case 4:
                            _itemDatas_04.Add(__instance._itemData);
                            break;
                        case 5:
                            _itemDatas_05.Add(__instance._itemData);
                            break;
                    }
                    Player._mainPlayer._pInventory.Remove_Item(__instance._itemData, 0);
                    __instance.InvokePrivateMethod("Init_SaveProfiles");
                    ProfileDataManager._current.Save_ItemStorageData();
                    return false;
                }
                return true;
            }
        }
    }
}

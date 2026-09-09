using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    [SerializeField] private InvenrotyUI _inventory;

    private enum ItemType
    {
        None,
        Key,            // pool
        Powerpotion,    // pool
        PlayerUpgrade,  // int
        PetUpgrade      // int

        // 강화 성공 증강제 추가 할 수 있음 나중에 시도 
    }


    [Header("키 스폰")]
    [SerializeField] GameObject _goKey = null;
    [SerializeField] private int _keyCount = 5;

    [Header("강화 포션 스폰")]
    [SerializeField] GameObject _goPotion = null;
    [SerializeField] private int _potionCount = 5;
    [SerializeField] private float _potionTimer = 15f;

    private readonly Queue<GameObject> _keyQueue = new Queue<GameObject>();
    private readonly List<GameObject> _keyList = new List<GameObject>();

    private readonly Queue<GameObject> _potionQueue = new Queue<GameObject>();
    private readonly List<GameObject> _potionList = new List<GameObject>();
    private readonly Dictionary<GameObject, float> _potionDiction = new Dictionary<GameObject, float>();

    private Transform _key;
    private Transform _powerPotion;

    public List<GameObject> PotionList => _potionList;
    public Dictionary<GameObject, float> PotionDiction => _potionDiction;
    public List<GameObject> KeyList => _keyList;

    [System.Serializable]
    private class ItemWeightRandom
    {
        public ItemType ItemType;
        [Min(0)]
        public int Weight;

        public ItemWeightRandom(ItemType type, int weight)
        {
            ItemType = type;
            Weight = weight;
        }
    }

    [Header("확률 Weight 설정")]
    [SerializeField, Min(0)] private int _noneWeight = 30;
    [SerializeField, Min(0)] private int _keyWeight = 5;
    [SerializeField, Min(0)] private int _potionWeight = 1;
    [SerializeField, Min(0)] private int _couponWeight = 5;
    [SerializeField, Min(0)] private int _petWeight = 2;

    private ItemWeightRandom[] _itemWeightRandoms;

    private GameData _gameData;
    private ObjectData _objectData;
    private PlayerData _playerData;

    private void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(CreateItem), nameof(Start), nameof(_gameData));
        }


        _objectData = _gameData._ObjectData;
        _playerData = _gameData._PlayerData;
        if (_objectData == null || _playerData == null)
        {
            Log.LogNull(nameof(CreateItem), nameof(Start));
        }

        if(_inventory == null)
        {
            Log.LogNull(nameof(CreateItem), nameof(Start));
        }
    }

    public void ItemSetting()
    {
        ItemWeightSetting();
        KeySetting();
        PotionSetting();
        Debug.Log("TestLog : 아이템 세팅 완료");
    }

    private void Update()
    {
        PotionToPool();
    }

    // 가중치 세팅 || QA 때 다듬기 
    private void ItemWeightSetting()
    {
        _itemWeightRandoms = new ItemWeightRandom[]
        {
        new ItemWeightRandom(ItemType.None, _noneWeight - _objectData.ChestLevel),
        new ItemWeightRandom(ItemType.Key, _keyWeight),
        new ItemWeightRandom(ItemType.Powerpotion, _potionWeight),
        new ItemWeightRandom(ItemType.PlayerUpgrade, _couponWeight),
        new ItemWeightRandom(ItemType.PetUpgrade, _petWeight)
        };

    }


    // =========================================================
    // 키, 포션 풀 세팅 
    // =========================================================

    private void KeySetting()
    {
        if (_key != null)
        {
            return;
        }

        GameObject root = new GameObject("Key");
        _key = root.transform;


        for (int i = 0; i < _keyCount - 1; i++)
        {
            GameObject key = Instantiate(_goKey, _key);
            key.SetActive(false);

            _keyQueue.Enqueue(key);
        }
    }

    private void PotionSetting()
    {
        if (_powerPotion != null)
        {
            return;
        }

        GameObject root = new GameObject("potion");
        _powerPotion = root.transform;


        for (int i = 0; i < _potionCount; i++)
        {
            GameObject potion = Instantiate(_goPotion, _powerPotion);
            potion.SetActive(false);

            _potionQueue.Enqueue(potion);
        }
    }

    // 포션 드랍
    private void DropPotion(Vector3 dropPosition)
    {
        if (_potionQueue.Count == 0)
        {
            MorePotion();
        }
        if(_potionQueue.Count == 0)
        {
            Debug.Log($"{nameof(CreateItem)} : {nameof(DropPotion)}" + " 포션 생성 실패");
            return;
        }

        GameObject potion = _potionQueue.Dequeue();

        potion.transform.position = dropPosition;
        potion.SetActive(true);

        _potionList.Add(potion);
        _potionDiction.Add(potion, _potionTimer);
        _inventory.InventoryAdd(potion);
    }

    // 풀을 더 생성
    private void MorePotion()
    {
        if (_goPotion == null)
        {
            Debug.Log($"{nameof(CreateItem)} : {nameof(MorePotion)}" + " 포션 프리팹 확인");
            return;
        }

        for (int i = 0; i < _potionCount; i++)
        {
            GameObject potion = Instantiate(_goPotion, _powerPotion);

            potion.SetActive(false);

            _potionQueue.Enqueue(potion);
        }

    }


    // 사용시 풀로 반환
    public void UsePowerPotion()
    {
        if (_potionList.Count == 0)
        {
            Debug.Log("사용할 포션이 없습니다.");
            return;
        }

        GameObject potion = null;

        float time = float.MaxValue;

        foreach (KeyValuePair<GameObject, float> potionData in _potionDiction)
        {
            GameObject go = potionData.Key;
            float potiontime = potionData.Value;

            if (potiontime < time)
            {
                time = potiontime;
                potion = go;
            }
        }

        if(potion == null)
        {
            Log.LogNull(nameof(CreateItem), nameof(UsePowerPotion));
        }

        ReturnPotionToPool(potion);
    }

    // 제한 시간이 끝난 포션을 풀로 반환
    private void PotionToPool()
    {
        for (int i = _potionList.Count - 1; i >= 0; i--)
        {
            GameObject potion = _potionList[i];

            if (!_potionDiction.ContainsKey(potion))
            {
                Debug.LogWarning($"{potion.name}의 시간 정보가 없습니다.");

                _potionList.RemoveAt(i);
                continue;
            }

            _potionDiction[potion] -= Time.deltaTime;

            if (_potionDiction[potion] > 0f)
            {
                continue;
            }

            ReturnPotionToPool(potion);
        }
    }

    // 풀로 돌리기 
    public void ReturnPotionToPool(GameObject potion)
    {
        _potionList.Remove(potion);
        _potionDiction.Remove(potion);

        potion.SetActive(false);
        potion.transform.SetParent(_powerPotion);

        _potionQueue.Enqueue(potion);
        _inventory.InventoryRemove(potion);
    }


    // =========================================================
    //  아이템 드랍
    // =========================================================
    public void ItemDrop(Vector3 dropPosition)
    {
        ItemType selectedItem = RandomItem();

        Debug.Log($"결과: {selectedItem}");

        switch (selectedItem)
        {
            case ItemType.None:
                break;

            case ItemType.Key:
                DropKey(dropPosition);
                break;

            case ItemType.Powerpotion:
                DropPotion(dropPosition);
                break;

            case ItemType.PlayerUpgrade:
                DropPlayerUpgrade();
                Debug.Log($"UpgradeCoupon 개수 : {_objectData.PlayerUpgrade}");
                break;

            case ItemType.PetUpgrade:
                DropPetUpgrade();
                Debug.Log($"PetUpgradeCoupon 개수 : {_objectData.PetUpgrade}");
                break;

            default:
                break;
        }
    }

    private ItemType RandomItem()
    {
        // 가중치 랜덤 방식 드랍
        int totalWeight = 0;

        foreach (ItemWeightRandom itemData in _itemWeightRandoms)
        {
            if (itemData.Weight > 0)
            {
                totalWeight += itemData.Weight;
            }
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("설정된 가중치가 없습니다.");
            return ItemType.None;
        }

        int randomValue = Random.Range(0, totalWeight);
        int Weight = 0;

        foreach (ItemWeightRandom itemData in _itemWeightRandoms)
        {
            if (itemData.Weight <= 0)
            {
                continue;
            }

            Weight += itemData.Weight;

            if (randomValue < Weight)
            {
                return itemData.ItemType;
            }
        }

        // 혹시 모르니 
        return ItemType.None;
    }


    // =========================================================
    // 키 드랍
    // =========================================================

    private void DropKey(Vector3 dropPosition)
    {
        if (_keyQueue.Count == 0)
        {
            UpgradeEnermy();
            return;
        }

        GameObject key = _keyQueue.Dequeue();

        key.transform.position = dropPosition;
        key.SetActive(true);

        _keyList.Add(key);
        _inventory.InventoryAdd(key);
    }



    public void UpgradeEnermy()
    {
        ReturnAllKeys();
        _objectData.AddChestLevel(1);

        ItemWeightSetting();

        Debug.Log($"상자 레벨 : {_objectData.ChestLevel}");
    }


    private void ReturnAllKeys()
    {
        foreach (GameObject key in _keyList)
        {
            key.SetActive(false);
            key.transform.SetParent(_key);

            _keyQueue.Enqueue(key);
            _inventory.InventoryRemove(key);
        }

        _keyList.Clear();
    }


    // =========================================================
    // 업그레이드 쿠폰 드랍 
    // =========================================================

    private void DropPlayerUpgrade()
    {
        // 기본 드랍 + 상자 레벨에 따른 추가 드랍. 고정 수치 인스펙터로 나중에 빼기 
        _objectData.AddPlayerUpgrade(1 + _objectData.ChestLevel * 3);
    }


    // =========================================================
    // 업그레이드 쿠폰 드랍 
    // =========================================================

    private void DropPetUpgrade()
    {
        // 기본 드랍 + 상자 레벨에 따른 추가 드랍. 고정 수치 인스펙터로 나중에 빼기 
        _objectData.AddPetUpgrade(1 + _objectData.ChestLevel * 3);
    }



}





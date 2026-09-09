using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    // 아이템 1번 사용처리 
    // playerBattle Power flag on off로 조절 
    // 필요한거 사용 시간으로 키고 끄기.
    // 아이템 사용시 풀 돌아가기 + 여러개 사용시 지속시간 연장 
    // 지속 시간 UI에 띄우기 
    // 여기에 열쇠도 들고와서 인벤토리 기능 만들기 흠... 근데 UI어케 만듬? 


    [SerializeField] private CreateItem _createItem;
    [SerializeField] private PlayerBattle _playerBattle;
    [SerializeField] private InvenrotyUI _invenrotyUI;

    private float _PowerTimer = 0f;
    private bool _flagPotionTimer = false;

    private List<GameObject> _potionList;
    private Dictionary<GameObject, float> _potionDiction;

    public float PowerTimer => _PowerTimer;
    public bool FlagPotionTimer => _flagPotionTimer;

    private void Awake()
    {
        if( _createItem == null )
        {
            _createItem = GetComponent<CreateItem>();
        }
        if (_createItem == null)
        {
            Log.LogNull(nameof(UseItem), nameof(Awake), nameof(_createItem));
        }
        if(_playerBattle == null)
        {
            Log.LogNull(nameof(UseItem), nameof(Awake), nameof(_createItem));
        }
    }

    void Start()
    {
        _createItem.ItemSetting();
        _invenrotyUI.InventorySetting();
        _potionList = _createItem.PotionList;
        _potionDiction = _createItem.PotionDiction;
    }

    void Update()
    {
        PotionTime();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PotionUse();
        }
    }

    public void PotionUse()
    {
        if (_potionList.Count == 0)
        {
            Debug.Log("TestLog : 포션 없음");
            return;
        }

        GameObject potion = null;
        float minTime = float.MaxValue;

        foreach (GameObject item in _potionList)
        {
            if(item == null)
            {
                continue;
            }

            if (_potionDiction.TryGetValue(item, out float time))
            {
                if (time < minTime)
                {
                    minTime = time;
                    potion = item;
                }
            }
        }

        if (potion == null)
        {
            Debug.Log("TestLog : 예외 발생");
            return;
        }

        _createItem.ReturnPotionToPool(potion);

        _playerBattle.FlagPower = true;
        _flagPotionTimer = true;
        _PowerTimer += 10f;
    }

    private void PotionTime()
    {
        if (_flagPotionTimer)
        {
            _PowerTimer -= Time.deltaTime;
        }

        if(_PowerTimer < 0f)
        {
            _PowerTimer = 0f;
            _playerBattle.FlagPower = false;
            _flagPotionTimer = false;
        }
    }



}

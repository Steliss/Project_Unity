using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenrotyUI : MonoBehaviour
{

    [SerializeField] private Image[] _inventoryImage;
    [SerializeField] private Sprite _iconPotion;
    [SerializeField] private Sprite _iconKey;

    [SerializeField] private CreateItem _createItem;
    [SerializeField] private UseItem _useItem;

    // 리스트 들고와서 리스트 기입하기,
    private List<GameObject> _objInventoryList = new List<GameObject>();
    private Dictionary<GameObject, Sprite> _obInventoryDiction = new Dictionary<GameObject, Sprite>();
    private List<GameObject> _keyList;
    private List<GameObject> _potionList;


    public void InventorySetting()
    {
        _keyList = _createItem.KeyList;
        _potionList = _createItem.PotionList;

        _objInventoryList.Clear();
        _obInventoryDiction.Clear();

        if(_createItem == null ||_useItem == null)
        {
            Log.LogNull(nameof(InvenrotyUI), nameof(InventorySetting));
        }

        for (int i = 0; i < _inventoryImage.Length; i++)
        {
            // index를 통한 저장이 없으면 i를 가르키기에 [i]가 전부 같은걸 가르키게됨
            int index = i;
            _inventoryImage[i].GetComponent<Button>().onClick.AddListener(() => InventoryClick(index));
        }
    }


    public void InventoryAdd(GameObject Item)
    {
        if (Item == null || _obInventoryDiction.ContainsKey(Item))
        {
            return;
        }

        Sprite icon;

        if (_keyList.Contains(Item))
        {
            icon = _iconKey;
        }
        else if (_potionList.Contains(Item))
        {
            icon = _iconPotion;
        }
        else
        {
            Debug.LogWarning("TestLog : 키 또는 포션 목록에 없는 아이템");
            return;
        }

        _objInventoryList.Add(Item);
        _obInventoryDiction.Add(Item, icon);

        InventoryUpdate();
    }

    public void InventoryRemove(GameObject Item)
    {
        _objInventoryList.Remove(Item);
        _obInventoryDiction.Remove(Item);

        InventoryUpdate();
    }

    private void InventoryUpdate()
    {
        // null 항목이 있으면 제거
        _objInventoryList.RemoveAll(obj => obj == null);

        for (int i = 0; i < _inventoryImage.Length; i++)
        {
            if (i < _objInventoryList.Count)
            {
                GameObject item = _objInventoryList[i];

                _inventoryImage[i].sprite = _obInventoryDiction[item];
                _inventoryImage[i].gameObject.SetActive(true);
            }
            else
            {
                _inventoryImage[i].sprite = null;
                _inventoryImage[i].gameObject.SetActive(false);
            }
        }
    }

    private void InventoryClick(int index)
    {
        if (index < 0 || index >= _objInventoryList.Count)
        {
            return;
        }

        GameObject item = _objInventoryList[index];

        if (!_obInventoryDiction.TryGetValue(item, out Sprite icon))
        {
            Debug.Log("TestLog : 딕셔너리에 해당 아이템 없음");
            return; 
        }

        if (icon == _iconPotion)
        {
            _useItem.PotionUse();
        }
        else
        {
            Debug.LogWarning("TestLog : 일치하는 아이템 없음");
        }
    }
}

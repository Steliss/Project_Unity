using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowRelicUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;

    [SerializeField] private Slider _slider;
    [SerializeField] private ScrollRect _scrollRect;

    private SaveData _saveData;
    private GameObject _relicUI;

    private readonly GameObject[] _relicSlots = new GameObject[10];
    private readonly TextMeshProUGUI[] _nameTexts = new TextMeshProUGUI[10];
    private readonly TextMeshProUGUI[] _descriptionTexts = new TextMeshProUGUI[10];

    private void Awake()
    {
        _relicUI = transform.Find("RelicUI").gameObject;
    }

    private void Start()
    {
        _saveData = ManagerDontDestroy.Instance.SaveData;
        if (_saveData == null)
        {
            Log.LogNull(nameof(ShowRelicUI), nameof(Start), nameof(_saveData));
        }

        for (int i = 0; i < _relicSlots.Length; i++)
        {
            Transform slot = _relicUI.transform.Find($"Viewport/ShowRelic/RelicInfo{i}");

            if (slot == null)
            {
                continue;
            }

            _relicSlots[i] = slot.gameObject;
            _nameTexts[i] = slot.Find("NameText").GetComponent<TextMeshProUGUI>();
            _descriptionTexts[i] = slot.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        }

        UpdateRelicUI();
        _relicUI.SetActive(false);

        SliderSetting();
    }

    private void SliderSetting()
    {
        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.wholeNumbers = false;

        _slider.SetValueWithoutNotify(1f);
        _scrollRect.verticalNormalizedPosition = 1f;
        _scrollRect.movementType = ScrollRect.MovementType.Clamped;

        _slider.onValueChanged.AddListener(value =>
        {
            _scrollRect.verticalNormalizedPosition = value;
        });

        // 마우스 휠이나 드래그로 스크롤해도 슬라이더 위치를 맞춤
        _scrollRect.onValueChanged.AddListener(position =>
        {
            _slider.SetValueWithoutNotify(position.y);
        });
    }


    public void ToggleRelicUI()
    {
        if (_relicUI.activeSelf)
        {
            _relicUI.SetActive(false);
            return;
        }

        UpdateRelicUI();
        _relicUI.SetActive(true);

        // 활성화된 유물 개수에 맞춰 콘텐츠 크기 갱신
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        Canvas.ForceUpdateCanvases();

        // 열 때 맨 위에서 시작
        _scrollRect.StopMovement();
        _scrollRect.verticalNormalizedPosition = 1f;
        _slider.SetValueWithoutNotify(1f);
    }


    private void UpdateRelicUI()
    {
        for (int i = 0; i < _relicSlots.Length; i++)
        {
            if (_relicSlots[i] == null)
            {
                continue;
            }

            _relicSlots[i].SetActive(false);
        }

        _pointsText.text = $"포인트 : {_saveData.Points}";

        foreach (RelicOwnedData relic in _saveData.Relics)
        {
            if (relic == null || relic.Level <= 0)
            {
                continue;
            }

            int index = (int)relic.Type;

            if (index < 0 || index >= _relicSlots.Length)
            {
                continue;
            }

            if (_relicSlots[index] == null)
            {
                continue;
            }

            _nameTexts[index].text = relic.Name;
            _descriptionTexts[index].text = relic.Description;

            _relicSlots[index].SetActive(true);
        }

    }
}
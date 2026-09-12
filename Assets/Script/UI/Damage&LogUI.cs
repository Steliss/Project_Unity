using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageLogUI : MonoBehaviour
{
    [Header("데미지 텍스트")]
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private int _damageTextCount = 5;
    [SerializeField] private float _damageTextTimer = 5f;
    [SerializeField] private float _heightOffset = 2f;

    [Header("로그 텍스트")]
    [SerializeField] private TextMeshProUGUI _logText;
    [SerializeField] private RectTransform _logContent;
    [SerializeField] private int _logCount = 10;

    [SerializeField] private Camera _MainCamera;

    private RectTransform _damageTextRoot;

    private readonly Queue<TextMeshProUGUI> _damageTextQueue = new();
    private readonly List<TextMeshProUGUI> _damageTextList = new();
    private readonly Dictionary<TextMeshProUGUI, float> _damageTextDictionTime = new();
    private readonly Dictionary<TextMeshProUGUI, Vector3> _damageTextDictionPos = new();

    private readonly Queue<TextMeshProUGUI> _logQueue = new();

    private void Awake()
    {
        DamageTextSetting();
    }

    private void Update()
    {
        DamageTextToPool();
    }
    private void LateUpdate()
    {
        DamafeTextPosUpdate();
    }


    private void DamageTextSetting()
    {
        if (_damageTextRoot != null)
        {
            return;
        }

        GameObject root = new GameObject(
            "DamageTextRoot",
            typeof(RectTransform)
        );

        _damageTextRoot = root.GetComponent<RectTransform>();
        _damageTextRoot.SetParent(transform, false);

        // 부모 UI 전체 영역에 맞추기
        _damageTextRoot.anchorMin = Vector2.zero;
        _damageTextRoot.anchorMax = Vector2.one;
        _damageTextRoot.offsetMin = Vector2.zero;
        _damageTextRoot.offsetMax = Vector2.zero;

        for (int i = 0; i < _damageTextCount; i++)
        {
            TextMeshProUGUI text = Instantiate(
                _damageText,
                _damageTextRoot
            );

            text.text = "";
            text.raycastTarget = false;
            text.gameObject.SetActive(false);

            _damageTextQueue.Enqueue(text);

            // 대기 상태이므로 남은 시간은 0
            _damageTextDictionTime.Add(text, 0f);
        }
    }

    // 제한 시간이 끝난 텍스트를 풀로 반환
    private void DamageTextToPool()
    {
        for (int i = _damageTextList.Count - 1; i >= 0; i--)
        {
            TextMeshProUGUI text = _damageTextList[i];

            if (!_damageTextDictionTime.ContainsKey(text))
            {
                Debug.LogWarning($"{text.name}의 시간 정보가 없습니다.");

                ReturnDamageTextToPool(text);
                continue;
            }

            _damageTextDictionTime[text] -= Time.deltaTime;

            if (_damageTextDictionTime[text] > 0f)
            {
                continue;
            }

            ReturnDamageTextToPool(text);
        }
    }

    // 텍스트를 풀로 반환
    public void ReturnDamageTextToPool(TextMeshProUGUI text)
    {
        if (!_damageTextList.Remove(text))
        {
            return;
        }

        _damageTextDictionTime[text] = 0f;

        text.text = string.Empty;
        text.gameObject.SetActive(false);
        text.transform.SetParent(_damageTextRoot, false);

        _damageTextQueue.Enqueue(text);
        _damageTextDictionPos.Remove(text);
    }

    public void ShowDamage(Vector3 hitPosition, float damage, bool isCritical)
    {
        // 적 머리 위의 위치를 화면 좌표로 변환
        Vector3 screenPosition = _MainCamera.WorldToScreenPoint(hitPosition + Vector3.up * _heightOffset);

        // 카메라 뒤에 있는 대상은 표시하지 않음
        if (screenPosition.z <= 0f)
        {
            return;
        }

        // 대기 중인 텍스트가 없으면 가장 오래된 텍스트 반환
        if (_damageTextQueue.Count == 0)
        {
            if (_damageTextList.Count == 0)
            {
                return;
            }

            ReturnDamageTextToPool(_damageTextList[0]);
        }

        TextMeshProUGUI text = _damageTextQueue.Dequeue();

        // 맞은 위치 저장
        _damageTextDictionPos[text] = hitPosition + Vector3.up * _heightOffset;

        // 화면 좌표를 부모 UI의 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_damageTextRoot, screenPosition, null, out Vector2 uiPosition);

        // 부모의 피벗을 기준으로 위치 지정
        text.rectTransform.anchorMin = _damageTextRoot.pivot;
        text.rectTransform.anchorMax = _damageTextRoot.pivot;
        text.rectTransform.pivot = new Vector2(Random.Range(0.3f, 0.8f), Random.Range(0.3f, 0.8f));
        text.rectTransform.anchoredPosition = uiPosition;

        // 숫자와 치명타 표현
        text.text = $"{damage:0}";
        text.color = isCritical ? Color.yellow : Color.green;
        text.rectTransform.localScale = Vector3.one * (isCritical ? 1.3f : 1f);

        // 표시 목록에 등록하고 남은 시간 초기화
        _damageTextList.Add(text);
        _damageTextDictionTime[text] = _damageTextTimer;

        text.gameObject.SetActive(true);
        text.rectTransform.SetAsLastSibling();
    }

    public void ShowLog(float damage, float HP, bool iscritical)
    {
        TextMeshProUGUI text;

        if (_logQueue.Count >= _logCount)
        {
            // 가장 오래된 텍스트 재사용
            text = _logQueue.Dequeue();
        }
        else
        {
            text = Instantiate(_logText, _logContent);
        }
        
        if(iscritical)
        {
            text.text = $"적에게 <color=yellow>{damage}</color> 피해를 입히고 HP는 <color=red>{HP}</color>입니다";
        }
        else
        {
            text.text = $"적에게 <color=green>{damage}</color> 피해를 입히고 HP는 <color=red>{HP}</color>입니다";
        }

        text.gameObject.SetActive(true);

        // 화면에서도 가장 아래로 이동
        text.transform.SetAsLastSibling();
        _logQueue.Enqueue(text);
    }

    public void AnyLog(string word)
    {
        TextMeshProUGUI text;

        if (_logQueue.Count >= _logCount)
        {
            text = _logQueue.Dequeue();
        }
        else
        {
            text = Instantiate(_logText, _logContent);
        }

        text.text = word;

        text.gameObject.SetActive(true);

        // 화면에서도 가장 아래로 이동
        text.transform.SetAsLastSibling();
        _logQueue.Enqueue(text);
    }

    private void DamafeTextPosUpdate()
    {
        foreach (TextMeshProUGUI text in _damageTextList)
        {
            Vector3 worldPosition = _damageTextDictionPos[text];
            Vector3 screenPosition = _MainCamera.WorldToScreenPoint(worldPosition);

            // 카메라 뒤로 넘어가면 숨기기
            text.enabled = screenPosition.z > 0f;

            if (!text.enabled)
            {
                continue;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_damageTextRoot, screenPosition, null, out Vector2 uiPosition);

            text.rectTransform.anchoredPosition = uiPosition;
        }
    }
}
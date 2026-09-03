using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardChoiceManager : MonoBehaviour
{
    [SerializeField] private bool _testLog = false;

    [Header("보상 선택 UI")]
    [SerializeField] private GameObject _rewardChoicePanel;

    [SerializeField] private Button[] _rewardButtons;
    [SerializeField] private TextMeshProUGUI[] _nameTexts;
    [SerializeField] private TextMeshProUGUI[] _descriptionTexts;

    private GameData _gameData;
    private PlayerData _playerData;

    private readonly List<IReward> _rewardPool = new List<IReward>();
    private readonly List<IReward> _currentChoices = new List<IReward>();

    private bool _isSelecting;

    public bool IsSelecting => _isSelecting;

    public enum RewardSize
    {
        None,
        Small,
        Middle,
        High
    }


    private void Start()
    {
        _gameData = ManagerDontDestroy.Instance.GameData;
        if (_gameData == null)
        {
            Log.LogNull(nameof(RewardChoiceManager), nameof(Start), nameof(_gameData));
            return;
        }

        _playerData = _gameData._PlayerData;
        if (_playerData == null)
        {
            Log.LogNull(nameof(RewardChoiceManager), nameof(Start), nameof(_playerData));
            return;
        }

        RewardPoolSetting();
        ButtonSetting();

        _rewardChoicePanel.SetActive(false);

    }

    private void Update()
    {
        // 테스트용
        if (!_isSelecting)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                OpenChoices();
            }

            return;
        }

    }

    public void ButtonSetting()
    {
        for (int i = 0; i < _rewardButtons.Length; i++)
        {
            int index = i;

            _rewardButtons[i].onClick.AddListener(() => SelectReward(index));
        }
    }


    private void RewardPoolSetting()
    {
        _rewardPool.Clear();

        // 옵션 여러개 만들기 + 일반 희귀 구분 + 플레이 자체를 바꾸는 선택지 추가 (용 불쏘는걸 오브젝트 + 파밍 시간 증가 + 일회성 보스 체력 감소 
        // 일반 희귀 값 넣을때 serialize or 깡 넣기 고민 

        _rewardPool.Add(new AttackPowerReward(10f, RewardSize.Small, false));
        _rewardPool.Add(new AttackPowerReward(50f, RewardSize.High, false));
        _rewardPool.Add(new AttackSpeedReward(1f, RewardSize.Small, false));
        _rewardPool.Add(new AttackSpeedReward(50f, RewardSize.High, false));
        _rewardPool.Add(new AttackRangeReward(5f, RewardSize.Small, false));

        _rewardPool.Add(new MoveSpeedReward(10f, RewardSize.None, false));
        _rewardPool.Add(new RotateSpeedReward(10f, RewardSize.None, false));

        _rewardPool.Add(new CriticalChanceReward(5f, RewardSize.Small, true));
        _rewardPool.Add(new CriticalDamageReward(10f, RewardSize.Small, true));
    }


    private void SelectReward(int index)
    {
        if (!_isSelecting)
        {
            return ;
        }

        if (index < 0 || index >= _currentChoices.Count)
        {
            Debug.LogWarning("유효하지 않은 선택지 번호입니다.");
            return ;
        }

        IReward selectedReward = _currentChoices[index];

        selectedReward.Apply(_playerData);

        // 일회성 선택지 삭제
        if (selectedReward.removeAfterSelect)
        {
            _rewardPool.Remove(selectedReward);
        }


        if (_testLog)
        {
            Debug.Log($"{selectedReward.Name} 선택 완료\n" + $"{selectedReward.Description}");
        }

        CloseChoices();
    }


    public void OpenChoices()
    {
        _isSelecting = true;

        //if (_isSelecting)
        //{
        //    Debug.LogWarning("선택지가 이미 열려 있습니다");
        //    return;
        //}

        if (_playerData == null)
        {
            Log.LogNull(nameof(RewardChoiceManager), nameof(OpenChoices), nameof(_playerData));
            return;
        }

        if (_rewardPool.Count < 3)
        {
            if (_testLog)
            {
                Debug.Log("TestLog 남은 선택지 3개 이하");
            }
            return;
        }

        _currentChoices.Clear();

        // 원본 보상 목록을 수정하지 않도록 복사
        List<IReward> copydada = new List<IReward>(_rewardPool);

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, copydada.Count);

            _currentChoices.Add(copydada[randomIndex]);

            // 같은 화면에서 동일 보상이 다시 나오지 않도록 제거
            copydada.RemoveAt(randomIndex);
        }

        _rewardChoicePanel.SetActive(true);

        // 일시정지 
        Time.timeScale = 0f;

        UpdateChoiceUI();

        if (_testLog)
        {
            Debug.Log("===== 보상을 선택하세요 =====");
            for (int i = 0; i < _currentChoices.Count; i++)
            {
                IReward reward = _currentChoices[i];
                Debug.Log($"{i + 1}. {reward.Name}\n" + $"{reward.Description}");
            }
        }

    }
    private void UpdateChoiceUI()
    {
        for (int i = 0; i < _rewardButtons.Length; i++)
        {
            bool hasReward = i < _currentChoices.Count;

            _rewardButtons[i].gameObject.SetActive(hasReward);

            if (!hasReward)
            {
                continue;
            }

            IReward reward = _currentChoices[i];

            _nameTexts[i].text = reward.Name;
            _descriptionTexts[i].text = reward.Description;
        }
    }


    private void CloseChoices()
    {
        _currentChoices.Clear();
        _rewardChoicePanel.SetActive(false);

        // 일시정지 해제
        Time.timeScale = 1f;
        _isSelecting = false;

        if (_testLog)
        {
            Debug.Log("보상 선택이 종료되었습니다.");
        }
    }

    public interface IReward
    {
        string Name { get; }
        string Description { get; }
        bool removeAfterSelect { get; }

        void Apply(PlayerData playerData);
    }


    ///////////////////////////////////////
    /// 보상 목록
    ///////////////////////////////////////

    public class AttackPowerReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect; 

        public string Name => $"공격력 증가 {SizeName()}";
        public string Description => $"공격력이 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public AttackPowerReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddAttackPower(_value);
        }
    }


    public class AttackSpeedReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"공격속도 {SizeName()}";
        public string Description => $"공격력가 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public AttackSpeedReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddAttackSpeed(_value);
        }
    }

    public class AttackRangeReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"사거리 {SizeName()}";
        public string Description => $"사거리가 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public AttackRangeReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddAttackRange(_value);
        }
    }

    public class RotateSpeedReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"회전속도 {SizeName()}";
        public string Description => $"회전속도가 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public RotateSpeedReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddRotateSpeed(_value);
        }
    }


    public class MoveSpeedReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"이동속도 증가 {SizeName()}";
        public string Description => $"이동속도가 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public MoveSpeedReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddMoveSpeed(_value);
        }
    }

    public class CriticalChanceReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"크리티컬 확률 증가 {SizeName()}";
        public string Description => $"크리티컬 확률이 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public CriticalChanceReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddCriticalChance(_value);
        }
    }

    public class CriticalDamageReward : IReward
    {
        private readonly float _value;
        private readonly RewardSize _rewardSize;
        private readonly bool _removeAfterSelect;

        public string Name => $"크리티컬 배율 증가 {SizeName()}";
        public string Description => $"크리티컬 배율이 {_value} 만큼 증가합니다.";
        public bool removeAfterSelect => _removeAfterSelect;

        public CriticalDamageReward(float value, RewardSize rewardSize, bool removeAfterSelect)
        {
            _value = value;
            _rewardSize = rewardSize;
            _removeAfterSelect = removeAfterSelect;
        }

        private string SizeName()
        {
            switch (_rewardSize)
            {
                case RewardSize.Small:
                    return "(소)";

                case RewardSize.Middle:
                    return "(중)";

                case RewardSize.High:
                    return "(대)";

                default:
                    return string.Empty;
            }
        }

        public void Apply(PlayerData playerData)
        {
            playerData.AddCriticalDamageMultiplier(_value);
        }
    }

}
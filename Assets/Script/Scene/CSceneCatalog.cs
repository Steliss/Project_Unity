using System;
using System.Collections.Generic;
using UnityEngine;

// 코드에서 사용할 씬 식별자
public enum ESceneId
{
    Menu,
    Field,
    BossRoom
}

// Inspector에서 씬 ID와 이름을 연결하기 위한 데이터
[Serializable]
public class SceneEntry
{
    public ESceneId Id;
    public string sceneName;
}

public class CSceneCatalog : MonoBehaviour
{
    [Header("씬 목록")]
    [SerializeField] private List<SceneEntry> _scenes = new List<SceneEntry>();

    private readonly Dictionary<ESceneId, string> _idToName = new Dictionary<ESceneId, string>();

    HashSet<string> registeredNames = new HashSet<string>();


    public void BuildMaps()
    {
        _idToName.Clear();

        for (int i = 0; i < _scenes.Count; i++)
        {
            SceneEntry entry = _scenes[i];

            if (entry == null)
            {
                Debug.LogWarning($"씬 목록 {i}번 항목이 비어 있습니다.");
                continue;
            }

            if (string.IsNullOrEmpty(entry.sceneName))
            {
                Debug.LogWarning($"{entry.Id}의 씬 이름이 비어 있습니다.");
                continue;
            }

            if (_idToName.ContainsKey(entry.Id))
            {
                Debug.LogWarning($"중복된 씬 ID입니다: {entry.Id}");
                continue;
            }

            string name = entry.sceneName.Trim();

            if (!registeredNames.Add(name))
            {
                Debug.LogWarning($"중복된 씬 이름입니다: {name}");

                continue;
            }

            _idToName.Add(entry.Id, entry.sceneName);
        }

        //Debug.Log($"씬 카탈로그 생성 완료: {_idToName.Count}개");
    }

    public bool TryGetSceneName(ESceneId id, out string sceneName)
    {
        return _idToName.TryGetValue(id, out sceneName);
    }
}
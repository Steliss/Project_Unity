using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSceneManager : MonoBehaviour
{
    [Header("씬 카탈로그")]
    [SerializeField] private CSceneCatalog _catalog;

    private bool _isLoading;



    private void Awake()
    {

        if (_catalog == null)
        {
            _catalog = GetComponent<CSceneCatalog>();
        }

        if (_catalog == null)
        {
            Log.LogNull(nameof(CSceneManager), nameof(Awake));

            Destroy(gameObject);
            return;
        }

        _catalog.BuildMaps();
    }

    private void Update()
    {
        if (_isLoading)
        {
            return;
        }

        TestSceneInput();
    }

    private void TestSceneInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadScene(ESceneId.Menu);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadScene(ESceneId.Field);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadScene(ESceneId.BossRoom);
        }
    }

    public void LoadScene(ESceneId id)
    {
        if (_isLoading)
        {
            return;
        }

        if (!_catalog.TryGetSceneName(id, out string sceneName))
        {
            Debug.Log($"카탈로그에 등록되지 않은 씬입니다: {id}");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.Log($"로드할 수 없는 씬입니다: {sceneName}");
            return;
        }

        _isLoading = true;
        StartCoroutine(Co_LoadScene(id, sceneName));
    }

    private IEnumerator Co_LoadScene(ESceneId id, string sceneName)
    {

        //Debug.Log($"씬 이동 시작: {id} / {sceneName}");

        /*
         * 나중에 페이드 아웃을 넣을 위치
         *
         * yield return
         *     _transitionUI.Co_FadeTo(1f);
         */

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError($"씬 로드 요청 실패: {sceneName}");

            _isLoading = false;
            yield break;
        }

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log($"진행률 : {progress}");
            yield return null;
        }

        /*
         * 나중에 페이드 인을 넣을 위치
         *
         * yield return
         *     _transitionUI.Co_FadeTo(0f);
         */

        //Debug.Log($"씬 이동 완료: {sceneName}"
        

        _isLoading = false;
    }

}
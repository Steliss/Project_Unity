using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [Header("페이드 설정")]
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _minCircleSize = 0f;
    [SerializeField] private float _maxCircleSize = 2f;

    [SerializeField] private bool _useUnscaledTime = true;

    private Image _fadeImage;
    private RawImage _backGroundImage;
    private Canvas _canvas;
    

    private Material _fadeMaterial;

    private static readonly int FadeCircleSizeId = Shader.PropertyToID("_fFadeCircleSize");

    private void Awake()
    {
        _fadeImage = transform.Find("FadeInOut/FadeEffect").GetComponent<Image>();
        _backGroundImage = transform.Find("FadeInOut/FadeBackGround").GetComponent<RawImage>();
        _canvas = transform.Find("FadeInOut").GetComponent<Canvas>();

        if (_fadeImage == null || _backGroundImage == null || _canvas == null)
        {
            Debug.LogError($"{nameof(FadeInOut)} 철자 및 위치 확인");
            return;
        }
        if (_fadeImage.material == null)
        {
            Debug.LogError($"{nameof(FadeInOut)} " + "_fadeImage Material 확인");
            return;
        }

        _fadeMaterial = _fadeImage.material;

        if (!_fadeMaterial.HasProperty(FadeCircleSizeId))
        {
            Debug.LogError($"{nameof(FadeInOut)} " + "Material 이름 확인");
        }
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        _canvas.gameObject.SetActive(true);
        _backGroundImage.gameObject.SetActive(true);
        yield return FadeTo(_minCircleSize, duration, true);
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        _backGroundImage.gameObject.SetActive(false);
        yield return FadeTo(_maxCircleSize, duration, false);

        _canvas.gameObject.SetActive(false);
    }

    private IEnumerator FadeTo(float targetSize, float duration, bool blockRaycastWhileFading)
    {
        if (_fadeMaterial == null)
        {
            yield break;
        }

        // -1이면 Inspector의 기본 시간 사용
        if (duration < 0f)
        {
            duration = _fadeDuration;
        }

        float startSize = _fadeMaterial.GetFloat(FadeCircleSizeId);
        _fadeImage.raycastTarget = blockRaycastWhileFading;

        // 시간이 0이면 즉시 변경
        if (duration <= 0f)
        {
            _fadeMaterial.SetFloat(FadeCircleSizeId, targetSize);
            _fadeImage.raycastTarget = Mathf.Approximately(targetSize, _minCircleSize);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            timer += deltaTime;

            float ratio = Mathf.Clamp01(timer / duration);
            float circleSize = Mathf.Lerp(startSize, targetSize, ratio);

            _fadeMaterial.SetFloat(FadeCircleSizeId, circleSize);
            yield return null;
        }

        // 마지막 값을 정확하게 적용
        _fadeMaterial.SetFloat(FadeCircleSizeId, targetSize);

        // 화면이 완전히 가려졌을 때만 입력 차단
        _fadeImage.raycastTarget = Mathf.Approximately(targetSize, _minCircleSize);
    }
}
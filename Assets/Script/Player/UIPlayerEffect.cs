using UnityEngine;

public class UIPlayerEffect : MonoBehaviour
{
    [SerializeField] private GameObject _VFX;
    [SerializeField] private Transform _target;

    [SerializeField] private float _duration = 3f;
    [SerializeField] private float _scale = 0.5f;

    private GameObject _root;
    private GameObject _instance;
    private ParticleSystem[] _particles;

    private float _timer;
    private bool _isPlaying;

    private void Start()
    {
        _root = new GameObject("LevelUPEffect");

        _instance = Instantiate(_VFX, _root.transform);
        _instance.transform.localScale = Vector3.one * _scale;

        _particles = _instance.GetComponentsInChildren<ParticleSystem>(true);

        StopEffect();
    }

    private void Update()
    {
        if (_isPlaying)
        {
            _timer += Time.deltaTime;

            if (_timer >= _duration)
            {
                StopEffect();
            }
        }
    }

    public void PlayEffect()
    {
        _timer = 0f;
        _isPlaying = true;

        _instance.transform.position = _target.position;
        _instance.SetActive(true);

        foreach (ParticleSystem particle in _particles)
        {
            particle.gameObject.SetActive(true);
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play(false);
        }
    }

    private void StopEffect()
    {
        _isPlaying = false;
        _timer = 0f;

        foreach (ParticleSystem particle in _particles)
        {
            particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        _instance.SetActive(false);
    }
}
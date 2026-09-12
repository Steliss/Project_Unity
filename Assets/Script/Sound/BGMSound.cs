using UnityEngine;

public class BGMSound : MonoBehaviour
{
    [SerializeField] private AudioClip _titleBGM;
    [SerializeField] private AudioClip _fieldBGM1;
    [SerializeField] private AudioClip _fieldBGM2;
    [SerializeField] private AudioClip _fieldBGM3;
    [SerializeField] private AudioClip _bossBGM;

    [SerializeField] private AudioClip _endLossBGM;
    [SerializeField] private AudioClip _endWinBGM;

    public AudioClip TitleBGM => _titleBGM;
    public AudioClip FieldBGM1 => _fieldBGM1;
    public AudioClip FieldBGM2 => _fieldBGM2;
    public AudioClip FieldBGM3 => _fieldBGM3;
    public AudioClip BossBGM => _bossBGM;
    public AudioClip EndLossBGM => _endLossBGM;
    public AudioClip EndWinBGM => _endWinBGM;

}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 오디오 소스는 여기에 클립은 저기에 
    // 여기서 합쳐서 공개 함수로 내보내서 재생


    [SerializeField] private AudioSource _bgmAudio;
    [SerializeField] private AudioSource _sfxAudio;

    private PlaySound _playSound;
    private BGMSound _bgmSound;

    public AudioSource BGMAudio => _bgmAudio;

    public enum BGM
    {
        None,
        Title,
        Field1,
        Field2,
        Field3,
        Boss
    }

    private void Awake()
    {
        _playSound = transform.Find("SFX").GetComponent<PlaySound>();
        if (_playSound == null)
        {
            Log.LogNull(nameof(SoundManager), nameof(Awake), nameof(_playSound));
            return;
        }

        _bgmSound = transform.Find("BGM").GetComponent<BGMSound>();
        if (_playSound == null)
        {
            Log.LogNull(nameof(SoundManager), nameof(Awake), nameof(_bgmSound));
            return;
        }

        _bgmAudio.volume = 0.3f;
    }


    public void BGMSoundPlay(BGM bgm)
    {
        _bgmAudio.Stop();

        switch (bgm)
        {
            case BGM.None:
                break;

            case BGM.Title:
                _bgmAudio.loop = true;
                _bgmAudio.PlayOneShot(_bgmSound.TitleBGM);
                break;

            case BGM.Field1:
                _bgmAudio.loop = false;
                _bgmAudio.PlayOneShot(_bgmSound.FieldBGM1);
                break;

            case BGM.Field2:
                _bgmAudio.loop = false;
                _bgmAudio.PlayOneShot(_bgmSound.FieldBGM2);
                break;

            case BGM.Field3:
                _bgmAudio.loop = false;
                _bgmAudio.PlayOneShot(_bgmSound.FieldBGM3);
                break;

            case BGM.Boss:
                _bgmAudio.loop = true;
                _bgmAudio.PlayOneShot(_bgmSound.BossBGM);
                break;

            default:
                break;
        }
    }

    public void SFXGunfirePlay()
    {
        int rand = Random.Range(0,4);

        if(rand == 0)
        {
            _sfxAudio.PlayOneShot(_playSound.GunFireSound);
        }
        else if(rand == 1)
        {
            _sfxAudio.PlayOneShot(_playSound.GunFireSound2);
        }
        else if(rand == 2)
        {
            _sfxAudio.PlayOneShot(_playSound.GunFireSound3);
        }
        else
        {
            _sfxAudio.PlayOneShot(_playSound.GunFireSound4);
        }
    }

    public void SFXDrangonClawPlay()
    {
        _sfxAudio.PlayOneShot(_playSound.DragonClawSound);
    }
    public void SFXDrangonBlessPlay()
    {
        _sfxAudio.PlayOneShot(_playSound.DragonBlessSound);
    }
    public void SFXStartMenuPlay()
    {
        _sfxAudio.PlayOneShot(_playSound.StartMenuSound);
    }

}

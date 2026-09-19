using UnityEngine;

public class PlaySound : MonoBehaviour 
{
    [SerializeField] private AudioClip _gunFireSound;
    [SerializeField] private AudioClip _gunFireSound2;
    [SerializeField] private AudioClip _gunFireSound3;
    [SerializeField] private AudioClip _gunFireSound4;
    [SerializeField] private AudioClip _dragonBlessSound;
    [SerializeField] private AudioClip _dragonClawSound;
    [SerializeField] private AudioClip _startMenuSound;
    [SerializeField] private AudioClip _uiUpgradeButton_Success;
    [SerializeField] private AudioClip _uiUpgradeButton_Fail;


    public AudioClip GunFireSound => _gunFireSound;
    public AudioClip GunFireSound2 => _gunFireSound2;
    public AudioClip GunFireSound3 => _gunFireSound3;
    public AudioClip GunFireSound4 => _gunFireSound4;
    public AudioClip DragonBlessSound => _dragonBlessSound;
    public AudioClip DragonClawSound => _dragonClawSound;
    public AudioClip StartMenuSound => _startMenuSound;
    public AudioClip UiUpgrageButton_Success => _uiUpgradeButton_Success;
    public AudioClip UiUpgrageButton_Fail => _uiUpgradeButton_Fail;
}

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


    public AudioClip GunFireSound => _gunFireSound;
    public AudioClip GunFireSound2 => _gunFireSound2;
    public AudioClip GunFireSound3 => _gunFireSound3;
    public AudioClip GunFireSound4 => _gunFireSound4;
    public AudioClip DragonBlessSound => _dragonBlessSound;
    public AudioClip DragonClawSound => _dragonClawSound;
    public AudioClip StartMenuSound => _startMenuSound;

}

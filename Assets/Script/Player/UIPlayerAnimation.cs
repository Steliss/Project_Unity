using UnityEngine;

public class UIPlayerAnimation : MonoBehaviour
{

    [SerializeField] private GameObject _uiPlayer;
    private PlayerAnimation _uiPlayerAnimation;

    private void Start()
    {
        _uiPlayerAnimation = _uiPlayer.GetComponent<PlayerAnimation>();
        if (_uiPlayerAnimation == null)
        {
            Log.LogNull(nameof(PlayerBattle), nameof(Start), nameof(_uiPlayerAnimation));
        }
    }


}
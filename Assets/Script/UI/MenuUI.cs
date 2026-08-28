using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUI : MonoBehaviour
{

    [SerializeField] private Button StartButton;

    [SerializeField] private GameObject GOmanager;

    private CSceneManager _cSceneManager;


    void Start()
    {

        StartButton.onClick.AddListener(() => OnButtonClick("Start"));

        _cSceneManager = GOmanager.GetComponentInChildren<CSceneManager>();
        if (_cSceneManager == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_cSceneManager));
        }

    }

    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "Start")
        {
            _cSceneManager.LoadScene(ESceneId.Field);
        }
    }

}

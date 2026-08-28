using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{

    [SerializeField] private Button FieldButton;

    private CSceneManager _cSceneManager;


    void Start()
    {

        FieldButton.onClick.AddListener(() => OnButtonClick("Field"));

        ManagerDontDestroy manager = ManagerDontDestroy.Instance;

        _cSceneManager = manager.SceneManager;
        if (_cSceneManager == null)
        {
            Log.LogNull(nameof(FiledUI), nameof(Start), nameof(_cSceneManager));
        }

    }

    private void OnButtonClick(string buttonType)
    {
        if (buttonType == "Field")
        {
            _cSceneManager.LoadScene(ESceneId.Field);
        }
    }

}
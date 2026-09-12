using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private RectTransform _damageUI;
    [SerializeField] private TMPro.TextMeshProUGUI _damageText;

    public void ShowDamage(Vector3 enemyPosition, float damage)
    {
        Vector3 worldPosition = enemyPosition + Vector3.up * 2f;
        Vector3 screenPosition = _worldCamera.WorldToScreenPoint(worldPosition);

        _damageUI.position = screenPosition;
        _damageText.text = $"{damage:0}";
        _damageUI.gameObject.SetActive(true);
    }


}
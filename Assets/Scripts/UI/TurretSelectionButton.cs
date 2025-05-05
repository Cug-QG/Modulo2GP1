using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurretSelectionButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject turretPrefab;
    [SerializeField] TextMeshProUGUI cost;

    private void Start()
    {
        cost.text = "" + turretPrefab.GetComponent<Placeable>().GetValue();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (turretPrefab.GetComponent<Placeable>().GetValue() > GameManager.Instance.GetCoins()) { return; }
        UIManager.Instance.StartPlacing(turretPrefab);
    }
}

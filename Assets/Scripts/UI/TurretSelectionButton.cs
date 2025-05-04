using UnityEngine;
using UnityEngine.EventSystems;

public class TurretSelectionButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject turretPrefab;
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.StartPlacing(turretPrefab);
    }
}

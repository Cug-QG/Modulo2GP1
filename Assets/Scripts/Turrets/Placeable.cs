using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour 
{
    [SerializeField] private string turretName;
    [SerializeField] private float value;
    [SerializeField] protected GameObject turretSelection;
    public virtual void ShowRange()
    {
        turretSelection.SetActive(true);
        UIManager.Instance.SetInfo("Turret: " + turretName);
    }
    public virtual void HideRange() { turretSelection.SetActive(false); UIManager.Instance.SetInfo(""); }

    public float GetValue() { return value; }
}

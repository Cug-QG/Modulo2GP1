using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour 
{
    [SerializeField] protected GameObject turretSelection;
    public virtual void ShowRange()
    {
        turretSelection.SetActive(true);
    }
    public virtual void HideRange() { turretSelection.SetActive(false); }
}

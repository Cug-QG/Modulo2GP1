using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretSlot : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private BoxCollider spawnCollider;
    [SerializeField] private int maxTurrets;
    [SerializeField] private List<GameObject> turrets;

    public void PlaceTurret(GameObject turret)
    {
        if (!Spawnable(turret)) { return; }
        GameObject turretInstance = Instantiate(turret, spawnPoint.position, Quaternion.identity);
        AssignPrefab(turret, turretInstance);
        AssignSlot(turretInstance);
        turrets.Add(turretInstance);
        
        spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 1, spawnPoint.position.z);
        spawnCollider.center = new Vector3(spawnCollider.center.x, spawnPoint.localPosition.y, spawnCollider.center.z);

        ApplyEffects();
        if (turretInstance.GetComponent<SupportTurret>() != null) { return; }
        SphereCollider radius = turretInstance.GetComponent<SphereCollider>();
        radius.center = new Vector3(radius.center.x, -turretInstance.transform.position.y, radius.center.z);
    }

    private void AssignPrefab(GameObject prefab, GameObject turret)
    {
        turret.GetComponent<Turret>()?.SetOriginalPrefab(prefab);
        turret.GetComponent<SupportTurret>()?.SetOriginalPrefab(prefab);
    }

    private void AssignSlot(GameObject turret)
    {
        turret.GetComponent<Turret>()?.SetSlot(gameObject.GetComponent<TurretSlot>());
        turret.GetComponent<SupportTurret>()?.SetSlot(gameObject.GetComponent<TurretSlot>());
    }

    public bool Spawnable(GameObject turret)
    {
        if (turrets.Count == maxTurrets) { return false; }
        if (turret.GetComponent<SupportTurret>() == null) return true;
        bool result = false;
        foreach (GameObject t in turrets)
        {
            if (t.GetComponent<Turret>() != null) { result = true; break; }
        }
        return result;
    }

    private void ApplyEffects() 
    {
        foreach (GameObject t in turrets) 
        {
            if (t.GetComponent<SupportTurret>() == null) { continue; }
            ApplyEffect(t);
        }
    }

    private void ApplyEffect(GameObject turret) 
    {
        foreach (GameObject t in turrets) 
        {
            if (t.GetComponent<Turret>() == null) { continue; }
            turret.GetComponent<SupportTurret>().ApplyEffect(t.GetComponent<Turret>());
        }
    }

    public void DestroyTurret(GameObject turret)
    {
        RemoveEffects();
        turrets.Remove(turret);
        spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y - 1, spawnPoint.position.z);
        spawnCollider.center = new Vector3(spawnCollider.center.x, spawnPoint.localPosition.y, spawnCollider.center.z);
        Destroy(turret);
        ApplyEffects();
    }

    private void RemoveEffects()
    {
        foreach (GameObject t in turrets)
        {
            if (t.GetComponent<SupportTurret>() == null) { continue; }
            RemoveEffect(t);
            t.GetComponent<SupportTurret>().ClearList();
        }
    }

    private void RemoveEffect(GameObject turret)
    {
        foreach (GameObject t in turrets)
        {
            if (t.GetComponent<Turret>() == null) { continue; }
            turret.GetComponent<SupportTurret>().RemoveEffect(t.GetComponent<Turret>());
        }
    }

    public bool IsAbove(GameObject turret)
    {
        for (int i = 0; i < turrets.Count; i++)
        {
            if (turrets[i] == turret && i == turrets.Count - 1) { return true; }
        }
        return false;
    }

    public Transform GetSpawnPoint() { return spawnPoint; }
}
using UnityEngine;

public class AreaTargetShoot : ShootEffect
{
    [SerializeField] AOEEffect aoeEffectPrefab;
    public override void FireAtTarget(Vector3 targetPosition, float scale, Transform parent) 
    { 
        Instantiate(aoeEffectPrefab, new Vector3(firePoint.position.x, 0f, firePoint.position.z) + Vector3.up * 0.01f, Quaternion.identity, parent);
    }
}


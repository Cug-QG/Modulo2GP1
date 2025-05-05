using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentAreaEffect : ShootEffect
{
    [SerializeField] GameObject aoeEffectPrefab;
    Transform effect;
    public override void FireAtTarget(Vector3 targetPosition, float scale, Transform parent) 
    { 
        effect = Instantiate(aoeEffectPrefab, new Vector3(firePoint.position.x, 0f, firePoint.position.z) + Vector3.up * 0.01f, Quaternion.identity, parent).transform;
        effect.transform.localScale = new Vector3(scale, 1f, scale); 
    }

    public void SetUpEffect(float scale)
    {
        effect.transform.localScale = new Vector3(scale, 1f, scale);
    }

    public void HideEffect() { if (effect!=null) { Destroy(effect.gameObject); } }
}

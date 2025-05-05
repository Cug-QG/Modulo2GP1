using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootEffect : MonoBehaviour
{
    [SerializeField] protected LineRenderer lineRenderer;
    [SerializeField] protected float beamDuration = 0.1f;
    [SerializeField] protected Transform firePoint;
    public virtual void FireAtTarget(Vector3 targetPosition, float scale, Transform parent) { }
}

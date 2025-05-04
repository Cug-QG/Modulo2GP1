using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ShootEffectInterface
{
    public virtual void FireAtTarget(Vector3 targetPosition, float scale) { }
}

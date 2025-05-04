using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetShoot : ShootEffect
{
    private void Start()
    {
        lineRenderer.enabled = false;
    }
    public override void FireAtTarget(Vector3 targetPosition, float scale, Transform parent)
    {
        StartCoroutine(ShowBeam(targetPosition));
    }

    private IEnumerator ShowBeam(Vector3 targetPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, targetPosition);

        yield return new WaitForSeconds(beamDuration);

        lineRenderer.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : Placeable
{
    [SerializeField] TurretData data;
    [SerializeField] Transform turretBase;
    [SerializeField] Transform turretGun;
    [SerializeField] ShootEffect shooter;

    public float damage;
    public float fireCooldown;
    private float currentCooldown = 0;
    private bool canShoot = true;
    private float radius;

    private GameObject originalPrefab;
    private TurretSlot slot;

    private List<Transform> targetsInRange = new();
    private List<Transform> targets = new();
    private delegate void SearchDelegate();
    private SearchDelegate Searching;
    public delegate void DamageDelegate(Transform target);
    public DamageDelegate Damaging;
    public delegate void StatusDelegate(Transform target);
    public StatusDelegate ApplyingStatus;



    [SerializeField] protected GameObject range;

    public override void ShowRange()
    {
        base.ShowRange();
        range.SetActive(true);
        range.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
    }

    public override void HideRange()
    {
        base.HideRange();
        range.SetActive(false);
    }

    void Awake()
    {
        damage = data.Damage;
        fireCooldown = 1f / data.FireRate;
        radius = data.Radius;
        GetComponent<SphereCollider>().radius = data.Radius;
        SetSearchMode();
        if (data.FireRate != 0) { Damaging = DoDamage; } else { Damaging = DoDamageTroughTime; }
        range.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
    }

    void Update()
    {
        if (currentCooldown > 0f) { Cooldown(); }
        if (targetsInRange.Count < 1) 
        {
            if (data.FireRate == 0) {
                PermanentAreaEffect areaEffectShooter = shooter as PermanentAreaEffect;
                areaEffectShooter.HideEffect();
                effectActive = false;
            }
            return; 
        }
        Searching?.Invoke();
        if (canShoot) { Shoot(); }
    }

    private void Cooldown()
    {
        currentCooldown -= Time.deltaTime;

        if (currentCooldown <= 0f)
        {
            canShoot = true;
        }
    }

    bool effectActive = false;
    private void Shoot()
    {
        if (targets.Count > 1)
        {
            ShowEffect(transform.position);
            foreach (Transform t in targets)
            {
                Damaging?.Invoke(t);
                ApplyingStatus?.Invoke(t);
            }
        }
        else if (targets.Count>0)
        {
            Transform t = targets[0];
            if (t == null) { return; }
            Damaging?.Invoke(t);
            ApplyingStatus?.Invoke(t);
            ShowEffect(t.position);
        }
        
        if (data.FireRate != 0) canShoot = false;
        currentCooldown = fireCooldown;
    }

    private void ShowEffect(Vector3 pos)
    {
        if (data.FireRate != 0) { shooter.FireAtTarget(pos, radius*2, transform); }
        else 
        {
            if (effectActive == true) 
            {
                PermanentAreaEffect areaEffectShooter = shooter as PermanentAreaEffect;
                areaEffectShooter.SetUpEffect(radius * 2);
                return;
            }
            shooter.FireAtTarget(pos, radius * 2, transform);
            effectActive = true;
        }
    }

    private void RemoveNullTargets()
    {
        targetsInRange.RemoveAll(target => target == null);
        targets.Clear();
    }
    private void LookAtTarget()
    {
        if (targets[0] == null) { return; }
        RotateBase();
        RotateGun();
    }

    void RotateBase()
    {
        // Calcola la direzione verso il target
        Vector3 direction = targets[0].position - turretBase.position;
        direction.y = 0; // Mantieni solo la rotazione orizzontale

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            turretBase.rotation = targetRotation;
        }
    }

    void RotateGun()
    {
        // Calcola la direzione verticale dal cannone al target
        Vector3 direction = targets[0].position - turretGun.position;

        // Trova l'angolo verticale
        float angle = Mathf.Atan2(direction.y, direction.magnitude) * Mathf.Rad2Deg;

        // Applica la rotazione solo sull'asse X
        turretGun.localRotation = Quaternion.Euler(-angle, 0, 0);
    }

    private void SetSearchMode()
    {
        Searching = RemoveNullTargets;
        switch (data.FireType)
        {
            case FireType.Area:
                Searching += AreaSearch;
                break;
            case FireType.Nearest:
                Searching += NearestEnemySearch;
                break;
            case FireType.Farther:
                Searching += FartherEnemySearch;
                break;
            default:
                break;
        }
        if (turretGun != null) { Searching += LookAtTarget; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targetsInRange.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Enemy))
        {
            targetsInRange.Remove(other.transform);
        }
    }

    private void AreaSearch()
    {
        foreach (Transform t in targetsInRange)
        {
            if (t == null) { continue; }
            targets.Add(t);
        }
    }

    private void NearestEnemySearch()
    {
        float distance = float.MaxValue;
        Transform target = null;
        foreach (Transform t in targetsInRange)
        {
            if (t == null) { continue; }
            if (Vector3.Distance(transform.position, t.position) < distance) { distance = Vector3.Distance(transform.position, t.position); target = t; }
        }
        targets.Add(target);
    }

    private void FartherEnemySearch()
    {
        float distance = 0;
        Transform target = null;
        foreach (Transform t in targetsInRange)
        {
            if (t == null) { continue; }
            if (Vector3.Distance(transform.position, t.position) > distance) { distance = Vector3.Distance(transform.position, t.position); target = t; }
        }
        targets.Add(target);
    }

    private void DoDamage(Transform target) { target?.GetComponent<Enemy>().TakeDamage(damage); }
    private void DoDamageTroughTime(Transform target) { target?.GetComponent<Enemy>().TakeDamage(damage * Time.deltaTime); }

    public float GetFireCooldown() { return fireCooldown; }
    public void SetFireCooldown(float value) { fireCooldown = value; }
    public float GetDamage() { return damage; }
    public void SetDamage(float value) { damage = value; }
    public float GetRange() { return radius; }
    public void SetRange(float value) { radius = value; GetComponent<SphereCollider>().radius = radius; range.transform.localScale = new Vector3(radius * 2, radius * 2, 1f); }

    public void SetOriginalPrefab(GameObject prefab) { originalPrefab = prefab; }
    public GameObject GetOriginalPrefab() { return originalPrefab; }
    public void SetSlot(TurretSlot tSlot) { slot = tSlot; }
    public TurretSlot GetSlot() { return slot; }
}
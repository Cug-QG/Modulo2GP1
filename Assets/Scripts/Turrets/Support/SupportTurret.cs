using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SupportTurret : Placeable
{
    [SerializeField] EffectData effectData;

    private List<Turret> alreadyApplied = new();

    public delegate void EffectDelegate(Turret turret);
    public EffectDelegate ApplyEffect;
    public delegate void StatusDelegate(Turret turret);
    public StatusDelegate StatusEffect;
    public delegate void EffectRemoveDelegate(Turret turret);
    public EffectDelegate RemoveEffect;

    private GameObject originalPrefab;
    private TurretSlot slot;

    private void Awake()
    {
        SetEffect();
    }

    private void SetEffect() 
    {
        switch (effectData.EffectType)
        {
            case EffectType.Default:
                break;
            case EffectType.BoostFireRate:
                ApplyEffect = BoostFireRate;
                RemoveEffect = RemoveBoostFireRate;
                break;
            case EffectType.BoostDamage:
                ApplyEffect = BoostDamage;
                RemoveEffect = RemoveBoostDamage;
                break;
            case EffectType.SlowEnemy:
                ApplyEffect = SlowEnemy;
                RemoveEffect = RemoveSlowEnemy;
                break;
            case EffectType.StunEnemy:
                //ApplyEffect = StunEnemy;
                //RemoveEffect = RemoveStunEnemy;
                break;
            case EffectType.PoisonEnemy:
                //ApplyEffect = PoisonEnemy;
                //RemoveEffect = RemovePoisonEnemy;
                break;
            case EffectType.BoostRange:
                ApplyEffect = BoostRange;
                RemoveEffect = RemoveBoostRange;
                break;
            default:
                break;
        }
    }

    private bool IsAlreadyApplied(Turret turret) { return alreadyApplied.Contains(turret); }

    private void BoostFireRate(Turret turret) 
    {
        if (IsAlreadyApplied(turret)) { return; }
        if (turret.GetFireCooldown() == Mathf.Infinity) { BoostDamage(turret); return; }
        turret.SetFireCooldown(turret.GetFireCooldown()/effectData.Multiplier);
        alreadyApplied.Add(turret);
    }

    private void RemoveBoostFireRate(Turret turret)
    {
        turret.SetFireCooldown(turret.GetFireCooldown() * effectData.Multiplier);
    }

    private void BoostDamage(Turret turret)
    {
        if (IsAlreadyApplied(turret)) { return; }
        turret.SetDamage(turret.GetDamage() * effectData.Multiplier);
        alreadyApplied.Add(turret);
    }

    private void RemoveBoostDamage(Turret turret)
    {
        turret.SetDamage(turret.GetDamage() / effectData.Multiplier);
    }

    private void BoostRange(Turret turret)
    {
        if (IsAlreadyApplied(turret)) { return; }
        turret.SetRange(turret.GetRange() * effectData.Multiplier);
        alreadyApplied.Add(turret);
    }

    private void RemoveBoostRange(Turret turret)
    {
        turret.SetRange(turret.GetRange() / effectData.Multiplier);
    }

    private void SlowEnemy(Turret turret) { if (IsAlreadyApplied(turret)) { return; } turret.ApplyingStatus += ApplyStatus; alreadyApplied.Add(turret); }
    private void RemoveSlowEnemy(Turret turret) { turret.ApplyingStatus -= ApplyStatus; }
    private void ApplyStatus(Transform target) 
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Passa una funzione che restituisce la coroutine
            enemy.ApplyStatus(StatusType.Slow, effectData.Duration, () => StatusCoroutine(StatusType.Slow, effectData.Duration, enemy, effectData.Multiplier));
        }
    }

    private IEnumerator StatusCoroutine(StatusType status, float duration, Enemy enemy, float value)
    {
        float baseSpeed = enemy.GetSpeed();
        enemy.SetSpeed(baseSpeed / value);

        yield return new WaitForSeconds(duration);

        enemy.activeStatuses.Remove(status);
        enemy.SetSpeed(baseSpeed);
    }


    //WORKING BUT UNBALANCED
    /*
    private void StunEnemy(Turret turret) { turret.ApplyingStatus += ApplyStatusstun; }
    private void RemoveStunEnemy(Turret turret) { turret.ApplyingStatus -= ApplyStatusstun; }
    private void ApplyStatusstun(Transform target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Passa una funzione che restituisce la coroutine
            enemy.ApplyStatus(StatusType.Stun, effectData.Duration, () => StunCoroutine(StatusType.Stun, effectData.Duration, enemy));
        }
    }

    private IEnumerator StunCoroutine(StatusType status, float duration, Enemy enemy)
    {
        enemy.SetStunStatus(true);

        yield return new WaitForSeconds(duration);

        enemy.SetStunStatus(false);
    }

    private void PoisonEnemy(Turret turret) { if (IsAlreadyApplied(turret)) { return; } turret.ApplyingStatus += ApplyPoison; alreadyApplied.Add(turret); }
    private void RemovePoisonEnemy(Turret turret) { turret.ApplyingStatus -= ApplyPoison; }
    private void ApplyPoison(Transform target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Passa una funzione che restituisce la coroutine
            enemy.ApplyStatus(StatusType.Poison, effectData.Duration, () => PoisonCoroutine(StatusType.Poison, effectData.Duration, enemy, effectData.Multiplier));
        }
    }

    private IEnumerator PoisonCoroutine(StatusType status, float duration, Enemy enemy, float value)
    {
        float time = duration;
        while (time > 0) 
        {
            time -= Time.deltaTime;
            enemy?.TakeDamage(value * Time.deltaTime);
            yield return null;
        }
    }*/

    public void ClearList() { alreadyApplied.Clear(); }

    public void SetOriginalPrefab(GameObject prefab)
    {
        originalPrefab = prefab;
    }

    public GameObject GetOriginalPrefab()
    {
        return originalPrefab;
    }
    public void SetSlot(TurretSlot tSlot) { slot = tSlot; }
    public TurretSlot GetSlot() { return slot; }
}

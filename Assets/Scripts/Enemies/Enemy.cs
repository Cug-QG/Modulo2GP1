using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    private int pathIndex = 1;
    public float currentHP;
    public float speed;
    private bool stunned = false;

    public Dictionary<StatusType, Coroutine> activeStatuses = new();

    private void Awake()
    {
        currentHP = data.HP;
        speed = data.Speed;
    }

    void Update()
    {
        if (stunned) { return; }
        if (Vector3.Distance(transform.position, WavesManager.Instance.pathPoints[pathIndex].position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, WavesManager.Instance.pathPoints[pathIndex].position, speed * Time.deltaTime);
            return;
        }

        pathIndex += 1;
        if (pathIndex == WavesManager.Instance.pathPoints.Count)
        {
            Damage();
        }
    }

    private void Damage()
    {
        WavesManager.Instance.livingEnemies.Remove(gameObject);
        GameManager.Instance.DecreseHP(data.Damage);
        Destroy(gameObject);
    }

    public void TakeDamage(float dmg) 
    {
        currentHP -= dmg;
        if (currentHP <= 0) 
        {
            GameManager.Instance.AddPoints(data.Points);
            WavesManager.Instance.livingEnemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    public void ApplyStatus(StatusType status, float duration, Func<IEnumerator> coroutineFunc)
    {
        if (activeStatuses.ContainsKey(status))
        {
            // Se esiste già, lo riavvia
            StopCoroutine(activeStatuses[status]);
        }

        // Avvia una nuova coroutine e la memorizza
        Coroutine newCoroutine = StartCoroutine(coroutineFunc());
        activeStatuses[status] = newCoroutine;
    }

    public float GetSpeed() { return data.Speed; }
    public void SetSpeed(float input) { speed = input; }
    public void SetStunStatus(bool input) { stunned = input; }
    public void ModifyHP(int multiplier) { currentHP *= multiplier; }
}

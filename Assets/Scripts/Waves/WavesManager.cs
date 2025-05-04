using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    private static WavesManager instance;
    private int multiplier = 1;

    public static WavesManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else { instance = this; }
    }

    [SerializeField] private List<Wave> waves;
    [SerializeField] public List<Transform> pathPoints;
    private int currentWaveIndex = 0;
    public List<GameObject> livingEnemies;

    public void StartWaves()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWaves());
            multiplier++;
        }
    }

    IEnumerator SpawnWaves()
    {
        //UIManager.Instance.SetMessagesText("Stage: " + (currentRoundIndex + 1));

        foreach (Wave wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        currentWaveIndex++;
        UIManager.Instance.SetWaveCounter(currentWaveIndex);
        foreach (var enemies in wave.enemies)
        {
            StartCoroutine(SpawnEnemies(enemies));
        }
        while (livingEnemies.Count!=0)
        {
            yield return null;
        }
    }

    IEnumerator SpawnEnemies(Wave.EnemiesSpawnInfo enemies)
    {
        for (int i = 0; i < enemies.number * multiplier; i++)
        {
            GameObject enemy = Instantiate(enemies.enemyPrefab, pathPoints[0].position, Quaternion.identity);
            enemy.GetComponent<Enemy>().ModifyHP(multiplier);
            livingEnemies.Add(enemy);
            yield return new WaitForSeconds(enemies.spawnDelay);
        }
    }
}

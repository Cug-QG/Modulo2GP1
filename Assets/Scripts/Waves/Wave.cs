using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Wave", fileName = "Wave")]
public class Wave : ScriptableObject
{
    [System.Serializable]
    public class EnemiesSpawnInfo
    {
        public GameObject enemyPrefab;
        public float number;
        public float spawnDelay = 0.5f;
    }

    public List<EnemiesSpawnInfo> enemies;
}
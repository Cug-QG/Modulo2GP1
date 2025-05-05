using UnityEngine;

[CreateAssetMenu(menuName = "SO/Enemy", fileName = "Enemy")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float hp;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float points;
    [SerializeField] private float coins;

    public float HP { get { return hp; } }
    public float Speed { get { return speed; } }
    public float Damage { get { return damage; } }
    public float Points { get { return points; } }
    public float Coins { get { return coins; } }
}

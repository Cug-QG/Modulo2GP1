using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Turret", fileName = "Turret")]
public class TurretData : ScriptableObject
{
    [SerializeField] private int damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float radius;
    [SerializeField] private FireType fireType;
    
    public float FireRate { get { return fireRate; } }
    public int Damage { get { return damage; } }
    public float Radius { get { return radius; } }
    public FireType FireType { get { return fireType; } }
}

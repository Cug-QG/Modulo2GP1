using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Effect", fileName = "Effect")]
public class EffectData : ScriptableObject
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private float multiplier;
    [SerializeField] private float duration;

    public EffectType EffectType { get { return effectType; } }
    public float Multiplier { get { return multiplier; } }
    public float Duration { get { return duration; } }
}

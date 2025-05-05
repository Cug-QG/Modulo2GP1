public class Tags
{
    public static readonly string Player = "Player";
    public static readonly string Enemy = "Enemy";
}

public enum FireType 
{
    Area,
    Nearest,
    Farther
}

public enum EffectType
{ 
    Default,
    BoostFireRate,
    BoostDamage,
    SlowEnemy,
    StunEnemy,
    PoisonEnemy,
    BoostRange
}

public enum StatusType
{ 
    Default,
    Slow,
    Stun,
    Poison
}
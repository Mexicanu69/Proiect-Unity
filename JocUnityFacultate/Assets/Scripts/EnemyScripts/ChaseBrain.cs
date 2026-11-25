using UnityEngine;

public class ChaseBrain : EnemyBrain
{
    // Chaser prost
    // Se duce intotdeauna la target, atata timp cat nu e oprit
    // (il opreste logica din EnemyBrain care opreste orice Brain cand
    //  ajunge in autoattack range)
    protected override Vector3 GetDestination()
    {
        return target.position;
    }
}
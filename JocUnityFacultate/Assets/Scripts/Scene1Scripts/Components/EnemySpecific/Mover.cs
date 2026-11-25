using UnityEngine;

public abstract class Mover : MonoBehaviour
{
    public abstract void SetSpeed(float speed);
    public abstract void MoveTo(Vector3 target);
    public abstract void Stop();
    public abstract void Teleport(Vector3 position);
}
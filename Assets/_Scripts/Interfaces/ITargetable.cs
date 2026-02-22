using UnityEngine;

public interface ITargetable
{
    Transform Transform { get; }
    bool IsDead { get; }

    float GetRadius();
}

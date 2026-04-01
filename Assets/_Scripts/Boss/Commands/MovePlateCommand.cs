using UnityEngine;

public class MovePlateCommand : ICommand
{
    public int TargetID { get; set; }
    public float TargetAngle { get; private set; }
    public Vector3 WeaponOffset { get; private set; }

    public MovePlateCommand(int id, float angle, Vector3 offset = default) 
    {   
        TargetID = id;
        TargetAngle = angle;
        WeaponOffset = offset;
    }
}

using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenesConfig", menuName = "Configs/Scenes")]
public class ScenesConfig : ScriptableObject
{
    [field: SerializeField] public SceneAsset GameScene { get; private set; }
    [field: SerializeField] public SceneAsset MenuScene { get; private set; }
}


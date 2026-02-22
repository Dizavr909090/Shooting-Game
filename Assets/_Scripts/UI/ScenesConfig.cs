using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenesConfig", menuName = "Configs/Scenes")]
public class ScenesConfig : ScriptableObject
{
    public SceneAsset gameScene;
    public SceneAsset menuScene;
}


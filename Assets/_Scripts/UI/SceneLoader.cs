using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISceneLoader
{
    [SerializeField] private ScenesConfig _sceneConfig;

    public void LoadGameScene()
    {
        SceneManager.LoadScene(_sceneConfig.GameScene.name);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(_sceneConfig.MenuScene.name);
    }
}

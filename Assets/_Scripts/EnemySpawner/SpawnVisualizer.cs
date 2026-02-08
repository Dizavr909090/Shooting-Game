using System.Collections;
using UnityEngine;

public class SpawnVisualizer : MonoBehaviour
{
    [SerializeField] private float _tileFlashSpeed = 4f;
    [SerializeField] private Color _flashColor = Color.red;

    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock _propBlock;

    private void Awake()
    {
        if (_propBlock == null)
            _propBlock = new MaterialPropertyBlock();
    }

    public IEnumerator StartBlink(Renderer renderer, float duration)
    {
        renderer.GetPropertyBlock(_propBlock);

        Color initialColor = renderer.sharedMaterial.color;

        float timer = 0;

        while (timer < duration)
        {
            var currentColor = Color.Lerp(initialColor, _flashColor, Mathf.PingPong(timer * _tileFlashSpeed, 1));
            _propBlock.SetColor(ColorProperty, currentColor);
            renderer.SetPropertyBlock(_propBlock);
            timer += Time.deltaTime;
            yield return null;
        }

        _propBlock.SetColor(ColorProperty, initialColor);
        renderer.SetPropertyBlock(_propBlock);
    }
}

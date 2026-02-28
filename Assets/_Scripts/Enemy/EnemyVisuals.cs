using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private EnemyAttack _attack;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private Color _originalColor;

    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

    private void OnDestroy()
    {
        if (_attack != null)
        {
            _attack.OnAttackStarted -= ShowAttackColor;
            _attack.OnAttackFinished -= ResetColor;
        }
    }

    public void Initialize(EnemyAttack attack)
    {
        _attack = attack;

        if (_renderer == null) _renderer = GetComponent<Renderer>();

        _propBlock = new MaterialPropertyBlock();
        _originalColor = _renderer.sharedMaterial.color;

        _attack.OnAttackStarted += ShowAttackColor;
        _attack.OnAttackFinished += ResetColor;
    }

    private void ShowAttackColor() => UpdateColor(Color.red);

    private void ResetColor() => UpdateColor(_originalColor);

    private void UpdateColor(Color color)
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(ColorProperty, color);
        _renderer.SetPropertyBlock(_propBlock);
    }
}

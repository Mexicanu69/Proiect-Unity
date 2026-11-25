using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HitFlash : MonoBehaviour
{
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float duration = 0.1f;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    Health health;
    Renderer[] renderers;
    MaterialPropertyBlock block;
    Coroutine routine;

    void Awake()
    {
        health = GetComponent<Health>();
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void OnEnable() => health.OnDamaged += Flash;

    void OnDisable()
    {
        health.OnDamaged -= Flash;
        ResetColor(); // important la pooling
    }

    void Flash(float _)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        SetColor(flashColor);
        yield return new WaitForSeconds(duration);
        ResetColor();
        routine = null;
    }

    void SetColor(Color c)
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetColor(BaseColorId, c);
            block.SetColor(ColorId, c);
            r.SetPropertyBlock(block);
        }
    }

    void ResetColor()
    {
        foreach (var r in renderers)
            r.SetPropertyBlock(null); // revine la culoarea din material
    }
}
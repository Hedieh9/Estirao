using UnityEngine;

public class SkyboxAnimation : MonoBehaviour
{
    [SerializeField] Material gradientMaterial;
    [SerializeField] float gradientHeightValue = 1f;
    private float lastGradientHeightValue = 0f;
    
    private void Start()
    {
        gradientMaterial.EnableKeyword("_gradientHeight");
    }
    
    private void Update()
    {
        if (gradientHeightValue != lastGradientHeightValue)
        {
            gradientMaterial.SetVector("_gradientHeight", new Vector2(0f, gradientHeightValue));
            lastGradientHeightValue = gradientHeightValue;
        }
    }
}
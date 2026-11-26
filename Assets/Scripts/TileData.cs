using UnityEngine;

public enum TileType
{
    Start,
    Normal,
    Trap,
    Treasure,
    Challenge,
    Temple
}

public class TileData : MonoBehaviour
{
    [SerializeField] private TileType tileType = TileType.Normal;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.55f);
    [SerializeField] private float pulseSpeed = 6f;

    public TileType Type => tileType;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private Renderer tileRenderer;
    private MaterialPropertyBlock block;
    private Color baseColor = Color.white;
    private bool highlighted;

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        if (tileRenderer != null && tileRenderer.sharedMaterial != null
            && tileRenderer.sharedMaterial.HasProperty(BaseColorId))
        {
            baseColor = tileRenderer.sharedMaterial.GetColor(BaseColorId);
        }
    }

    // Turns the pulsing light of the tile on or off
    public void SetHighlight(bool on)
    {
        highlighted = on;

        if (!on && tileRenderer != null)
            tileRenderer.SetPropertyBlock(null);
    }

    private void Update()
    {
        if (!highlighted || tileRenderer == null)
            return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        tileRenderer.GetPropertyBlock(block);
        block.SetColor(BaseColorId, Color.Lerp(baseColor, highlightColor, t));
        tileRenderer.SetPropertyBlock(block);
    }
}
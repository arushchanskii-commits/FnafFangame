using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class trail : MonoBehaviour
{
    [Header("Cable Settings")]
    public Transform startPoint;
    public float width = 0.1f;
    public Color color = Color.white;
    public Material material;

    void Reset()
    {
        if (material == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            material = new Material(shader);
            material.color = color;
        }
    }

    void Start()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, width);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        if (material != null)
        {
            lineRenderer.material = material;
        }
        else
        {
            Shader shader = Shader.Find("Sprites/Default");
            lineRenderer.material = new Material(shader) { color = color };
        }
    }

    void Update()
    {
        if (startPoint == null)
            return;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, transform.position);
    }
}

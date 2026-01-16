using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float width;
    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.alignment = LineAlignment.TransformZ;
        line.startWidth = width;
        line.endWidth = width;
        line.material.mainTextureScale = new Vector2(6, -1);
        line.SetPosition(1, target.position);
    }

    private void Update()
    {
        line.SetPosition(0, transform.position);
    }
}

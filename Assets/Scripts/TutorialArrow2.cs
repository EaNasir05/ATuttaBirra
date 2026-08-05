using UnityEngine;

public class TutorialArrow2 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float scalingDuration;
    [SerializeField] private float minScale;
    private Transform _t;
    private Transform arrowTransform;
    private float standardScale;
    private bool reducingScale;
    private Vector3 targetPosition;
    Vector3 modelForward = Vector3.back;
    
    void Awake()
    {
        _t = transform;
        arrowTransform = _t.GetChild(0);
        reducingScale = true;
        standardScale = arrowTransform.localScale.x;
        targetPosition = target.position;
    }

    void Update()
    {
        Vector3 direction = targetPosition - _t.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion correction = Quaternion.FromToRotation(modelForward, Vector3.forward);
        _t.rotation = targetRotation * Quaternion.Inverse(correction);
    }
}

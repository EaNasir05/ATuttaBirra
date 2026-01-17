using UnityEngine;

public class TutorialArrow2 : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float scalingDuration;
    [SerializeField] private float minScale;
    private Transform arrowTransform;
    private float standardScale;
    private bool reducingScale;
    private Vector3 targetPosition;
    Vector3 modelForward = Vector3.back;
    
    void Awake()
    {
        arrowTransform = transform.GetChild(0);
        reducingScale = true;
        standardScale = arrowTransform.localScale.x;
        targetPosition = target.position;
    }

    void Update()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        Quaternion correction = Quaternion.FromToRotation(modelForward, Vector3.forward);
        transform.rotation = targetRotation * Quaternion.Inverse(correction);
    }
}

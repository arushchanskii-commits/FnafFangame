using UnityEngine;

public class circleclicking : MonoBehaviour
{
    public GameObject targetObject;
    public int score = 0;
    public int hitScoreAmount = 1;
    public int missScoreAmount = -5000;

    private Collider2D circleCollider;
    private Collider2D targetCollider;
    private CirlcePath circlePath;

    void Start()
    {
        circleCollider = GetComponent<Collider2D>();
        if (circleCollider == null)
        {
            Debug.LogWarning("circleclicking requires a Collider2D component on the same GameObject.");
        }

        if (targetObject != null)
        {
            targetCollider = targetObject.GetComponent<Collider2D>();
            if (targetCollider == null)
            {
                Debug.LogWarning("Target object requires a Collider2D component.");
            }
        }
        else
        {
            Debug.LogWarning("Target object is not assigned in circleclicking.");
        }

        circlePath = GetComponent<CirlcePath>();
        if (circlePath == null)
        {
            Debug.LogWarning("circleclicking requires a CirlcePath component on the same GameObject to reverse speed.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (IsTouchingTarget())
            {
                score += hitScoreAmount;
                if (circlePath != null)
                {
                    circlePath.ReverseAndIncreaseSpeed();
                    circlePath.MoveTargetObjectToRandomPathPosition(targetObject != null ? targetObject.transform : null);
                    Debug.Log("Correct input: on target, rotation reversed, speed increased, target moved, score=" + score);
                }
                else
                {
                    Debug.Log("Correct input, but CirlcePath component is missing. score=" + score);
                }
            }
            else
            {
                score += missScoreAmount;
                Debug.Log("Pressed F but the circle is not touching the target object. score=" + score);
            }
        }
    }

    private bool IsTouchingTarget()
    {
        if (circleCollider == null || targetCollider == null)
        {
            return false;
        }

        ColliderDistance2D distanceInfo = circleCollider.Distance(targetCollider);
        return distanceInfo.isOverlapped || distanceInfo.distance <= 0f;
    }
}

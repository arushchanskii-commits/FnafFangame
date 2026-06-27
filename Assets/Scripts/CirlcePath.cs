
using UnityEngine;

public class CirlcePath : MonoBehaviour
{
    // Target object to rotate around
    public Transform target;
    public float rotationSpeed = 2f;
    public float radius = 1f;
    public float angle = 0f;
    public float targetMoveMinAngleDifference = 30f;
    public float speedIncreaseAmount = 0.5f;

    private float initialRotationSpeed;

    void Awake()
    {
        initialRotationSpeed = rotationSpeed;
    }

    void Update()
    {
        float x = target.position.x + Mathf.Cos(angle) * radius;
        float y = target.position.y + Mathf.Sin(angle) * radius;
        float z = target.position.z;

        transform.position = new Vector3(x, y, z);

        angle += rotationSpeed * Time.deltaTime;
    }

    public void MoveTargetObjectToRandomPathPosition(Transform targetObject)
    {
        if (targetObject == null || target == null)
        {
            return;
        }

        Vector3 center = target.position;
        float currentAngle = Mathf.Atan2(targetObject.position.y - center.y, targetObject.position.x - center.x);
        float newAngle = GetRandomPathAngle(currentAngle);

        targetObject.position = new Vector3(
            center.x + Mathf.Cos(newAngle) * radius,
            center.y + Mathf.Sin(newAngle) * radius,
            targetObject.position.z
        );
    }

    public void ReverseAndIncreaseSpeed()
    {
        float increase = Mathf.Abs(speedIncreaseAmount);
        rotationSpeed *= -1f;
        rotationSpeed += Mathf.Sign(rotationSpeed) * increase;
    }

    public void ResetSpeedToInitial()
    {
        rotationSpeed = initialRotationSpeed;
    }

    private float GetRandomPathAngle(float currentAngle)
    {
        float currentAngleDeg = currentAngle * Mathf.Rad2Deg;
        for (int i = 0; i < 10; i++)
        {
            float candidate = Random.Range(0f, Mathf.PI * 2f);
            float candidateDeg = candidate * Mathf.Rad2Deg;
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngleDeg, candidateDeg)) >= targetMoveMinAngleDifference)
            {
                return candidate;
            }
        }

        return currentAngle + targetMoveMinAngleDifference * Mathf.Deg2Rad;
    }
}

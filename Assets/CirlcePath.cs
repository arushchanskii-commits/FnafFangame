
using UnityEngine;

public class CirlcePath : MonoBehaviour
{
    // Target object to rotate around
    public Transform target;
    public float rotationSpeed = 2f;
    public float radius = 1f;
    public float angle = 0f;

    void Update()
    {
        float x = target.position.x + Mathf.Cos(angle) * radius;
        float y = target.position.y + Mathf.Sin(angle) * radius;
        float z = target.position.z;

        transform.position = new Vector3(x, y, z);

        angle += rotationSpeed * Time.deltaTime;
    }
}

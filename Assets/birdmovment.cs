using UnityEngine;

public class birdmovment : MonoBehaviour
{
    public float speed = 5;
    public float deathzone = -30;
    public LogicScript logic;
    [Tooltip("Scene object that counts as the valid collision target.")]
    public GameObject targetObject;

    [Tooltip("Optional tag to identify valid targets if target object is not set.")]
    public string targetTag = "Target";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * speed) * Time.deltaTime;
        
        if (transform.position.x < deathzone)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsValidTarget(collision.gameObject))
        {
            Debug.Log("Bird murder attempt success");
        }
    }

    private bool IsValidTarget(GameObject other)
    {
        if (other == null)
            return false;

        if (targetObject != null && other == targetObject)
            return true;

        if (!string.IsNullOrEmpty(targetTag) && other.CompareTag(targetTag))
            return true;

        return false;
    }
}

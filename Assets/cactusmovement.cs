using UnityEngine;

public class cactusmovement : MonoBehaviour
{
    public float speed = 5;
    public float deathzone = -30;
    [Tooltip("Scene object that counts as the valid collision target.")]
    public GameObject targetObject;

    [Tooltip("Optional tag to identify valid targets if target object is not set.")]
    public string targetTag = "Target";

    [Tooltip("Global scene name to load when this collision kills the target.")]
    public string deathSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NormalizeTransform();
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
            if (!string.IsNullOrWhiteSpace(deathSceneName))
            {
                Death.GlobalDeath(deathSceneName);
            }
            else
            {
                Death.death(collision.gameObject);
            }
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

    private void NormalizeTransform()
    {
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y),
            Mathf.Abs(transform.localScale.z));
    }
}


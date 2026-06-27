using UnityEngine;

public class HotwireEnding : MonoBehaviour
{
    [Tooltip("The specific GameObject or prefab asset that should trigger the next minigame when it touches this object.")]
    public GameObject targetObject;

    [Tooltip("If true, the collision with a child object of the target will also count as a hit.")]
    public bool targetChildrenAlsoMatch = true;

    private void OnTriggerEnter(Collider other)
    {
        TryAdvanceMiniGame(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryAdvanceMiniGame(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HotwireEnding: OnTriggerEnter2D detected with " + other.name);
        TryAdvanceMiniGame(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("HotwireEnding: OnCollisionEnter2D detected with " + collision.gameObject.name);
        TryAdvanceMiniGame(collision.gameObject);
    }

    private void TryAdvanceMiniGame(GameObject other)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("HotwireEnding: targetObject is not assigned.");
            return;
        }

        if (!IsTargetMatch(other))
        {
            Debug.Log("HotwireEnding: collision detected with non-target object: " + other.name);
            return;
        }

        Debug.Log("HotwireEnding: target object touched. Advancing minigame.");
        MiniGameSwapper.CompleteCurrentMiniGame();
    }

    private bool IsTargetMatch(GameObject other)
    {
        if (other == targetObject)
        {
            return true;
        }

        if (targetObject != null)
        {
            if (IsNameMatch(other.name, targetObject.name))
            {
                return true;
            }

            var root = other.transform.root.gameObject;
            if (root != null && IsNameMatch(root.name, targetObject.name))
            {
                return true;
            }
        }

        if (targetChildrenAlsoMatch && targetObject != null && targetObject.scene.IsValid() && other.transform.IsChildOf(targetObject.transform))
        {
            return true;
        }

        return false;
    }

    private bool IsNameMatch(string objectName, string targetName)
    {
        if (objectName == targetName)
        {
            return true;
        }

        if (objectName == targetName + "(Clone)")
        {
            return true;
        }

        if (objectName.StartsWith(targetName + " ("))
        {
            return true;
        }

        return false;
    }
}

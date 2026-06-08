using UnityEngine;

public class Death : MonoBehaviour
{
    [Header("Death Visual")]
    public GameObject deathImage;

    /// <summary>
    /// Trigger death behavior on the target object's Death component.
    /// Use this from any other script: Death.death(targetGameObject);
    /// </summary>
    public static void death(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("Death.death called with null target.");
            return;
        }

        var deathComponent = target.GetComponent<Death>();
        if (deathComponent != null)
        {
            deathComponent.TriggerDeath();
        }
        else
        {
            Debug.LogWarning($"No Death component found on {target.name}.");
        }
    }

    /// <summary>
    /// Instance-level death handler. Put the actual death logic here.
    /// </summary>
    public void TriggerDeath()
    {
        if (deathImage != null)
        {
            deathImage.SetActive(true);
        }

        Debug.Log($"{gameObject.name} died.");
        // Example: disable the GameObject or components until respawn
        // gameObject.SetActive(false);
    }
}

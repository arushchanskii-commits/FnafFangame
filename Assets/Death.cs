using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    private static Death instance;

    [Header("Death Visual")]
    public GameObject deathImage;

    [Header("Scene on Death")]
    [Tooltip("Optional scene name to load when this object dies.")]
    public string deathSceneName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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

        var deathComponent = FindDeathComponent(target);
        if (deathComponent != null)
        {
            deathComponent.TriggerDeath();
            return;
        }

        Debug.LogWarning($"Death.death: No Death component found on '{target.name}' or its parent/children.");
    }

    /// <summary>
    /// Trigger a global death event from any script, similar to the jumpscare trigger.
    /// Optionally pass a scene name to load immediately.
    /// </summary>
    public static void GlobalDeath(string sceneName = null)
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            GoToScene(sceneName);
            return;
        }

        if (instance != null)
        {
            instance.TriggerDeath();
            return;
        }

        Debug.LogWarning("Death.GlobalDeath called but no Death instance exists in the scene and no scene name was provided.");
    }

    private static Death FindDeathComponent(GameObject target)
    {
        var deathComponent = target.GetComponent<Death>();
        if (deathComponent != null)
        {
            return deathComponent;
        }

        deathComponent = target.GetComponentInParent<Death>();
        if (deathComponent != null)
        {
            return deathComponent;
        }

        return target.GetComponentInChildren<Death>();
    }

    /// <summary>
    /// Trigger a global death scene transition.
    /// Use this from any other script: Death.GoToScene("GameOverScene");
    /// </summary>
    public static void GoToScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Death.GoToScene called with an empty scene name.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"Death.GoToScene: Scene '{sceneName}' cannot be loaded. Add it to the Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneName);
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
        if (!string.IsNullOrWhiteSpace(deathSceneName))
        {
            GoToScene(deathSceneName);
        }
        // Example: disable the GameObject or components until respawn
        // gameObject.SetActive(false);
    }
}

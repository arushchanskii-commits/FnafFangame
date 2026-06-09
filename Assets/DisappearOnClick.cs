using UnityEngine;

public class DisappearOnClick : MonoBehaviour
{
    [Tooltip("Maximum time between clicks to count as a double-click.")]
    public float doubleClickTime = 0.3f;

    private float lastClickTime = 0f;

    private void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickTime)
        {
            gameObject.SetActive(false);
            MiniGameSwapper.CompleteCurrentMiniGame();
        }

        lastClickTime = Time.time;
    }
}

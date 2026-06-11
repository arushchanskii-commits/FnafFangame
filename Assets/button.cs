using UnityEngine;
using UnityEngine.SceneManagement;

public class button : MonoBehaviour
{
    // Advance to the next scene in the build settings
    public void AdvanceScene()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("button: no next scene in build settings.");
        }
    }

    // For desktop: clicking the object (requires a Collider)
    private void OnMouseDown()
    {
        AdvanceScene();
    }

    // For mobile: detect touch on this object's 2D collider
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended)
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(t.position);
                Vector2 touchPos = new Vector2(wp.x, wp.y);
                Collider2D hit = Physics2D.OverlapPoint(touchPos);
                if (hit != null && hit.gameObject == gameObject)
                {
                    AdvanceScene();
                }
            }
        }
    }
}

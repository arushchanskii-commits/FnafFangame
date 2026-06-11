using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AdvanceSceneOnClick : MonoBehaviour, IPointerClickHandler
{
    // Public method so it can be assigned to a UI Button OnClick
    public void AdvanceScene()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("AdvanceSceneOnClick: no next scene in build settings.");
        }
    }

    // Handle UI clicks (requires an EventSystem in the scene)
    public void OnPointerClick(PointerEventData eventData)
    {
        AdvanceScene();
    }

    // Handle non-UI clicks on GameObjects with Colliders
    private void OnMouseDown()
    {
        AdvanceScene();
    }
}

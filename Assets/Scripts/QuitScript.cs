using UnityEngine;

public class QuitScript : MonoBehaviour
{
    void OnMouseDown()
    {
        QuitGame();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

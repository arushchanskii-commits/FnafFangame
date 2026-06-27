using UnityEngine;

public class RestartButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        MiniGameSwapper.ResetToFirstScene();
    }
}

using UnityEngine;

public class GoToHotWire : MonoBehaviour
{
    private void OnMouseDown()
    {
        MiniGameSwapper.CompleteCurrentMiniGame();
    }

    public void GoToNextMiniGame()
    {
        MiniGameSwapper.CompleteCurrentMiniGame();
    }
}

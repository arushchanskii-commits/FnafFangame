using UnityEngine;

public class ContiniueButton : MonoBehaviour
{
    public void Continue()
    {
        MiniGameSwapper.ContinueFromDeathCheckpoint();
    }

    private void OnMouseDown()
    {
        Continue();
    }

    public void Refresh()
    {
        gameObject.SetActive(MiniGameSwapper.HasDeathCheckpoint);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

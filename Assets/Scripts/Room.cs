using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "FNaF/Room")]
public class Room : ScriptableObject
{
    public string roomName = "New Room";
    public int cameraIndex = -1; // -1 = no camera for this room
}

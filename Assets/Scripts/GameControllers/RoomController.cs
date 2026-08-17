using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] doors;

    public void UpdateRoom(bool[] status)  // enables and disables paths for each room 
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}

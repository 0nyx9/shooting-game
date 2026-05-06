using UnityEngine;

public class DoorTriggerHandler : MonoBehaviour
{
    public SlidingDoor doorScript; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorScript.SetPlayerNearby(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorScript.SetPlayerNearby(false);
        }
    }
}
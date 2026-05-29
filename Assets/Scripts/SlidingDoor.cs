using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorCube;  
    public float lowerAmount = 3f; 
    public float speed = 5f;       

    private Vector3 upPosition;
    private Vector3 downPosition;
    private bool isPlayerNear = false;

    void Start()
    {
        upPosition = doorCube.localPosition;
        
        downPosition = upPosition + new Vector3(0, -lowerAmount, 0);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            doorCube.localPosition = Vector3.MoveTowards(doorCube.localPosition, downPosition, speed * Time.deltaTime);
        }
        else
        {
            doorCube.localPosition = Vector3.MoveTowards(doorCube.localPosition, upPosition, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
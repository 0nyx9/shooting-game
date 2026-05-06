using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public float openSpeed = 2f;
    public float downDistance = 3f;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isPlayerNearby = false;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + (Vector3.down * downDistance);
    }

    void Update()
    {
        Vector3 targetPos = isPlayerNearby ? openPosition : closedPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * openSpeed);
    }

    public void SetPlayerNearby(bool nearby)
    {
        isPlayerNearby = nearby;
    }
}
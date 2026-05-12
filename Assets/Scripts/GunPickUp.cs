using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    public Material highlightMaterial;
    public GameObject gunPrefab; 
    public float reachDistance = 5f;

    private Material[] originalMaterials;
    private MeshRenderer[] meshRenderers;
    private bool isLookingAt = false;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
    }

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            // if (hit.collider.transform.root == transform.root)
            if (hit.collider.gameObject == gameObject || hit.transform.IsChildOf(transform))
            {
                if (!isLookingAt) ToggleHighlight(true);
                
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUp();
                }
            }
            else if (isLookingAt) ToggleHighlight(false);
        }
        else if (isLookingAt) ToggleHighlight(false);
    }

    void ToggleHighlight(bool on)
    {
        isLookingAt = on;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = on ? highlightMaterial : originalMaterials[i];
        }
    }

    void PickUp()
    {
        PlayerShooting player = FindFirstObjectByType<PlayerShooting>();
        
        if (player.gun != null) Destroy(player.gun.gameObject);

        GameObject newGun = Instantiate(gunPrefab, player.gunHolder);
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;

        player.gun = newGun.GetComponent<Gun>();
        Destroy(gameObject);
    }
}
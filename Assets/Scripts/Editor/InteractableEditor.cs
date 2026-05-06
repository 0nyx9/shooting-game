using UnityEditor;

public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        base.OnInspectorGUI();
        if (interactable.useEvent)
        {
            if (interactable.GetComponent<InteractionEvent>() == null)
            {
            interactable.gameObject.AddComponent<InteractionEvent>();
               
            }
            else
            {
                if (interactable.GetComponent<InteractionEvent>() != null)
                {
                    DestroyImmediate(interactable.GetComponent<InteractionEvent>());
                }
            }
        }
    }
}

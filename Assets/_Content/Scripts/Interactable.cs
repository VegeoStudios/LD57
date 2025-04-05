using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float InteractionDistance = 2f;
    public LayerMask InteractableLayer;

    private void OnEnable()
    {
        InteractionSystem.Instance.RegisterInteractable(this);
    }

    private void OnDisable()
    {
        InteractionSystem.Instance.DeregisterInteractable(this);
    }

    public void StartHover()
    {

    }

    public void StopHover()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}

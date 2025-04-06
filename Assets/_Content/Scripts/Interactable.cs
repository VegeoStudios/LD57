using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float InteractionDistance = 2f;

    [SerializeField]
    private GameObject _interactionUI;

    private void Awake()
    {
        if (_interactionUI == null)
        {
            Debug.LogError("Interaction UI is not assigned in the inspector.");
        }
        else
        {
            _interactionUI.SetActive(false);
        }
    }

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
        if (_interactionUI != null)
            _interactionUI.SetActive(false);
    }

    public virtual void Interact()
    {
        if (_interactionUI != null)
            _interactionUI.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}

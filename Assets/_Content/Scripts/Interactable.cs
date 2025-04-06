using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float InteractionDistance = 2f;

    [SerializeField] private GameObject _interactionUI;
    [SerializeField] private GameObject _hoverVisual;

    private Material _material;

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

        _material = GetComponent<Renderer>()?.material;
        _material?.SetFloat("_OutlineWidth", 0f);

        _hoverVisual?.SetActive(false);
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
        _material?.SetFloat("_OutlineWidth", 1f);
        _hoverVisual?.SetActive(true);
    }

    public void StopHover()
    {
        _interactionUI?.SetActive(false);
        _material?.SetFloat("_OutlineWidth", 0f);
        _hoverVisual?.SetActive(false);
    }

    public virtual void Interact()
    {
        _interactionUI?.SetActive(!_interactionUI.activeSelf);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}

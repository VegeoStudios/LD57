using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool Interacting => InteractionUI.activeSelf;
    public float InteractionDistance = 2f;
    public bool OverridePlayerInput = false;

    public GameObject InteractionUI;
    [SerializeField] private GameObject _hoverVisual;

    private Material _material;

    private void Awake()
    {
        if (InteractionUI) InteractionUI.SetActive(false);

        _material = GetComponent<Renderer>()?.material;
        if (_material) _material.SetFloat("_OutlineWidth", 0f);

        if (_hoverVisual) _hoverVisual.SetActive(false);
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
        if (_material) _material.SetFloat("_OutlineWidth", 1f);
        if (_hoverVisual) _hoverVisual.SetActive(true);
    }

    public void StopHover()
    {
        if (InteractionUI) InteractionUI.SetActive(false);
        if (_material) _material.SetFloat("_OutlineWidth", 0f);
        if (_hoverVisual) _hoverVisual.SetActive(false);
    }

    public virtual void Interact()
    {
        if (InteractionUI.activeSelf)
        {
            // Hide interaction UI
            InteractionUI.SetActive(false);

            if (OverridePlayerInput)
            {
                PlayerController.Instance.RemoveMovementRestrictor(this);
            }
        }
        else
        {
            // Show interaction UI
            InteractionUI.SetActive(true);

            if (OverridePlayerInput)
            {
                PlayerController.Instance.AddMovementRestrictor(this);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}

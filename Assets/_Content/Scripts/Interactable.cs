using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float InteractionDistance = 2f;

    [SerializeField] private GameObject _interactionUI;
    [SerializeField] private GameObject _hoverVisual;

    private Material _material;

    private void Awake()
    {
        if (_interactionUI) _interactionUI.SetActive(false);

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
        if (_interactionUI) _interactionUI.SetActive(false);
        if (_material) _material.SetFloat("_OutlineWidth", 0f);
        if (_hoverVisual) _hoverVisual.SetActive(false);
    }

    public virtual void Interact()
    {
        if (_interactionUI) _interactionUI.SetActive(!_interactionUI.activeSelf);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}

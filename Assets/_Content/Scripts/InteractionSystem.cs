using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    private static InteractionSystem _instance;
    public static InteractionSystem Instance => _instance ??= FindAnyObjectByType<InteractionSystem>();

    public Transform PlayerTransform { get; private set; }

    public List<Interactable> Interactables { get; private set; } = new List<Interactable>();
    public Interactable HoveredInteractable { get; private set; }

    private void Awake()
    {
        if (PlayerTransform == null)
        {
            PlayerTransform = GetComponentInChildren<PlayerController>().transform;
        }
    }

    private void FixedUpdate()
    {
        float closestSqrDistance = float.MaxValue;
        Interactable closestInteractable = null;
        foreach (var interactable in Interactables)
        {
            float distance = (PlayerTransform.position - interactable.transform.position).sqrMagnitude;
            if (distance < closestSqrDistance)
            {
                closestSqrDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null)
        {
            float interactionDistance = closestInteractable.InteractionDistance;
            if (closestInteractable == HoveredInteractable)
            {
                if (closestSqrDistance <= interactionDistance * interactionDistance)
                    return;
                else
                {
                    HoveredInteractable.StopHover();
                    HoveredInteractable = null;
                }
            }

            if (closestSqrDistance <= interactionDistance * interactionDistance)
            {
                HoveredInteractable?.StopHover();
                HoveredInteractable = closestInteractable;
                HoveredInteractable.StartHover();
            }
            else if (HoveredInteractable != null)
            {
                HoveredInteractable.StopHover();
                HoveredInteractable = null;
            }
        }
        else
        {
            HoveredInteractable?.StopHover();
            HoveredInteractable = null;
        }
    }

    public void OnInteract()
    {
        if (HoveredInteractable != null)
        {
            HoveredInteractable.Interact();
        }
    }

    public void RegisterInteractable(Interactable interactable)
    {
        if (!Interactables.Contains(interactable))
        {
            Interactables.Add(interactable);
        }
    }

    public void DeregisterInteractable(Interactable interactable)
    {
        if (Interactables.Contains(interactable))
        {
            Interactables.Remove(interactable);
        }
    }
}

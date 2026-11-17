using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    private GridPosition gridPosition;
    private Animator animator;
    private Action onInteractionComplete;
    private bool isActive;
    private float timer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            onInteractionComplete();
            isActive = false;
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;

        isActive = true;
        timer = .5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }


    private void OpenDoor()
    {
        isOpen = true;
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, isOpen);
        animator.SetBool("isOpen", isOpen);
    }

    private void CloseDoor()
    {
        isOpen = false;
        Pathfinding.Instance.SetWalkableGridPosition(gridPosition, isOpen);
        animator.SetBool("isOpen", isOpen);
    }
}

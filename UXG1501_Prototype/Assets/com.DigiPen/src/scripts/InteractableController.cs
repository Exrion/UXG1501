using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableController : MonoBehaviour
{
    public float m_InteractionDistance;
    [SerializeField]
    private Camera m_Camera;
    [SerializeField]
    private string m_InteractActionName;
    [SerializeField]
    private float m_OutlineUpdateInterval;
    [SerializeField]
    private float m_InteractionHoldTime = 1f;

    private InputAction m_InteractAction;
    private IInteractable m_CurrentInteractable;
    private IInteractable m_CurrentOutline;

    private void Start()
    {
        m_InteractAction = InputSystem.actions.FindAction(m_InteractActionName);
    }

    private void Update()
    {
        CheckInput();
        StartCoroutine(OutlineObject());
    }

    private IEnumerator OutlineObject()
    {
        if (!GameManager.Instance.m_ReadySceneSwitch)
        {
            int count = 0;
            RaycastHit[] raycastHits = Physics.RaycastAll(m_Camera.transform.position, m_Camera.transform.forward, m_InteractionDistance);
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    count++;
                    m_CurrentOutline = interactable;
                    break;
                }
            }
            if (count != 0)
                m_CurrentOutline.HandleOutline(true);
            else
            {
                if (m_CurrentOutline != null)
                {
                    m_CurrentOutline.HandleOutline(false);
                    m_CurrentOutline = null;
                }
            }
        }
        return new WaitForSecondsRealtime(m_OutlineUpdateInterval);
    }

    private void CheckInput()
    {
        if (m_InteractAction != null && m_InteractAction.phase == InputActionPhase.Started && m_CurrentInteractable == null)
            CheckInteract();
        else if (m_InteractAction != null && m_InteractAction.phase == InputActionPhase.Performed)
        {
            if (m_CurrentInteractable)
            {
                m_CurrentInteractable.HandleInteractionPerformed();
                m_CurrentInteractable = null;
            }
        }
        else if (m_InteractAction != null && m_InteractAction.phase == InputActionPhase.Waiting && m_CurrentInteractable != null)
        {
            if (m_CurrentInteractable)
            {
                m_CurrentInteractable.HandleInteractionCancelled();
                m_CurrentInteractable = null;
            }
        }
    }

    private void CheckInteract()
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(m_Camera.transform.position, m_Camera.transform.forward, m_InteractionDistance);
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                m_CurrentInteractable = interactable;
                m_CurrentInteractable.InjectSwivel(m_Camera, m_InteractionDistance);
                interactable.HandleInteractionStarted(m_InteractionHoldTime);
                break;
            }
        }
    }
}

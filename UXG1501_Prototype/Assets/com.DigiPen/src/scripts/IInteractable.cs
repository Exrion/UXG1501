using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider), typeof(Outline))]
public abstract class IInteractable : MonoBehaviour
{
    [SerializeField]
    private Interactable_Script m_InteractableScript;
    [SerializeField]
    private UIDocument m_InteractableDocument;
    [SerializeField]
    private float m_SwivelDistance = 1f;
    [SerializeField]
    private bool m_SwivelEnable;

    [SerializeField]
    [Tooltip("Only required if Disable HUD On Interact is checked.")]
    private UIDocument m_HUDDocument;
    [SerializeField]
    private bool m_disableHUDOnInteract;

    [SerializeField]
    [Tooltip("Only required if Disable FPS Controller On Interact is checked.")]
    private FirstPersonController m_FPSController;
    [SerializeField]
    private bool m_disableFPSControllerOnInteract;

    private Outline m_OutlineScript;
    private float m_InteractHoldTime;
    private float m_InteractHoldTimeCurrent;
    private bool m_InProgress;

    private Camera m_Camera;
    private Vector3 m_ChildOrigin;
    private float m_SwivelRayCastDistance;

    protected virtual void Start()
    {
        m_OutlineScript = GetComponent<Outline>();
        m_OutlineScript.enabled = false;

        m_ChildOrigin = m_OutlineScript.transform.position;

        if (m_InteractableScript == null)
            Logger.Log("Interactable_Script not found in child of gameobject!",
                Logger.SEVERITY_LEVEL.ERROR,
                Logger.LOGGER_OPTIONS.VERBOSE,
                MethodBase.GetCurrentMethod());
        if (m_InteractableDocument == null)
            Logger.Log("UIDocument not found in child of gameobject!",
                Logger.SEVERITY_LEVEL.ERROR,
                Logger.LOGGER_OPTIONS.VERBOSE,
                MethodBase.GetCurrentMethod());
    }

    protected virtual void Update()
    {
        // Progress Radial
        if (m_InProgress && m_InteractableScript != null)
        {
            // Set Progress
            m_InteractableScript.CalculateAndSetProgress(
                m_InteractHoldTimeCurrent += Time.deltaTime,
                m_InteractHoldTime);
            if (m_Camera != null && m_SwivelEnable)
                SwivelToCamera();
        }

        // Hide and Show Radial
        if (!m_InProgress && m_InteractableDocument != null)
            m_InteractableDocument.rootVisualElement.style.display = DisplayStyle.None;
        else if (m_InProgress && m_InteractableDocument != null)
            m_InteractableDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void InjectSwivel(Camera camera, float dist)
    {
        m_Camera = camera;
        m_SwivelRayCastDistance = dist;
    }
    public void ClearSwivel() => m_Camera = null;

    private void SwivelToCamera()
    {
        m_InteractableScript.transform.position = m_ChildOrigin;
        m_InteractableScript.transform.LookAt(m_Camera.transform);
        RaycastHit[] hits = Physics.RaycastAll(
            m_Camera.transform.position, 
            transform.position - m_Camera.transform.position, 
            m_SwivelRayCastDistance);
        foreach (RaycastHit hit in hits) 
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                float depthDist = Vector3.Distance(transform.position, m_Camera.transform.position) - hit.distance;
                m_InteractableScript.transform.position += m_InteractableScript.transform.forward * (m_SwivelDistance + depthDist);
                break;
            }
    }

    public virtual void HandleOutline(bool state)
    {
        m_OutlineScript.enabled = state;
    }

    public virtual void HandleInteractionStarted(float inputHoldTime)
    {
        m_InteractHoldTime = inputHoldTime;
        m_InProgress = true;
    }

    public void HandleInteractionPerformed()
    {
        ResetProgress();
        ClearSwivel();
        OnInteracted();
        if (m_disableHUDOnInteract && m_HUDDocument != null)
            ToggleHUD();
        if (m_disableFPSControllerOnInteract && m_FPSController != null)
            ToggleFPSController();
    }

    public virtual void HandleInteractionCancelled()
    {
        ResetProgress();
        ClearSwivel();
    }

    protected void ToggleHUD()
    {
        if (m_HUDDocument.rootVisualElement.style.display == DisplayStyle.None)
            m_HUDDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        else
            m_HUDDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    protected void ToggleFPSController()
    {
        m_FPSController.cameraCanMove = !m_FPSController.cameraCanMove;
        m_FPSController.playerCanMove = !m_FPSController.playerCanMove;
    }

    private void ResetProgress()
    {
        m_InProgress = false;
        m_InteractableScript.ResetProgress();
        m_InteractHoldTimeCurrent = 0f;
    }

    public abstract void OnInteracted();
}

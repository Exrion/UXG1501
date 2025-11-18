using UnityEngine;
using UnityEngine.Events;

class Interactable_Event : IInteractable
{
    public UnityEvent<INPUT_TYPE> OnInput;
    public INPUT_TYPE m_InputType;

    public override void OnInteracted()
    {
        OnInput?.Invoke(m_InputType);
    }
}
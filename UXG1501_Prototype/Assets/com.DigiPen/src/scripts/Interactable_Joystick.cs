using UnityEngine;
using UnityEngine.InputSystem;

class Interactable_Joystick : IInteractable
{
    private InputAction m_InputUp;
    private InputAction m_InputDown;
    private InputAction m_InputLeft;
    private InputAction m_InputRight;

    public Arcade_Script m_ArcadeScript;

    protected override void Start()
    {
        base.Start();
        m_InputUp = InputSystem.actions.FindAction("MoveUp");
        m_InputDown = InputSystem.actions.FindAction("MoveDown");
        m_InputLeft = InputSystem.actions.FindAction("MoveLeft");
        m_InputRight = InputSystem.actions.FindAction("MoveRight");
    }

    public override void OnInteracted()
    {
        // Nothing
    }

    public void ListenInput()
    {
        if (m_ArcadeScript == null) return;

        if (m_InputUp.triggered)
        {
            m_ArcadeScript.HandleInput(INPUT_TYPE.UP);
            return;
        }

        if (m_InputDown.triggered)
        {
            m_ArcadeScript.HandleInput(INPUT_TYPE.DOWN);
            return;
        }

        if (m_InputLeft.triggered)
        {
            m_ArcadeScript.HandleInput(INPUT_TYPE.LEFT);
            return;
        }

        if (m_InputRight.triggered)
        {
            m_ArcadeScript.HandleInput(INPUT_TYPE.RIGHT);
            return;
        }
    }
}

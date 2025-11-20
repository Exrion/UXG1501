using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Scriptables/Timer SO")]
public class Timer_SO : ScriptableObject
{
    public float m_StateTimer = 0f;
    public float m_StateTimerMax = -1f;
    public int m_TimeLeft;
}

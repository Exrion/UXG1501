using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class Interactable_Script : MonoBehaviour
{
    RadialProgress m_RadialProgress;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var progressCircle = root.Q<VisualElement>("ProgressCircle");

        m_RadialProgress = new RadialProgress()
        {
            style = {
                position = Position.Relative
            }
        };

        progressCircle.Add(m_RadialProgress);
    }

    public void ResetProgress() => m_RadialProgress.progress = 0f;
    public void CalculateAndSetProgress(float current, float maximum) => 
        m_RadialProgress.progress = Mathf.Clamp(current / maximum * 100, 0, 100);
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUD_Script : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument hud = GetComponent<UIDocument>();
        VisualElement root = hud.rootVisualElement;
    }
}

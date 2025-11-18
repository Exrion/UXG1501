using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum INPUT_TYPE
{
    ALL = 0x00,
    CONFIRM = 0x01,
    BACK = 0x02,
    UP = 0x03,
    DOWN = 0x04,
    LEFT = 0x05,
    RIGHT = 0x06,
    SKIP = 0x07
}

public class Arcade_Script : MonoBehaviour
{
    enum STATE
    {
        NONE = 0x00,
        START = 0x01,
        MODE_SELECT = 0x02,
        PICK_SIDE = 0x03,
        TUTORIAL1 = 0x04,
        TUTORIAL2 = 0x05,
        CHARACTER_SELECT = 0x06
    }

    UIDocument m_UIDocument;
    VisualElement m_RootVisualElement;
    VisualElement m_Container;

    bool m_LocaleEnglish = true;
    public Locale_SO m_Locales;

    VisualElement m_StartOverlay;

    [SerializeField]
    List<string> m_VisualTreeAssetPaths = new();
    [SerializeField]
    List<STATE> m_ScreenStateOrder = new List<STATE>();

    Dictionary<STATE, TemplateContainer> m_TemplateContainers = new();
    STATE m_State = STATE.START;

    // Mode Select
    // [HERE]

    // Pick Side
    bool m_PickSide_Left = true;

    void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_RootVisualElement = m_UIDocument.rootVisualElement;
        m_Container = m_RootVisualElement.Q("Container");
        m_StartOverlay = m_RootVisualElement.Q("Screen_Start");

        InitTemplateContainers();
    }

    void Update()
    {

    }

    public void HandleInput(INPUT_TYPE input)
    {
        switch (m_State)
        {
            case STATE.NONE:
                Logger.Log("None State Achieved. Issue is likely with your ScreenStateOrder.",
                    Logger.SEVERITY_LEVEL.WARNING,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
                break;
            case STATE.START:
                // Nothing
                break;
            case STATE.MODE_SELECT:
                // @TODO: TEMP SOLUTION
                if (input == INPUT_TYPE.CONFIRM)
                    ChangeScreenState(++m_State);
                else if (input == INPUT_TYPE.BACK)
                    ChangeScreenState(--m_State);
                break;

            case STATE.PICK_SIDE:
                Update_PickSide();
                if (input == INPUT_TYPE.LEFT)
                {
                    m_PickSide_Left = true;
                    Update_PickSide();
                }
                else if (input == INPUT_TYPE.RIGHT)
                {
                    m_PickSide_Left = false;
                    Update_PickSide();
                }
                if (input == INPUT_TYPE.CONFIRM)
                    ChangeScreenState(++m_State);
                else if (input == INPUT_TYPE.BACK)
                    ChangeScreenState(--m_State);
                break;
            case STATE.TUTORIAL1:
                if (input == INPUT_TYPE.SKIP)
                    ChangeScreenState(++m_State);
                break;
            case STATE.TUTORIAL2:
                if (input == INPUT_TYPE.SKIP)
                    ChangeScreenState(++m_State);
                break;
            case STATE.CHARACTER_SELECT:
                if (input == INPUT_TYPE.CONFIRM)
                {
                    m_State = STATE.START;
                    m_StartOverlay.RemoveFromClassList("transparent");
                    ChangeScreenState(m_State);
                }
                break;
            default:
                Logger.Log("Unknown State Achieved. We should never be here.",
                    Logger.SEVERITY_LEVEL.WARNING,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
                break;
        }
    }

    void Update_PickSide()
    {
        if (m_PickSide_Left)
        {
            m_Container.Q("image-left").RemoveFromClassList("fighter-image-disabled");
            m_Container.Q("image-right").AddToClassList("fighter-image-disabled");
            m_Container.Q("image-base-left").RemoveFromClassList("fighter-disabled");
            m_Container.Q("image-base-right").AddToClassList("fighter-disabled");
        }
        else
        {
            m_Container.Q("image-left").AddToClassList("fighter-image-disabled");
            m_Container.Q("image-right").RemoveFromClassList("fighter-image-disabled");
            m_Container.Q("image-base-left").AddToClassList("fighter-disabled");
            m_Container.Q("image-base-right").RemoveFromClassList("fighter-disabled");
        }
    }

    public void HandleCardSwipe()
    {
        if (m_State == STATE.START)
        {
            m_StartOverlay.AddToClassList("transparent");
            m_State = STATE.MODE_SELECT;
            ChangeScreenState(m_State);
        }
    }

    public void HandleLocaleChange()
    {
        m_LocaleEnglish = !m_LocaleEnglish;
        UpdateLocale();
    }

    void ChangeScreenState(STATE state)
    {
        m_Container.Clear();

        if (state == STATE.START) return;

        if (m_TemplateContainers.TryGetValue(state, out TemplateContainer template))
        {
            template.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            template.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            m_Container.Add(template);
            UpdateLocale();
        }
        else
            Logger.Log("Could not find STATE matching type \"" + state.ToString() + "\"!",
                    Logger.SEVERITY_LEVEL.ERROR,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
    }

    void UpdateLocale()
    {
        List<VisualElement> localeLabels = m_Container.Query(className: "locale").ToList();
        for (int i = 0; i < localeLabels.Count; i++)
        {
            string localeName = localeLabels[i].name;
            Label label = (Label)localeLabels[i];
            label.text = Locale_SO.GetValue(m_LocaleEnglish ? m_Locales.locale_en : m_Locales.locale_jp, localeName);
        }
    }

    void InitTemplateContainers()
    {
        for (int i = 0; i < m_VisualTreeAssetPaths.Count; i++)
        {
            VisualTreeAsset va = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(m_VisualTreeAssetPaths[i]);
            if (va != null)
            {
                m_TemplateContainers.Add(m_ScreenStateOrder[i < m_ScreenStateOrder.Count ? i : 0], va.CloneTree());
            }
            else
                Logger.Log("Could not find item at path \"" + m_VisualTreeAssetPaths[i] + "\"!",
                    Logger.SEVERITY_LEVEL.ERROR,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
        }
    }
}

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
    List<VisualTreeAsset> m_VisualTreeAssets = new();
    [SerializeField]
    List<STATE> m_ScreenStateOrder = new List<STATE>();

    Dictionary<STATE, TemplateContainer> m_TemplateContainers = new();
    STATE m_State = STATE.START;

    [SerializeField]
    Timer_SO m_TimerSO;
    [SerializeField]
    UDictionary<STATE, float> m_StateTimerMaxDict = new();

    // Mode Select
    int m_ModeSelect_Count = 0;

    // Pick Side
    bool m_PickSide_Left = true;

    private void Start()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_RootVisualElement = m_UIDocument.rootVisualElement;
        m_Container = m_RootVisualElement.Q("Container");
        m_StartOverlay = m_RootVisualElement.Q("Screen_Start");

        InitTemplateContainers();
    }

    private void Update()
    {
        UpdateTimer();
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
                if (input == INPUT_TYPE.LEFT)
                    m_ModeSelect_Count = m_ModeSelect_Count <= 0 ? 4 : m_ModeSelect_Count - 1;
                else if (input == INPUT_TYPE.RIGHT)
                    m_ModeSelect_Count = m_ModeSelect_Count >= 4 ? 0 : m_ModeSelect_Count + 1;

                Update_ModeSelect();
                UpdateLocale();

                if (input == INPUT_TYPE.CONFIRM)
                    ChangeScreenState(++m_State);
                break;

            case STATE.PICK_SIDE:
                if (input == INPUT_TYPE.LEFT)
                    m_PickSide_Left = true;
                else if (input == INPUT_TYPE.RIGHT)
                    m_PickSide_Left = false;

                Update_PickSide();
                UpdateLocale();

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

    private void UpdateTimer()
    {
        if (m_TimerSO.m_StateTimerMax == -1f) return;
        m_TimerSO.m_StateTimer += Time.deltaTime;
        m_TimerSO.m_TimeLeft = Mathf.RoundToInt(m_TimerSO.m_StateTimerMax - m_TimerSO.m_StateTimer);
        if (m_TimerSO.m_TimeLeft <= 0)
            ChangeScreenState(++m_State);
    }

    private void Update_ModeSelect()
    {
        switch (m_ModeSelect_Count)
        {
            case 0:
                // Local
                m_Container.Q("mode-coop").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-coop").RemoveFromClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-coop").RemoveFromClassList("mode-select-image-local-disabled");
                m_Container.Q("locale_coop").RemoveFromClassList("mode-select-text-disabled");

                // Online
                m_Container.Q("mode-online").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-online").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-online").AddToClassList("mode-select-image-online-disabled");
                m_Container.Q("locale_online").AddToClassList("mode-select-text-disabled");

                // Friend
                m_Container.Q("mode-friend").style.display = DisplayStyle.None;

                // Ranked
                m_Container.Q("mode-ranked").style.display = DisplayStyle.None;

                // Practise
                m_Container.Q("mode-practise").style.display = DisplayStyle.None;

                // FrontPad
                m_Container.Q("mode-pad-front").style.display = DisplayStyle.Flex;

                // BackPad
                m_Container.Q("mode-pad-back").style.display = DisplayStyle.None;
                break;
            case 1:
                // Local
                m_Container.Q("mode-coop").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-coop").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-coop").AddToClassList("mode-select-image-local-disabled");
                m_Container.Q("locale_coop").AddToClassList("mode-select-text-disabled");

                // Online
                m_Container.Q("mode-online").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-online").RemoveFromClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-online").RemoveFromClassList("mode-select-image-online-disabled");
                m_Container.Q("locale_online").RemoveFromClassList("mode-select-text-disabled");

                // Friend
                m_Container.Q("mode-friend").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-friend").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-friend").AddToClassList("mode-select-image-friend-disabled");
                m_Container.Q("locale_friend").AddToClassList("mode-select-text-disabled");

                // Ranked
                m_Container.Q("mode-ranked").style.display = DisplayStyle.None;

                // Practise
                m_Container.Q("mode-practise").style.display = DisplayStyle.None;

                // FrontPad
                m_Container.Q("mode-pad-front").style.display = DisplayStyle.None;

                // BackPad
                m_Container.Q("mode-pad-back").style.display = DisplayStyle.None;
                break;
            case 2:
                // Local
                m_Container.Q("mode-coop").style.display = DisplayStyle.None;

                // Online
                m_Container.Q("mode-online").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-online").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-online").AddToClassList("mode-select-image-online-disabled");
                m_Container.Q("locale_online").AddToClassList("mode-select-text-disabled");

                // Friend
                m_Container.Q("mode-friend").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-friend").RemoveFromClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-friend").RemoveFromClassList("mode-select-image-friend-disabled");
                m_Container.Q("locale_friend").RemoveFromClassList("mode-select-text-disabled");

                // Ranked
                m_Container.Q("mode-ranked").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-ranked").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-ranked").AddToClassList("mode-select-image-ranked-disabled");
                m_Container.Q("locale_ranked").AddToClassList("mode-select-text-disabled");

                // Practise
                m_Container.Q("mode-practise").style.display = DisplayStyle.None;

                // FrontPad
                m_Container.Q("mode-pad-front").style.display = DisplayStyle.None;

                // BackPad
                m_Container.Q("mode-pad-back").style.display = DisplayStyle.None;
                break;
            case 3:
                // Local
                m_Container.Q("mode-coop").style.display = DisplayStyle.None;

                // Online
                m_Container.Q("mode-online").style.display = DisplayStyle.None;

                // Friend
                m_Container.Q("mode-friend").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-friend").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-friend").AddToClassList("mode-select-image-friend-disabled");
                m_Container.Q("locale_friend").AddToClassList("mode-select-text-disabled");

                // Ranked
                m_Container.Q("mode-ranked").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-ranked").RemoveFromClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-ranked").RemoveFromClassList("mode-select-image-ranked-disabled");
                m_Container.Q("locale_ranked").RemoveFromClassList("mode-select-text-disabled");

                // Practise
                m_Container.Q("mode-practise").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-practise").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-practise").AddToClassList("mode-select-image-practise-disabled");
                m_Container.Q("locale_practise").AddToClassList("mode-select-text-disabled");

                // FrontPad
                m_Container.Q("mode-pad-front").style.display = DisplayStyle.None;

                // BackPad
                m_Container.Q("mode-pad-back").style.display = DisplayStyle.None;
                break;
            case 4:
                // Local
                m_Container.Q("mode-coop").style.display = DisplayStyle.None;

                // Online
                m_Container.Q("mode-online").style.display = DisplayStyle.None;

                // Friend
                m_Container.Q("mode-friend").style.display = DisplayStyle.None;

                // Ranked
                m_Container.Q("mode-ranked").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-ranked").AddToClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-ranked").AddToClassList("mode-select-image-ranked-disabled");
                m_Container.Q("locale_ranked").AddToClassList("mode-select-text-disabled");

                // Practise
                m_Container.Q("mode-practise").style.display = DisplayStyle.Flex;
                m_Container.Q("mode-practise").RemoveFromClassList("mode-select-base-disabled");
                m_Container.Q("mode-image-practise").RemoveFromClassList("mode-select-image-practise-disabled");
                m_Container.Q("locale_practise").RemoveFromClassList("mode-select-text-disabled");

                // FrontPad
                m_Container.Q("mode-pad-front").style.display = DisplayStyle.None;

                // BackPad
                m_Container.Q("mode-pad-back").style.display = DisplayStyle.Flex;
                break;
            default:
                Logger.Log("Unknown Mode Select Achieved. We should never be here.",
                    Logger.SEVERITY_LEVEL.WARNING,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
                break;
        }
    }

    private void Update_PickSide()
    {
        if (m_PickSide_Left)
        {
            m_Container.Q("image-left").RemoveFromClassList("fighter-image-disabled");
            m_Container.Q("image-right").AddToClassList("fighter-image-disabled");
            m_Container.Q("image-base-left").RemoveFromClassList("fighter-disabled");
            m_Container.Q("image-base-right").AddToClassList("fighter-disabled");
            m_Container.Q("element-arrow").RemoveFromClassList("element-arrow-flip");
        }
        else
        {
            m_Container.Q("image-left").AddToClassList("fighter-image-disabled");
            m_Container.Q("image-right").RemoveFromClassList("fighter-image-disabled");
            m_Container.Q("image-base-left").AddToClassList("fighter-disabled");
            m_Container.Q("image-base-right").RemoveFromClassList("fighter-disabled");
            m_Container.Q("element-arrow").AddToClassList("element-arrow-flip");
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

    private void ChangeScreenState(STATE state)
    {
        m_Container.Clear();

        if (state == STATE.START) return;

        if (m_StateTimerMaxDict.TryGetValue(state, out float timerMax))
            m_TimerSO.m_StateTimerMax = timerMax;
        else
            m_TimerSO.m_StateTimerMax = -1f;
        m_TimerSO.m_StateTimer = 0f;

        if (m_TemplateContainers.TryGetValue(state, out TemplateContainer template))
        {
            template.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            template.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            m_Container.Add(template);
            UpdateLocale();
        }
        else
            Logger.Log("Could not find STATE matching type \"" + state.ToString() + "\" for Containers!",
                    Logger.SEVERITY_LEVEL.ERROR,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
    }

    private void UpdateLocale()
    {
        List<VisualElement> localeLabels = m_Container.Query(className: "locale").ToList();
        for (int i = 0; i < localeLabels.Count; i++)
        {
            string localeName = localeLabels[i].name;
            Label label = (Label)localeLabels[i];
            if (localeName == "locale_subtitle")
                label.text = Locale_SO.GetValue(m_LocaleEnglish ? m_Locales.locale_en : m_Locales.locale_jp, localeName + m_ModeSelect_Count);
            else
                label.text = Locale_SO.GetValue(m_LocaleEnglish ? m_Locales.locale_en : m_Locales.locale_jp, localeName);
        }
    }

    private void InitTemplateContainers()
    {
        for (int i = 0; i < m_VisualTreeAssets.Count; i++)
        {
            if (m_VisualTreeAssets[i] != null)
            {
                m_TemplateContainers.Add(m_ScreenStateOrder[i < m_ScreenStateOrder.Count ? i : 0], m_VisualTreeAssets[i].CloneTree());
            }
            else
                Logger.Log("Could not find Visual Tree Asset \"" + m_VisualTreeAssets[i] + "\"!",
                    Logger.SEVERITY_LEVEL.ERROR,
                    Logger.LOGGER_OPTIONS.VERBOSE,
                    MethodBase.GetCurrentMethod());
        }
    }
}

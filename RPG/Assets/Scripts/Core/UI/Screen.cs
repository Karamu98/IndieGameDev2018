using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core.UI
{
    public abstract class Screen : MonoBehaviour
    {
        public abstract string ScreenName {get;}

        void Awake()
        {
            ScreenManager.Get().TrackScreen(ScreenName, this);
            Init();
        }

        protected virtual void Init()
        {

        }

        void OnDestroy()
        {
            ScreenManager.Get().StopTrackingScreen(ScreenName);
        }

        public void SetScreen(string screenToSet)
        {
            ScreenManager.Get().SetScreen(screenToSet);
        }
    }

    public class ScreenManager
    {
        static ScreenManager s_instance;
        public static ScreenManager Get()
        {
            if(s_instance == null)
            {
                s_instance = new ScreenManager();
                s_instance.m_screens = new Dictionary<string, Screen>();
                s_instance.m_activeScreens = new List<string>();
            }

            return s_instance;
        }

        Dictionary<string, Screen> m_screens;
        List<string> m_activeScreens;

        public void SetScreen(string screenToSet)
        {
            foreach(string scrn in m_activeScreens)
            {
                m_screens[scrn].gameObject.SetActive(false);
            }

            m_screens[screenToSet].gameObject.SetActive(true);
            m_activeScreens.Add(screenToSet);
        }

        public void EnableScreen(string screenToEnable)
        {
            m_screens[screenToEnable].gameObject.SetActive(true);
            m_activeScreens.Add(screenToEnable);
        }

        public void TrackScreen(string screenName, Screen newScreen)
        {
            m_screens[screenName] = newScreen;
            m_activeScreens.Add(screenName);
        }

        public void StopTrackingScreen(string screenName)
        {
            if(m_activeScreens.Contains(screenName))
            {
                m_activeScreens.Remove(screenName);
            }
            m_screens.Remove(screenName);
        }
    }
}


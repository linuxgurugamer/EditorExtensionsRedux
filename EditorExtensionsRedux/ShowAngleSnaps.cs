using KSP.Localization;
using System;
using UnityEngine;

using ClickThroughFix;

namespace EditorExtensionsRedux
{
    public class ShowAngleSnaps : MonoBehaviour
    {
        public delegate void WindowDisabledEventHandler();

        public event WindowDisabledEventHandler WindowDisabled;

        protected virtual void OnWindowDisabled()
        {
            if (WindowDisabled != null)
                WindowDisabled();
        }

        ConfigData _config;


        string _windowTitle = string.Empty;
        string _version = string.Empty;

        Rect _windowRect = new Rect()
        {
            xMin = Screen.width - 325,
            xMax = Screen.width - 275,
            yMin = 50,
            yMax = 50 //0 height, GUILayout resizes it
        };

        public bool isVisible()
        {
            return this.enabled;
        }

        void Awake()
        {
            //start disabled
            this.enabled = false;
            Log.Debug("ShowAngleSnaps Awake()");
        }

        void OnEnable()
        {
            Log.Debug("ShowAngleSnaps OnEnable()");

            if (_config == null)
            {
                this.enabled = false;
            }
        }

        void CloseWindow()
        {
            this.enabled = false;
            OnWindowDisabled();
        }

        //void OnDisable ()
        //{
        //}

        void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                //_windowRect.yMax = _windowRect.yMin;
                _windowRect = ClickThruBlocker.GUILayoutWindow(this.GetInstanceID(), _windowRect, WindowContent, Localizer.Format("#LOC_EEX_146"));
            }
        }

        //void OnDestroy ()
        //{
        //}

        /// <summary>
        /// Initializes the window content and enables it
        /// </summary>
        public void Show(ConfigData config)
        {
            Log.Debug("ShowAngleSnaps Show()");
            _config = config;

            this.enabled = true;
        }

        public void Hide()
        {
            CloseWindow();
        }

        // private string[] _toolbarStrings = { "Settings 1", "Settings 2", "Angle Snap" };
        string keyMapToUpdate = string.Empty;
        string newAngleString = string.Empty;
        public int angleGridIndex = -1;
        public string[] angleStrings = new string[] { string.Empty };
        object anglesLock = new object();
        GUILayoutOption[] settingsLabelLayout = new GUILayoutOption[] { GUILayout.MinWidth(150) };

        void WindowContent(int windowID)
        {
            GUILayout.BeginVertical(Localizer.Format("#LOC_EEX_66"));

            #region angle snap values settings

            try
            {
                foreach (float a in _config.AngleSnapValues)
                {
                    if (a != 0.0f)
                    {
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(a.ToString()))
                        {
                            EditorLogic.fetch.srfAttachAngleSnap = a;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

            }
#if DEBUG
            catch (Exception ex)
            {
                //potential for some intermittent locking/threading issues here	
                //Debug only to avoid log spam
                Log.Error("Error updating AngleSnapValues: " + ex.Message);
            }
#else
				catch(Exception){
					//just ignore the error and continue since it's non-critical
				}
#endif


            #endregion

            GUILayout.EndVertical();//end main content

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Localizer.Format("#LOC_EEX_51")))
            {
                //reload config to reset any unsaved changes?
                //_config = ConfigManager.LoadConfig (_configFilePath);
                CloseWindow();
            }
#if false
            if (GUILayout.Button(Localizer.Format("#LOC_EEX_142")))
            {
                _config = ConfigManager.CreateDefaultConfig(_configFilePath, _version);
            }

            if (GUILayout.Button(Localizer.Format("#LOC_EEX_145")))
            {
                ConfigManager.SaveConfig(_config, _configFilePath);
                CloseWindow();
            }
#endif
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

    }
}


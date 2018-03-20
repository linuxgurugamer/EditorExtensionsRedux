using System;
using UnityEngine;
using KSP.UI.Screens;

namespace EditorExtensionsRedux
{
	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
	public class AppLauncherButton : MonoBehaviour
	{
		private ApplicationLauncherButton button = null;

		public static AppLauncherButton Instance;

		const string texPathDefault = "EditorExtensionsRedux/Textures/AppLauncherIcon";
		const string texPathOn = "EditorExtensionsRedux/Textures/AppLauncherIcon-On";
		const string texPathOff = "EditorExtensionsRedux/Textures/AppLauncherIcon-Off";

		private void Start ()
		{
			if (button == null) {
				OnGuiAppLauncherReady ();
			}
		}

		private void Awake ()
		{
			if (AppLauncherButton.Instance == null) {
				GameEvents.onGUIApplicationLauncherReady.Add (this.OnGuiAppLauncherReady);
				Instance = this;
			}
		}

		private void OnDestroy ()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove (this.OnGuiAppLauncherReady);
			if (this.button != null) {
				ApplicationLauncher.Instance.RemoveModApplication (this.button);
			}
		}

		private void ButtonState (bool state)
		{
			Log.Debug ("ApplicationLauncher on" + state.ToString ());
			EditorExtensions.Instance.Visible = state;
		}
        /// <summary>
        /// Copied from ToolbarController
        /// </summary>
        /// 
              //
        // The following function was initially copied from @JPLRepo's AmpYear mod, which is covered by the GPL, as is this mod
        //
        // This function will attempt to load either a PNG or a JPG from the specified path.  
        // It first checks to see if the actual file is there, if not, it then looks for either a PNG or a JPG
        //
        // easier to specify different cases than to change case to lower.  This will fail on MacOS and Linux
        // if a suffix has mixed case
        static string[] imgSuffixes = new string[] { ".png", ".jpg", ".gif", ".PNG", ".JPG", ".GIF" };
        public static Boolean LoadImageFromFile(ref Texture2D tex, String fileNamePath)
        {

            Boolean blnReturn = false;
            try
            {
                string path = fileNamePath;
                if (!System.IO.File.Exists(fileNamePath))
                {
                    // Look for the file with an appended suffix.
                    for (int i = 0; i < imgSuffixes.Length; i++)

                        if (System.IO.File.Exists(fileNamePath + imgSuffixes[i]))
                        {
                            path = fileNamePath + imgSuffixes[i];
                            break;
                        }
                }

                //File Exists check
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        tex.LoadImage(System.IO.File.ReadAllBytes(path));
                        blnReturn = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to load the texture:" + path);
                        Log.Error(ex.Message);
                    }
                }
                else
                {
                    Log.Error("Cannot find texture to load:" + fileNamePath);
                }


            }
            catch (Exception ex)
            {
                Log.Error("Failed to load (are you missing a file):" + fileNamePath);
                Log.Error(ex.Message);
            }
            return blnReturn;
        }

        Texture2D GetTexture(string path, bool b)
        {

            Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);

            if (LoadImageFromFile(ref tex, KSPUtil.ApplicationRootPath + "GameData/" + path))
                return tex;
            return tex;
        }
/// <summary>
/// End of copy
/// </summary>

        private void OnGuiAppLauncherReady ()
		{
			if (this.button == null) {
				try {
					this.button = ApplicationLauncher.Instance.AddModApplication (
						() => {
							EditorExtensions.Instance.Show ();
						}, 	//RUIToggleButton.onTrue
						() => {
							EditorExtensions.Instance.Hide ();
						},	//RUIToggleButton.onFalse
						() => {
							EditorExtensions.Instance.ShowMenu ();
						}, //RUIToggleButton.OnHover
						() => {
							EditorExtensions.Instance.HideMenu ();
						}, //RUIToggleButton.onHoverOut
						null, //RUIToggleButton.onEnable
						null, //RUIToggleButton.onDisable
						ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, //visibleInScenes
						GetTexture (texPathDefault, false) //texture
					);
					Log.Debug ("Added ApplicationLauncher button");
				} catch (Exception ex) {
					Log.Error ("Error adding ApplicationLauncher button: " + ex.Message);
				}
			}

		}

		private void Update ()
		{
			if (this.button == null) {
				return;
			}

//			if(this.button.State != RUIToggleButton.ButtonState.TRUE)
//				this.button.SetTexture(GameDatabase.Instance.GetTexture (texPathOn, false));
//			else
//				this.button.SetTexture(GameDatabase.Instance.GetTexture (texPathOff, false));

			try {
				if (EditorLogic.fetch != null) {
//					if (EditorExtensions.Instance.Visible && this.button.State != RUIToggleButton.ButtonState.TRUE) {
					if (EditorExtensions.Instance.Visible && !this.button.enabled) {
						this.button.SetTrue ();
						//this.button.SetTexture(GameDatabase.Instance.GetTexture (texPathOn, false));
//					} else if (!EditorExtensions.Instance.Visible && this.button.State != RUIToggleButton.ButtonState.FALSE) {
					} else if (!EditorExtensions.Instance.Visible && this.button.enabled) {
						this.button.SetFalse ();
						//this.button.SetTexture(GameDatabase.Instance.GetTexture (texPathOff, false));
					}
//				} else if (this.button.State != RUIToggleButton.ButtonState.DISABLED) {
				} else if (this.button.enabled) {
					this.button.Disable ();
				}
			} catch (Exception ex) {
				Log.Error ("Error updating ApplicationLauncher button: " + ex.Message);
			}
		}
	}
}
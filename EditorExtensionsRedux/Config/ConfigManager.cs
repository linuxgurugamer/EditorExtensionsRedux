using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace EditorExtensionsRedux
{
	public static class ConfigManager
	{
		public static bool FileExists (string filePath)
		{
			try {
				FileInfo file = new FileInfo (filePath);
				return file.Exists;
			} catch (Exception ex) {
				Log.Error ("Failed to verify file " + filePath + " Error: " + ex.Message);
				return false;
			}
		}

		public static bool SaveConfig (ConfigData configData, string configFilePath)
		{
			try {
				XmlSerializer serializer = new XmlSerializer (typeof(ConfigData));
				using (TextWriter writer = new StreamWriter (configFilePath)) {
					serializer.Serialize (writer, configData); 
				}
				Log.Debug ("Saved config file");
				return true;
			} catch (Exception ex) {
				Log.Error ("Error saving config file: " + ex.Message);
				return false;
			}
		}

		public static ConfigData LoadConfig (string configFilePath)
		{
			try {
				XmlSerializer deserializer = new XmlSerializer (typeof(ConfigData));

				ConfigData data;
				using (TextReader reader = new StreamReader (configFilePath)) {
					object obj = deserializer.Deserialize (reader);
					data = (ConfigData)obj;
				}

				//need to verify that there are no missing fields

				return data;
			} catch (Exception ex) {
				Log.Error ("Error loading config file: " + ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Creates a new config file with defaults
		/// will replace any existing file
		/// </summary>
		/// <returns>New config object with default settings</returns>
		public static ConfigData CreateDefaultConfig (string configFilePath, string version)
		{
			try {
                ConfigData defaultConfig = new ConfigData() {
                    AngleSnapValues = new List<float> { 0.0f, 1.0f, 5.0f, 15.0f, 22.5f, 30.0f, 45.0f, 60.0f, 90.0f },
                    MaxSymmetry = 20,
                    // Rapidzoom by Fwiffo 
                    RapidZoom = true,
                    //ZoomCycling = true,
                    //ZoomCycleDistances = new List<float> { 0f, 1f, 5f, 30f, 100f, 500f, 10000f, 100000f, 150000f }, // RKTODO: Set good defaults
                    FileVersion = version,
                    OnScreenMessageTime = 1.5f,
                    ShowDebugInfo = true,
                    ReRootEnabled = true,
                    NoOffsetLimitEnabled = true,
                    FineAdjustEnabled = true,
                    AnglesnapModIsToggle = true,
                    CycleSymmetryModeModIsToggle = true
				};

				KeyMaps defaultKeys = new KeyMaps () {
					AttachmentMode = KeyCode.T,
					PartClipping = KeyCode.Z,
					ResetCamera = KeyCode.Space,
					ZoomSelected = KeyCode.KeypadPeriod,
					VerticalSnap = KeyCode.V,
					HorizontalSnap = KeyCode.H,
					CompoundPartAlign = KeyCode.U,
                    ToggleReRoot = KeyCode.K,
                    ToggleNoOffsetLimit = KeyCode.L,
                    StartMasterSnap = KeyCode.LeftControl,

					Up = KeyCode.UpArrow,
					Down = KeyCode.DownArrow,
					Left = KeyCode.LeftArrow,
					Right = KeyCode.RightArrow,
					Forward = KeyCode.RightShift,
					Back = KeyCode.RightControl
						
				};
				defaultConfig.KeyMap = defaultKeys;

				if (ConfigManager.SaveConfig (defaultConfig, configFilePath))
					Log.Debug ("Created default config");
				else
					Log.Error ("Failed to save default config");

				return defaultConfig;
			} catch (Exception ex) {
				Log.Error ("Error defaulting config: " + ex.Message);
				return null;
			}
		}
	}
}


using System;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

namespace EditorExtensionsRedux
{
	[XmlRoot("ConfigData")]
	public class ConfigData
	{
		[XmlElement("FileVersion")]
		public string FileVersion { get; set; }

		[XmlElement("KeyMap")]
		public KeyMaps KeyMap { get; set; }

		[XmlArray("AngleSnapValues")]
		public List<float> AngleSnapValues { get; set; }

		[XmlElement("MaxSymmetry")]
		public int MaxSymmetry { get; set; }

		[XmlElement("ShowDebugInfo")]
		public bool ShowDebugInfo { get; set; }

		/// <summary>
		/// Time, in seconds, that the on screen messages will display
		/// </summary>
		[XmlElement("OnScreenMessageTime")]
		public float OnScreenMessageTime { get; set; }

		public ConfigData()
		{
			this.KeyMap = new KeyMaps ();
		}
	}

	public class KeyMaps
	{
		[XmlElement("ResetCamera")]
		public KeyCode ResetCamera { get; set; }

		[XmlElement("ZoomSelected")]
		public KeyCode ZoomSelected { get;set; }

		[XmlElement("AttachmentMode")]
		public KeyCode AttachmentMode { get; set; }

		[XmlElement("PartClipping")]
		public KeyCode PartClipping { get; set; }

		[XmlElement("VerticalSnap")]
		public KeyCode VerticalSnap { get; set; }

		[XmlElement("HorizontalSnap")]
		public KeyCode HorizontalSnap { get; set; }

		[XmlElement("CompoundPartAlign")]
		public KeyCode CompoundPartAlign { get; set; }
	}
}


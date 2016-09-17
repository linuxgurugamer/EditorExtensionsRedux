using System;
using UnityEngine;

namespace EditorExtensionsRedux
{
	#if DEBUG
//	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
//	public class Debug_AutoLoadQuicksaveOnStartup: UnityEngine.MonoBehaviour
//	{
//		public static bool first = true;
//		public void Start()
//		{
//			if (first)
//			{
//				first = false;
//				HighLogic.SaveFolder = "dev";
//				var game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
//
//				if (game != null && game.flightState != null && game.compatible)
//				{
//					HighLogic.LoadScene(GameScenes.SPACECENTER);
//				}
//			}
//		}
//	}
	#endif
}


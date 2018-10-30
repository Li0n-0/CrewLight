using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace CrewLight
{
	[KSPAddon(KSPAddon.Startup.EveryScene, true)]
	public class GameSettingsLive : MonoBehaviour
	{
		public static List<MorseCode> morseCode;
		public static bool inSunlight = true;
		public static int layerMask = (1 << 10 | 1 << 15); // Scaled & Local Scenery layer
		public static int maxSearch = 200;

		private CL_GeneralSettings morseSettings;

		// Backup :
		private string bckCode;
		private float bckDih, bckDah, bckSymSpace, bckLetterSpace, bckWordSpace;
		private bool bckManual;

		// window pos
		Vector2d windowPos;

		private Texture morseAlph;

		public void Start ()
		{
			GameEvents.OnGameSettingsApplied.Add (SettingsApplied);
			GameEvents.onGameStateLoad.Add (GameLoad);
			GameEvents.onGameUnpause.Add (OutOfPause);

			windowPos = new Vector2d (Screen.width / 2 + 120, Screen.height / 2 - 150);
			morseSettingsRect = new Rect ((float)windowPos.x, (float)windowPos.y, 1, 1);
			morseAlphabetRect = new Rect ((float)windowPos.x - 680, (float)windowPos.y, 450, 450);

			DoStart ();

			DontDestroyOnLoad (this);
		}

		private void DoStart ()
		{
			if (HighLogic.fetch.currentGame == null)
			{
				return;
			}

			morseAlph = (Texture)GameDatabase.Instance.GetTexture ("CrewLight/International_Morse_Code", false);

			morseSettings = HighLogic.CurrentGame.Parameters.CustomParams<CL_GeneralSettings> ();
			ParseSettings ();
		}

		public void OnDestroy ()
		{
			GameEvents.OnGameSettingsApplied.Remove (SettingsApplied);
			GameEvents.onGameStateLoad.Remove (GameLoad);
			GameEvents.onGameUnpause.Remove (OutOfPause);
		}

		private void GameLoad (ConfigNode node)
		{
			DoStart ();
		}

		private void OutOfPause ()
		{
			CloseSettings ();
		}

		private void SettingsApplied ()
		{
			// execute once when leaving the stock setting screen

			if (morseSettings.morseConf)
			{
				// reset the more morse conf toggle to false asap
				morseSettings.morseConf = false;
				morseSettings.Save (HighLogic.CurrentGame.config);

				showSettingsWindow = true;
			}
			// backup the original settings
			ParseToBackup ();
		}

		private void ParseToBackup ()
		{
			bckCode = morseSettings.morseCodeStr;
			bckDih = morseSettings.ditDuration;
			bckDah = morseSettings.dahDuration;
			bckSymSpace = morseSettings.symbolSpaceDuration;
			bckLetterSpace = morseSettings.letterSpaceDuration;
			bckWordSpace = morseSettings.wordSpaceDuration;
			bckManual = morseSettings.manualTiming;
		}

		private void RestoreBackup ()
		{
			morseSettings.morseCodeStr = bckCode;
			morseSettings.ditDuration = bckDih;
			morseSettings.dahDuration = bckDah;
			morseSettings.symbolSpaceDuration = bckSymSpace;
			morseSettings.letterSpaceDuration = bckLetterSpace;
			morseSettings.wordSpaceDuration = bckWordSpace;
			morseSettings.manualTiming = bckManual;
		}

		private void ParseSettings ()
		{
			ParseMorseCode ();
		}

		private void ParseMorseCode ()
		{
			morseCode = new List<MorseCode> ();
			foreach (char c in morseSettings.morseCodeStr) {
				switch (c) {
				case '.':
					morseCode.Add (MorseCode.dih);
					break;
				case '_':
					morseCode.Add (MorseCode.dah);
					break;
				case '-':
					morseCode.Add (MorseCode.dah);
					break;
				case ' ':
					morseCode.Add (MorseCode.letterspc);
					break;
				case '|':
					morseCode.Add (MorseCode.wordspc);
					break;
				default:
					morseCode.Add (MorseCode.dih);
					break;
				}
				morseCode.Add (MorseCode.symspc);
			}
		}

		private void ApplySettings ()
		{
			ParseSettings ();

			morseSettings.Save (HighLogic.CurrentGame.config);

			showSettingsWindow = false;
			showAlphabetWindow = false;
		}

		private void CloseSettings ()
		{
			RestoreBackup ();

			showSettingsWindow = false;
			showAlphabetWindow = false;
		}

		#region MorseGUI

		private bool showSettingsWindow = false;
		private bool showAlphabetWindow = false;

		private Rect morseSettingsRect;
		private Rect morseAlphabetRect;

		public void OnGUI ()
		{
			if (showSettingsWindow)
			{
				GUILayout.BeginArea (morseSettingsRect);
				morseSettingsRect = GUILayout.Window (991237, morseSettingsRect, MorseSettingsWindow, Localizer.Format ("#autoLOC_CL_0077"));
				GUILayout.EndArea ();
			}

			if (showAlphabetWindow)
			{
				GUILayout.BeginArea (morseAlphabetRect);
				morseAlphabetRect = GUILayout.Window (596064, morseAlphabetRect, MorseAlphabetWindow, Localizer.Format ("#autoLOC_CL_0078"));
				GUILayout.EndArea ();
			}
		}

		public void MorseSettingsWindow (int id)
		{
			GUILayout.BeginVertical ();

			morseSettings.morseCodeStr = GUILayout.TextField (morseSettings.morseCodeStr);

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (/*Dit*/Localizer.Format ("#autoLOC_CL_0029") + " (.)")) {
				morseSettings.morseCodeStr += ".";
			}
			if (GUILayout.Button (/*"Dah*/Localizer.Format ("#autoLOC_CL_0032") +  " (_)")) {
				morseSettings.morseCodeStr += "_";
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (/*"Letter Space */Localizer.Format ("#autoLOC_CL_0036") + " ( )")) {
				morseSettings.morseCodeStr += " ";
			}
			if (GUILayout.Button (/*"Word Space*/Localizer.Format ("#autoLOC_CL_0038") + " (|)")) {
				morseSettings.morseCodeStr += "|";
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (/*"Dih duration :"*/Localizer.Format ("#autoLOC_CL_0030"));
			if (GUILayout.Button ("--")) {
				morseSettings.ditDuration -= .1f;
				UpdateTiming ();
			}
			if (GUILayout.Button ("-")) {
				morseSettings.ditDuration -= .01f;
				UpdateTiming ();
			}
			GUILayout.Label (morseSettings.ditDuration.ToString ());
			if (GUILayout.Button ("+")) {
				morseSettings.ditDuration += .01f;
				UpdateTiming ();
			}

			if (GUILayout.Button ("++")) {
				morseSettings.ditDuration += .1f;
				UpdateTiming ();
			}
			GUILayout.EndHorizontal ();

			morseSettings.manualTiming = GUILayout.Toggle (morseSettings.manualTiming, /*"Manual Timing"*/Localizer.Format ("#autoLOC_CL_0031"));

			GUILayout.BeginHorizontal ();
			GUILayout.Label (/*"Dah duration :"*/Localizer.Format ("#autoLOC_CL_0033"));
			if (GUILayout.Button ("--")) {
				if (morseSettings.manualTiming) {
					morseSettings.dahDuration -= .1f;
				}
			}
			if (GUILayout.Button ("-")) {
				if (morseSettings.manualTiming) {
					morseSettings.dahDuration -= .01f;
				}
			}
			GUILayout.Label (morseSettings.dahDuration.ToString ());
			if (GUILayout.Button ("+")) {
				if (morseSettings.manualTiming) {
					morseSettings.dahDuration += .01f;
				}
			}
			if (GUILayout.Button ("++")) {
				if (morseSettings.manualTiming) {
					morseSettings.dahDuration += .1f;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (/*"Symbol Space :"*/Localizer.Format ("#autoLOC_CL_0034"));
			if (GUILayout.Button ("--")) {
				if (morseSettings.manualTiming) {
					morseSettings.symbolSpaceDuration -= .1f;
				}
			}
			if (GUILayout.Button ("-")) {
				if (morseSettings.manualTiming) {
					morseSettings.symbolSpaceDuration -= .01f;
				}
			}
			GUILayout.Label (morseSettings.symbolSpaceDuration.ToString ());
			if (GUILayout.Button ("+")) {
				if (morseSettings.manualTiming) {
					morseSettings.symbolSpaceDuration += .01f;
				}
			}
			if (GUILayout.Button ("++")) {
				if (morseSettings.manualTiming) {
					morseSettings.symbolSpaceDuration += .1f;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (/*"Letter Space :"*/Localizer.Format ("#autoLOC_CL_0036"));
			if (GUILayout.Button ("--")) {
				if (morseSettings.manualTiming) {
					morseSettings.letterSpaceDuration -= .1f;
				}
			}
			if (GUILayout.Button ("-")) {
				if (morseSettings.manualTiming) {
					morseSettings.letterSpaceDuration -= .01f;
				}
			}
			GUILayout.Label (morseSettings.letterSpaceDuration.ToString ());
			if (GUILayout.Button ("+")) {
				if (morseSettings.manualTiming) {
					morseSettings.letterSpaceDuration += .01f;
				}
			}
			if (GUILayout.Button ("++")) {
				if (morseSettings.manualTiming) {
					morseSettings.letterSpaceDuration += .1f;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label (/*"Word Space :"*/Localizer.Format ("#autoLOC_CL_0038"));
			if (GUILayout.Button ("--")) {
				if (morseSettings.manualTiming) {
					morseSettings.wordSpaceDuration -= .1f;
				}
			}
			if (GUILayout.Button ("-")) {
				if (morseSettings.manualTiming) {
					morseSettings.wordSpaceDuration -= .01f;
				}
			}
			GUILayout.Label (morseSettings.wordSpaceDuration.ToString ());
			if (GUILayout.Button ("+")) {
				if (morseSettings.manualTiming) {
					morseSettings.wordSpaceDuration += .01f;
				}
			}
			if (GUILayout.Button ("++")) {
				if (morseSettings.manualTiming) {
					morseSettings.wordSpaceDuration += .1f;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (/*"Cancel"*/Localizer.Format ("#autoLOC_CL_0079"))) {
				CloseSettings ();
			}
			if (GUILayout.Button (/*"Morse Alphabet"*/Localizer.Format ("#autoLOC_CL_0078"))) {
				showAlphabetWindow = !showAlphabetWindow;
			}
			if (GUILayout.Button (/*"Apply"*/Localizer.Format ("#autoLOC_CL_0080"))) {
				ApplySettings ();
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}

		public void MorseAlphabetWindow (int id)
		{
			GUILayout.Label (morseAlph);

			GUI.DragWindow ();
		}

		private void UpdateTiming ()
		{
			if (!morseSettings.manualTiming)
			{
				morseSettings.dahDuration = morseSettings.ditDuration * 3;
				morseSettings.symbolSpaceDuration = morseSettings.ditDuration;
				morseSettings.letterSpaceDuration = morseSettings.dahDuration;
				morseSettings.wordSpaceDuration = morseSettings.dahDuration * 3;
			}
		}
		#endregion
	}
}


using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public class Settings
	{
		private ConfigNode settingsNode;
		private ConfigNode nodeDistantVesselLight;
		private ConfigNode nodeLightActionGroup;

		// Default settings :
		public bool useMorseCode = true;
		public bool onlyForControllable = false;
		public string morseCodeStr = "_._|...|.__.";
		public double distance = 200d;
		public float ditDuration = .9f;
		public float dahDuration = 2f;
		public float symbolSpaceDuration = 1f;
		public float letterSpaceDuration = 1.3f;
		public float wordSpaceDuration = 1.7f;
		public bool disableAutoAG = true;

		public List<int> morseCode;

		private void ParseMorse ()
		{
			morseCode = new List<int> ();
			foreach (char c in morseCodeStr) {
				switch (c) {
				case '.':
					morseCode.Add (0);
					break;
				case '_':
					morseCode.Add (1);
					break;
				case '|':
					morseCode.Add (2);
					break;
				case ' ':
					morseCode.Add (3);
					break;
				}
				morseCode.Add (4);
			}
		}

		private bool LoadNodes ()
		{
			settingsNode = ConfigNode.Load (KSPUtil.ApplicationRootPath + "GameData/CrewLight/PluginData/Settings.cfg");
			if (settingsNode == null) {
				return false;
			}
			if (settingsNode.HasNode("Distant_Vessel_Morse_Code")) {
				nodeDistantVesselLight = settingsNode.GetNode ("Distant_Vessel_Morse_Code");
			} else { return false; }
			if (settingsNode.HasNode("Light_Action_Group")) {
				nodeLightActionGroup = settingsNode.GetNode ("Light_Action_Group");
			} else { return false; }

			string[] paramMorseValue = new string[] {
				"use_morse_code",
				"only_for_controllable_vessel",
				"morse_code",
				"distance",
				"dit",
				"dah",
				"symbol_space",
				"letter_space",
				"word_space"
			};
			string[] paramLightAGValue = new string[] {
				"disable_auto_light_action_group"
			};
			if (nodeDistantVesselLight.HasValues (paramMorseValue) && nodeLightActionGroup.HasValues (paramLightAGValue)) {
				useMorseCode = bool.Parse (nodeDistantVesselLight.GetValue ("use_morse_code"));
				onlyForControllable = bool.Parse (nodeDistantVesselLight.GetValue("only_for_controllable_vessel"));
				morseCodeStr = nodeDistantVesselLight.GetValue ("morse_code");
				distance = Double.Parse(nodeDistantVesselLight.GetValue ("distance"));
				ditDuration = float.Parse (nodeDistantVesselLight.GetValue ("dit"));
				dahDuration = float.Parse (nodeDistantVesselLight.GetValue ("dah"));
				symbolSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("symbol_space"));
				letterSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("letter_space"));
				wordSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("word_space"));
				disableAutoAG = bool.Parse (nodeLightActionGroup.GetValue ("disable_auto_light_action_group"));
			} else { return false; }

			return true;
		}



		public void Load ()
		{
			if (!LoadNodes()) {
				Create ();
			}
			ParseMorse ();
			Debug.Log ("[Crew Light] Settings : settings.cfg node : " + settingsNode.ToString ());
		}

		private void Create ()
		{
			settingsNode = new ConfigNode ();

			settingsNode.AddNode ("Distant_Vessel_Morse_Code");
			settingsNode.AddNode ("Light_Action_Group");

			ConfigNode nodeDistantVesselLight = settingsNode.GetNode ("Distant_Vessel_Morse_Code");
			ConfigNode nodeLightActionGroup = settingsNode.GetNode ("Light_Action_Group");

			nodeDistantVesselLight.AddValue ("use_morse_code", useMorseCode);

			nodeDistantVesselLight.AddValue ("only_for_controllable_vessel", onlyForControllable);

			nodeDistantVesselLight.AddValue ("morse_code", morseCodeStr, 
				"'.' for ti, '_' for taah, '|' for separate letters, ' ' for separate words");
			
			nodeDistantVesselLight.AddValue ("distance", distance, 
				"distance at which the message begin, in meter, maximum 200");
			
			nodeDistantVesselLight.AddValue("dit", ditDuration, 
				"duration of the light for the dit (.), in seconds");
			
			nodeDistantVesselLight.AddValue ("dah", dahDuration, 
				"duration of the light for the dah (_), in seconds");
			
			nodeDistantVesselLight.AddValue ("symbol_space", symbolSpaceDuration, 
				"duration of the darkness between two symbol, in seconds");
			
			nodeDistantVesselLight.AddValue ("letter_space", letterSpaceDuration, 
				"duration of the darkness between two letters, '|', in seconds");
			
			nodeDistantVesselLight.AddValue ("word_space", wordSpaceDuration, 
				"duration of the darkness between two words, ' ', in seconds");

			nodeLightActionGroup.AddValue ("disable_auto_light_action_group", disableAutoAG, 
				"Don't use Light action group for crewable part");

			settingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/CrewLight/Settings.cfg");
		}
	}
}


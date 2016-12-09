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
		private ConfigNode nodeSunLight;

		// Default settings :

		// Distant Lightning :
		public bool useMorseCode = true;
		public bool onlyForControllable = false;
		public string morseCodeStr = "__.|._..|.|_.|_.";
		public double distance = 200d;
		public float ditDuration = .9f;
		public float dahDuration = 2f;
		public float symbolSpaceDuration = 1f;
		public float letterSpaceDuration = 1.3f;
		public float wordSpaceDuration = 1.7f;

		// Sun Light :
		public bool useSunLight = true;
		public bool onlyNoAGpart = true;

		// Light Action Group :
		public bool disableCrewAG = true;
		public bool disableAllAG = false;

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
			if (settingsNode.HasNode("Sun_Light")) {
				nodeSunLight = settingsNode.GetNode ("Sun_Light");
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
			string[] paramSunLightValue = new string[] {
				"use_sun_light",
				"only_light_not_in_AG"
			};
			string[] paramLightAGValue = new string[] {
				"disable_light_action_group_for_crew_part",
				"disable_action_group_for_light_part"
			};

			if (nodeDistantVesselLight.HasValues (paramMorseValue) 
				&& nodeSunLight.HasValues(paramSunLightValue)
				&& nodeLightActionGroup.HasValues (paramLightAGValue))
			{
				useMorseCode = bool.Parse (nodeDistantVesselLight.GetValue ("use_morse_code"));
				onlyForControllable = bool.Parse (nodeDistantVesselLight.GetValue("only_for_controllable_vessel"));
				morseCodeStr = nodeDistantVesselLight.GetValue ("morse_code");
				distance = Double.Parse(nodeDistantVesselLight.GetValue ("distance"));
				ditDuration = float.Parse (nodeDistantVesselLight.GetValue ("dit"));
				dahDuration = float.Parse (nodeDistantVesselLight.GetValue ("dah"));
				symbolSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("symbol_space"));
				letterSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("letter_space"));
				wordSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("word_space"));

				useSunLight = bool.Parse (nodeSunLight.GetValue ("use_sun_light"));
				onlyNoAGpart = bool.Parse (nodeSunLight.GetValue ("only_light_not_in_AG"));

				disableCrewAG = bool.Parse (nodeLightActionGroup.GetValue ("disable_light_action_group_for_crew_part"));
				disableAllAG = bool.Parse (nodeLightActionGroup.GetValue("disable_action_group_for_light_part"));
			} else { return false; }

			return true;
		}



		public void Load ()
		{
			if (!LoadNodes()) {
				Create ();
			}
			ParseMorse ();
		}

		private void Create ()
		{
			settingsNode = new ConfigNode ();

			settingsNode.AddNode ("Distant_Vessel_Morse_Code");
			settingsNode.AddNode ("Sun_Light");
			settingsNode.AddNode ("Light_Action_Group");

			ConfigNode nodeDistantVesselLight = settingsNode.GetNode ("Distant_Vessel_Morse_Code");
			ConfigNode nodeSunLight = settingsNode.GetNode ("Sun_Light");
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

			nodeSunLight.AddValue("use_sun_light", useSunLight, 
				"lights will go on/off as the sun rise/fall");

			nodeSunLight.AddValue ("only_light_not_in_AG", onlyNoAGpart,
				"only lights not assigned to an Action Group will be lighted when the sun fall");

			nodeLightActionGroup.AddValue ("disable_light_action_group_for_crew_part", disableCrewAG, 
				"remove crewable part from the Light Action Group");

			nodeLightActionGroup.AddValue ("disable_action_group_for_light_part", disableAllAG, 
				"remove all the light part from the Light Action Group");

			settingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/CrewLight/PluginData/Settings.cfg");
		}
	}
}


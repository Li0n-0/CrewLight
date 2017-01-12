using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public static class CLSettings
	{
		private static ConfigNode settingsNode;
		private static ConfigNode nodeDistantVesselLight;
		private static ConfigNode nodeSunLight;
		private static ConfigNode nodeEVALight;
		private static ConfigNode nodeLightActionGroup;

		// Default settings :

		// Distant Lightning :
		public static bool useMorseCode = true;
		public static bool onlyForControllable = false;
		public static string morseCodeStr = "_._|...|.__.";
		public static double distance = 200d;
		public static float ditDuration = .9f;
		public static float dahDuration = 2f;
		public static float symbolSpaceDuration = 1f;
		public static float letterSpaceDuration = 1.3f;
		public static float wordSpaceDuration = 1.7f;

		// Sun Light :
		public static bool useSunLight = true;
		public static bool onlyNoAGpart = true;
		public static bool useDepthLight = true;
		public static double depthThreshold = 20d;

		// EVA Light :
		public static bool useSunLightEVA = true;
		public static bool onForEVASpace = false;
		public static bool onForEVALanded = false;

		// Light Action Group :
		public static bool disableCrewAG = true;
		public static bool disableAllAG = false;

		// Internal :
		public static List<int> morseCode;
		public static int layerMask = (1 << 10 | 1 << 15); // Scaled & Local Scenery layer
		public static float waitBetweenRay = 1.5f;
		public static int maxSearch = 200;

		static CLSettings ()
		{
			if (!LoadNodes()) {
				Create ();
			}
			ParseMorse ();
		}

		private static void ParseMorse ()
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

		private static bool LoadNodes ()
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
			if (settingsNode.HasNode("EVA_Light")) {
				nodeEVALight = settingsNode.GetNode ("EVA_Light");
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
				"use_depth_light",
				"depth_threshold",
				"only_light_not_in_AG"
			};
			string[] paramEVALight = new string[] {
				"use_sunlight_for_EVA",
				"always_on_in_space",
				"always_on_landed"
			};
			string[] paramLightAGValue = new string[] {
				"disable_light_action_group_for_crew_part",
				"disable_action_group_for_light_part"
			};

			if (nodeDistantVesselLight.HasValues (paramMorseValue) 
				&& nodeSunLight.HasValues (paramSunLightValue) 
				&& nodeEVALight.HasValues (paramEVALight)
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
				useDepthLight = bool.Parse (nodeSunLight.GetValue ("use_depth_light"));
				depthThreshold = Double.Parse (nodeSunLight.GetValue ("depth_threshold"));
				onlyNoAGpart = bool.Parse (nodeSunLight.GetValue ("only_light_not_in_AG"));

				useSunLightEVA = bool.Parse (nodeEVALight.GetValue ("use_sunlight_for_EVA"));
				onForEVASpace = bool.Parse (nodeEVALight.GetValue ("always_on_in_space"));
				onForEVALanded = bool.Parse (nodeEVALight.GetValue ("always_on_landed"));

				disableCrewAG = bool.Parse (nodeLightActionGroup.GetValue ("disable_light_action_group_for_crew_part"));
				disableAllAG = bool.Parse (nodeLightActionGroup.GetValue("disable_action_group_for_light_part"));
			} else { return false; }

			return true;
		}

		private static void Create ()
		{
			settingsNode = new ConfigNode ();

			settingsNode.AddNode ("Distant_Vessel_Morse_Code");
			settingsNode.AddNode ("Sun_Light");
			settingsNode.AddNode ("EVA_Light");
			settingsNode.AddNode ("Light_Action_Group");

			ConfigNode nodeDistantVesselLight = settingsNode.GetNode ("Distant_Vessel_Morse_Code");
			ConfigNode nodeSunLight = settingsNode.GetNode ("Sun_Light");
			ConfigNode nodeEVALight = settingsNode.GetNode ("EVA_Light");
			ConfigNode nodeLightActionGroup = settingsNode.GetNode ("Light_Action_Group");

			// Distant Vessel :
			nodeDistantVesselLight.AddValue ("use_morse_code", useMorseCode);

			nodeDistantVesselLight.AddValue ("only_for_controllable_vessel", onlyForControllable);

			nodeDistantVesselLight.AddValue ("morse_code", morseCodeStr, 
				"'.' for ti, '_' for taah, '|' for separate letters, ' ' for separate words");
			
			nodeDistantVesselLight.AddValue ("distance", distance, 
				"distance at which the message begin, in meter, maximum 2000");
			
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

			// Sun Light :
			nodeSunLight.AddValue("use_sun_light", useSunLight, 
				"lights will go on/off as the sun rise/fall");

			nodeSunLight.AddValue ("use_depth_light", useDepthLight,
				"lights will go on/off when the craft reach a certain depth");

			nodeSunLight.AddValue ("depth_threshold", depthThreshold);

			nodeSunLight.AddValue ("only_light_not_in_AG", onlyNoAGpart,
				"only lights not assigned to an Action Group will be lighted when the sun fall");

			// EVA Light :
			nodeEVALight.AddValue ("use_sunlight_for_EVA", useSunLightEVA, 
				"kerbal's headlights will go on/off as the sun rise/fall");

			nodeEVALight.AddValue ("always_on_in_space", onForEVASpace, 
				"always turn on the headlights when EVA in space");

			nodeEVALight.AddValue ("always_on_landed", onForEVALanded, 
				"always turn on the headlights when EVA landed");

			// Editor Light :
			nodeLightActionGroup.AddValue ("disable_light_action_group_for_crew_part", disableCrewAG, 
				"remove crewable part from the Light Action Group");

			nodeLightActionGroup.AddValue ("disable_action_group_for_light_part", disableAllAG, 
				"remove all the light part from the Light Action Group");

			settingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/CrewLight/PluginData/Settings.cfg");
		}
	}
}


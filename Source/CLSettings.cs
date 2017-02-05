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
		private static ConfigNode nodeVesselLightsOnEVA;
		private static ConfigNode nodeTransferCrew;

		// Default settings :

		// Distant Lightning :
		public static bool useMorseCode = true;
		public static bool onlyForControllable = false;
		public static string morseCodeStr = "_._|...|.__.";
		public static double distance = 200d;
		public static float ditDuration = 1.1f;
		public static float dahDuration = 2.5f;
		public static float symbolSpaceDuration = 1.1f;
		public static float letterSpaceDuration = 1.7f;
		public static float wordSpaceDuration = 2.5f;

		// Sun Light :
		public static bool useSunLight = true;
		public static bool onlyNoAGpart = true;
		public static bool useDepthLight = true;
		public static double depthThreshold = 20d;
		public static float delayLowTimeWarp = 2f;
		public static float delayHighTimeWarp = .1f;
		public static bool useStaggerdLight = true;
		public static int maxLightPerStage = 6;
		public static int minLightPerStage = 2;
		public static float delayStage = 1.5f;
		public static bool useRandomDelay = true;

		// EVA Light :
		public static bool useSunLightEVA = true;
		public static bool onForEVASpace = false;
		public static bool onForEVALanded = false;

		// Light Action Group :
		public static bool disableCrewAG = true;
		public static bool disableAllAG = false;

		// Toggle vessel lights on EVA
		public static bool useVesselLightsOnEVA = true;
		public static bool lightSymLights = false;

		// Transfer Crew
		public static bool useTransferCrew = true;

		// Internal :
		public static List<int> morseCode;
		public static int layerMask = (1 << 10 | 1 << 15); // Scaled & Local Scenery layer
		public static int maxSearch = 200;

		static CLSettings ()
		{
			// Check for the settings file
			settingsNode = ConfigNode.Load (KSPUtil.ApplicationRootPath + "GameData/CrewLight/PluginData/Settings.cfg");
			if (settingsNode == null) {
				settingsNode = new ConfigNode ();
			}

			// Check for nodes in settings file
			if (! settingsNode.HasNode("Distant_Vessel_Morse_Code")) {
				settingsNode.AddNode ("Distant_Vessel_Morse_Code");
			}
			nodeDistantVesselLight = settingsNode.GetNode ("Distant_Vessel_Morse_Code");

			if (! settingsNode.HasNode("Sun_Light")) {
				settingsNode.AddNode ("Sun_Light");
			}
			nodeSunLight = settingsNode.GetNode ("Sun_Light");

			if (! settingsNode.HasNode("EVA_Light")) {
				settingsNode.AddNode ("EVA_Light");
			}
			nodeEVALight = settingsNode.GetNode ("EVA_Light");

			if (! settingsNode.HasNode("Light_Action_Group")) {
				settingsNode.AddNode ("Light_Action_Group");
			}
			nodeLightActionGroup = settingsNode.GetNode ("Light_Action_Group");

			if (! settingsNode.HasNode("Toggle_Vessel_Lights_On_EVA")) {
				settingsNode.AddNode ("Toggle_Vessel_Lights_On_EVA");
			}
			nodeVesselLightsOnEVA = settingsNode.GetNode ("Toggle_Vessel_Lights_On_EVA");

			if (! settingsNode.HasNode ("Crew_Light")) {
				settingsNode.AddNode ("Crew_Light");
			}
			nodeTransferCrew = settingsNode.GetNode ("Crew_Light");

			// Check for values in settings file
			// Distant Vessel Morse Code
			//
			if (nodeDistantVesselLight.HasValue ("use_morse_code")) {
				useMorseCode = bool.Parse (nodeDistantVesselLight.GetValue ("use_morse_code"));
			}
			nodeDistantVesselLight.SetValue ("use_morse_code", useMorseCode, true);

			if (nodeDistantVesselLight.HasValue ("only_for_controllable_vessel")) {
				onlyForControllable = bool.Parse (nodeDistantVesselLight.GetValue ("only_for_controllable_vessel"));
			}
			nodeDistantVesselLight.SetValue ("only_for_controllable_vessel", onlyForControllable, true);

			if (nodeDistantVesselLight.HasValue ("morse_code")) {
				morseCodeStr = nodeDistantVesselLight.GetValue ("morse_code");
			}
			nodeDistantVesselLight.SetValue ("morse_code", morseCodeStr, 
				"'.' for ti, '_' for taah, '|' for separate letters, ' ' for separate words", true);
			
			if (nodeDistantVesselLight.HasValue ("distance")) {
				distance = Double.Parse (nodeDistantVesselLight.GetValue ("distance"));
			}
			nodeDistantVesselLight.SetValue ("distance", distance, 
				"distance at which the message begin, in meter, maximum 2000", true);
			
			if (nodeDistantVesselLight.HasValue ("dit")) {
				ditDuration = float.Parse (nodeDistantVesselLight.GetValue ("dit"));
			}
			nodeDistantVesselLight.SetValue("dit", ditDuration, 
				"duration of the light for the dit (.), in seconds", true);
			
			if (nodeDistantVesselLight.HasValue ("dah")) {
				dahDuration = float.Parse (nodeDistantVesselLight.GetValue ("dah"));
			}
			nodeDistantVesselLight.SetValue ("dah", dahDuration, 
				"duration of the light for the dah (_), in seconds", true);
			
			if (nodeDistantVesselLight.HasValue ("symbol_space")) {
				symbolSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("symbol_space"));
			}
			nodeDistantVesselLight.SetValue ("symbol_space", symbolSpaceDuration, 
				"duration of the darkness between two symbol, in seconds", true);

			if (nodeDistantVesselLight.HasValue ("letter_space")) {
				letterSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("letter_space"));
			}
			nodeDistantVesselLight.SetValue ("letter_space", letterSpaceDuration, 
				"duration of the darkness between two letters, '|', in seconds", true);

			if (nodeDistantVesselLight.HasValue ("word_space")) {
				wordSpaceDuration = float.Parse (nodeDistantVesselLight.GetValue ("word_space"));
			}
			nodeDistantVesselLight.SetValue ("word_space", wordSpaceDuration, 
				"duration of the darkness between two words, ' ', in seconds", true);
			//
			// Sun Light
			//
			if (nodeSunLight.HasValue ("use_sun_light")) {
				useSunLight = bool.Parse (nodeSunLight.GetValue ("use_sun_light"));
			}
			nodeSunLight.SetValue("use_sun_light", useSunLight, 
				"lights will go on/off as the sun rise/fall", true);

			if (nodeSunLight.HasValue ("use_depth_light")) {
				useDepthLight = bool.Parse (nodeSunLight.GetValue ("use_depth_light"));
			}
			nodeSunLight.SetValue ("use_depth_light", useDepthLight,
				"lights will go on/off when the craft reach a certain depth", true);

			if (nodeSunLight.HasValue ("depth_threshold")) {
				depthThreshold = Double.Parse (nodeSunLight.GetValue ("depth_threshold"));
			}
			nodeSunLight.SetValue ("depth_threshold", depthThreshold, true);

			if (nodeSunLight.HasValue ("only_light_not_in_AG")) {
				onlyNoAGpart = bool.Parse (nodeSunLight.GetValue ("only_light_not_in_AG"));
			}
			nodeSunLight.SetValue ("only_light_not_in_AG", onlyNoAGpart,
				"only lights not assigned to an Action Group will be lighted when the sun fall", true);

			if (nodeSunLight.HasValue ("delay_in_low_timewarp")) {
				delayLowTimeWarp = float.Parse (nodeSunLight.GetValue ("delay_in_low_timewarp"));
			}
			nodeSunLight.SetValue ("delay_in_low_timewarp", delayLowTimeWarp, 
				"delay between check of the sun position when in physic timewrap, increase for better performance, " +
				"lower for a quicker response of the lights." +
				"Is divided by the current warp-time speed", true);

			if (nodeSunLight.HasValue ("delay_in_high_timewarp")) {
				delayHighTimeWarp = float.Parse (nodeSunLight.GetValue ("delay_in_high_timewarp"));
			}
			nodeSunLight.SetValue ("delay_in_high_timewarp", delayHighTimeWarp, 
				"delay between check of the sun position when in on-rail timewrap, increase for better performance, " +
				"lower for a quicker response of the lights", true);

			if (nodeSunLight.HasValue ("use_staggered_lightning")) {
				useStaggerdLight = bool.Parse (nodeSunLight.GetValue ("use_staggered_lightning"));
			}
			nodeSunLight.SetValue ("use_staggered_lightning", useStaggerdLight, "turn on the light in a staggered " +
				"way, or all at the same time", true);

			if (nodeSunLight.HasValue ("max_light_per_stage")) {
				maxLightPerStage = int.Parse (nodeSunLight.GetValue ("max_light_per_stage"));
			}
			nodeSunLight.SetValue ("max_light_per_stage", maxLightPerStage, true);

			if (nodeSunLight.HasValue ("min_light_per_stage")) {
				minLightPerStage = int.Parse (nodeSunLight.GetValue ("min_light_per_stage"));
			}
			nodeSunLight.SetValue ("min_light_per_stage", minLightPerStage, true);

			if (nodeSunLight.HasValue ("delay_between_stage")) {
				delayStage = float.Parse (nodeSunLight.GetValue ("delay_between_stage"));
			}
			nodeSunLight.SetValue ("delay_between_stage", delayStage, "in seconds", true);

			if (nodeSunLight.HasValue ("use_a_random_delay")) {
				useRandomDelay = bool.Parse (nodeSunLight.GetValue ("use_a_random_delay"));
			}
			nodeSunLight.SetValue ("use_a_random_delay", useRandomDelay, "different between each stage, " +
				"will overide the delay_between_stage above", true);
			//
			// EVA Light (helmet's lights)
			//
			if (nodeEVALight.HasValue ("use_sunlight_for_EVA")) {
				useSunLight = bool.Parse (nodeEVALight.GetValue ("use_sunlight_for_EVA"));
			}
			nodeEVALight.SetValue ("use_sunlight_for_EVA", useSunLightEVA, 
				"kerbal's headlights will go on/off as the sun rise/fall", true);

			if (nodeEVALight.HasValue ("always_on_in_space")) {
				onForEVASpace = bool.Parse (nodeEVALight.GetValue ("always_on_in_space"));
			}
			nodeEVALight.SetValue ("always_on_in_space", onForEVASpace, 
				"always turn on the headlights when EVA in space", true);
			
			if (nodeEVALight.HasValue ("always_on_landed")) {
				onForEVALanded = bool.Parse (nodeEVALight.GetValue ("always_on_landed"));
			}
			nodeEVALight.SetValue ("always_on_landed", onForEVALanded, 
				"always turn on the headlights when EVA landed", true);
			//
			// Light Action Group
			//
			if (nodeLightActionGroup.HasValue ("disable_light_action_group_for_crew_part")) {
				disableCrewAG = bool.Parse (nodeLightActionGroup.GetValue ("disable_light_action_group_for_crew_part"));
			}
			nodeLightActionGroup.SetValue ("disable_light_action_group_for_crew_part", disableCrewAG, 
				"remove crewable part from the Light Action Group", true);
			
			if (nodeLightActionGroup.HasValue ("disable_action_group_for_light_part")) {
				disableAllAG = bool.Parse (nodeLightActionGroup.GetValue ("disable_action_group_for_light_part"));
			}
			nodeLightActionGroup.SetValue ("disable_action_group_for_light_part", disableAllAG, 
				"remove all the light part from the Light Action Group", true);
			//
			// Toggle Vessel Light On EVA
			//
			if (nodeVesselLightsOnEVA.HasValue ("enable_EVA_toggle_of_vessel_lights")) {
				useVesselLightsOnEVA = bool.Parse (nodeVesselLightsOnEVA.GetValue ("enable_EVA_toggle_of_vessel_lights"));
			}
			nodeVesselLightsOnEVA.SetValue ("enable_EVA_toggle_of_vessel_lights", useVesselLightsOnEVA, true);

			if (nodeVesselLightsOnEVA.HasValue ("toggle_symmetric_lights")) {
				lightSymLights = bool.Parse (nodeVesselLightsOnEVA.GetValue ("toggle_symmetric_lights"));
			}
			nodeVesselLightsOnEVA.SetValue ("toggle_symmetric_lights", lightSymLights, 
				"if true all symmetrical lights will respond to the toggle", true);
			//
			// Transfer Crew
			//
			if (nodeTransferCrew.HasValue ("use_cabin_crew_lightning")) {
				useTransferCrew = bool.Parse (nodeTransferCrew.GetValue ("use_cabin_crew_lightning"));
			}
			nodeTransferCrew.SetValue ("use_cabin_crew_lightning", useTransferCrew, 
				"kerbal turns the light on in their cabin/pod", true);
			
			settingsNode.Save (KSPUtil.ApplicationRootPath + "GameData/CrewLight/PluginData/Settings.cfg");

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
	}
}


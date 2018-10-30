using System;
using UnityEngine;
using KSP.Localization;

namespace CrewLight
{
	public class CL_GeneralSettings : GameParameters.CustomParameterNode
	{
		public override string Title {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override string Section {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override string DisplaySection {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override int SectionOrder {
			get {
				return 1;
			}
		}

		public override GameParameters.GameMode GameMode {
			get {
				return GameParameters.GameMode.ANY;
			}
		}

		public override bool HasPresets {
			get {
				return false;
			}
		}

		[GameParameters.CustomParameterUI (/*Automatic Pod Lightning*/"#autoLOC_CL_0002", toolTip = "#autoLOC_CL_0003")]
		public bool useTransferCrew = true;



		[GameParameters.CustomParameterUI (/*Motion Detector*/"#autoLOC_CL_0020", toolTip = "#autoLOC_CL_0021")]
		public bool useMotionDetector = true;


		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy7 = "";

		[GameParameters.CustomStringParameterUI (/*Morse Code*/"#autoLOC_CL_0022")]
		public string dummy8 = "";

		[GameParameters.CustomParameterUI (/*Morse Code*/"#autoLOC_CL_0022", toolTip = "#autoLOC_CL_0023")]
		public bool useMorseCode = true;

		[GameParameters.CustomParameterUI (/*Only for controlable vessel*/"#autoLOC_CL_0024")]
		public bool onlyForControllable = false;

		[GameParameters.CustomStringParameterUI (/*Code*/"#autoLOC_CL_0025", toolTip = "#autoLOC_CL_0026")]
		public string morseCodeStr = "_._ ... .__.";

		[GameParameters.CustomIntParameterUI (/*Distance*/"#autoLOC_CL_0027", toolTip = "#autoLOC_CL_0028", minValue = 5, maxValue = 2000, displayFormat = "0m")]
		public int distance = 200;

		[GameParameters.CustomParameterUI (/*More Morse Config*/"#autoLOC_CL_0074", toolTip = "#autoLOC_CL_0075")]
		public bool morseConf = false;

		[GameParameters.CustomFloatParameterUI (/*Dih*/"#autoLOC_CL_0029", toolTip = "#autoLOC_CL_0030")]
		public float ditDuration = 1.1f;

		[GameParameters.CustomParameterUI (/*Manual Timing*/"#autoLOC_CL_0031")]
		public bool manualTiming = false;

		[GameParameters.CustomFloatParameterUI (/*Dah*/"#autoLOC_CL_0032", toolTip = "#autoLOC_CL_0033")]
		public float dahDuration = 2.5f;

		[GameParameters.CustomFloatParameterUI (/*Symbol Space*/"#autoLOC_CL_0034", toolTip = "#autoLOC_CL_0035")]
		public float symbolSpaceDuration = 1.1f;

		[GameParameters.CustomFloatParameterUI (/*Letter Space*/"#autoLOC_CL_0036", toolTip = "#autoLOC_CL_0037")]
		public float letterSpaceDuration = 1.7f;

		[GameParameters.CustomFloatParameterUI (/*Word Space*/"#autoLOC_CL_0038", toolTip = "#autoLOC_CL_0039")]
		public float wordSpaceDuration = 2.5f;

		[GameParameters.CustomStringParameterUI (/*Character for Letter Space*/"#autoLOC_CL_0040", toolTip = "#autoLOC_CL_0041")]
		public string letterSpaceChar = " ";

		[GameParameters.CustomStringParameterUI (/*Character for Word Space*/"#autoLOC_CL_0042", toolTip = "#autoLOC_CL_0043")]
		public string wordSpaceChar = "|";


		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy5 = "";

		[GameParameters.CustomStringParameterUI (/*Switch Light on EVA*/"#autoLOC_CL_0048")]
		public string dummy6 = "";

		[GameParameters.CustomParameterUI (/*Switch Light on EVA*/"#autoLOC_CL_0048", toolTip = "#autoLOC_CL_0049")]
		public bool useVesselLightsOnEVA = true;

		[GameParameters.CustomParameterUI (/*Toggle Sym Lights*/"#autoLOC_CL_0050")]
		public bool lightSymLights = false;

		[GameParameters.CustomParameterUI (/*Enable for AviationLights*/"#autoLOC_CL_0051", toolTip = "#autoLOC_CL_0052")]
		public bool onAviationLights = true;



		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy0 = "";

		[GameParameters.CustomStringParameterUI (/*Kerbal Headlight*/"#autoLOC_CL_0044")]
		public string dummy1 = "";

		[GameParameters.CustomParameterUI (/*Kerbal Headlight*/"#autoLOC_CL_0044", toolTip = "#autoLOC_CL_0045")]
		public bool useSunLightEVA = true;

		[GameParameters.CustomParameterUI (/*Always On in Space*/"#autoLOC_CL_0046")]
		public bool onForEVASpace = false;

		[GameParameters.CustomParameterUI (/*Always On Landed*/"#autoLOC_CL_0047")]
		public bool onForEVALanded = false;



		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy2 = "";

		[GameParameters.CustomStringParameterUI (/*Action Group*/"#autoLOC_CL_0053", autoPersistance = false)]
		public string dummy3 = "";

		[GameParameters.CustomParameterUI (/*Disable LAG for crewable part*/"#autoLOC_CL_0054", toolTip = "#autoLOC_CL_0055")]
		public bool disableCrewAG = true;

		[GameParameters.CustomParameterUI (/*Disable all LAG*/"#autoLOC_CL_0056", toolTip = "#autoLOC_CL_0057")]
		public bool disableAllAG = false;

		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy4 = "";

		[GameParameters.CustomParameterUI (/*Use AviationLights effects*/"#autoLOC_CL_0058", toolTip = "#autoLOC_CL_0059")]
		public bool useAviationLightsEffect = true;

		public override bool Interactible (System.Reflection.MemberInfo member, GameParameters parameters)
		{
			// HeadLight interactible
			if (member.Name == "onForEVASpace" || member.Name == "onForEVALanded") {
				if (useSunLightEVA) {
					return true;
				} else {
					return false;
				}
			}

			// EVA switch lights
			if (member.Name == "lightSymLights" || member.Name == "onAviationLights") {
				if (useVesselLightsOnEVA) {
					return true;
				} else {
					return false;
				}
			}

			// Morse Code
			if (member.Name == "onlyForControllable" || member.Name == "morseCodeStr" 
				|| member.Name == "distance" || member.Name == "morseConf")
			{
				if (useMorseCode) {
					return true;
				} else {
					return false;
				}
			}


			return base.Interactible (member, parameters);
		}

		public override bool Enabled (System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (member.Name == "ditDuration" || member.Name == "manualTiming" 
				|| member.Name == "dahDuration" || member.Name == "symbolSpaceDuration" 
				|| member.Name == "letterSpaceDuration"  || member.Name == "wordSpaceDuration" 
				|| member.Name == "letterSpaceChar" || member.Name == "wordSpaceChar")
			{
				return false;
			}

			return base.Enabled (member, parameters);
		}
	}

	public class CL_SunLightSettings : GameParameters.CustomParameterNode
	{
		public override string Title {
			get {
				return /*Light Detector*/Localizer.Format ("#autoLOC_CL_0004");
			}
		}

		public override string Section {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override string DisplaySection {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override int SectionOrder {
			get {
				return 2;
			}
		}

		public override GameParameters.GameMode GameMode {
			get {
				return GameParameters.GameMode.ANY;
			}
		}

		public override bool HasPresets {
			get {
				return false;
			}
		}

		[GameParameters.CustomParameterUI (/*Light Detector*/"#autoLOC_CL_0004", toolTip = "#autoLOC_CL_0005")]
		public bool useSunLight = true;

		[GameParameters.CustomParameterUI (/*Only for lights NOT in the Light Action Group*/"#autoLOC_CL_0006")]
		public bool onlyNoAGpart = true;

		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy0 = "";

		[GameParameters.CustomParameterUI (/*Detect Depth*/"#autoLOC_CL_0007", toolTip = "#autoLOC_CL_0008")]
		public bool useDepthLight = true;

		[GameParameters.CustomIntParameterUI (/*Depth Threshold*/"#autoLOC_CL_0009", minValue = 2, maxValue = 200, displayFormat = "0m")]
		public int depthThreshold = 20;

		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy1 = "";

		[GameParameters.CustomParameterUI (/*Staggered Lightning*/"#autoLOC_CL_0010", toolTip = "#autoLOC_CL_0011")]
		public bool useStaggeredLight = true;

		[GameParameters.CustomParameterUI (/*Random Delay*/"#autoLOC_CL_0013")]
		public bool useRandomDelay = true;

		[GameParameters.CustomFloatParameterUI (/*Delay between stage*/"#autoLOC_CL_0012", minValue = .1f, maxValue = 10f, logBase = .01f, stepCount = 100, displayFormat = "0.0")]
		public float delayStage = 1.5f;

		[GameParameters.CustomIntParameterUI (/*Max Light per Stage*/"#autoLOC_CL_0014", minValue = 1, maxValue = 100)]
		public int maxLightPerStage = 6;

		[GameParameters.CustomIntParameterUI (/*Min Light per Stage*/"#autoLOC_CL_0015", minValue = 1, maxValue = 100)]
		public int minLightPerStage = 2;

		[GameParameters.CustomStringParameterUI ("\n\0", autoPersistance = false)]
		public string dummy2 = "";

		[GameParameters.CustomFloatParameterUI (/*Update in Low TimeWarp*/"#autoLOC_CL_0016", toolTip = "#autoLOC_CL_0017", minValue = .1f, maxValue = 10f, logBase = .01f, stepCount = 100, displayFormat = "0.0")]
		public float delayLowTimeWarp = 2f;

		[GameParameters.CustomFloatParameterUI (/*Update in High TimeWarp*/"#autoLOC_CL_0018", toolTip = "#autoLOC_CL_0019", minValue = .1f, maxValue = 10f, logBase = .01f, stepCount = 100, displayFormat = "0.0")]
		public float delayHighTimeWarp = .1f;


		public override bool Interactible (System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (!useSunLight) {
				if (member.Name != "useSunLight") {
					return false;
				}
			}

			if (member.Name == "depthThreshold") {
				if (useDepthLight) {
					return true;
				} else {
					return false;
				}
			}

			if (member.Name == "delayStage") {
				if (useStaggeredLight && !useRandomDelay) {
					return true;
				} else {
					return false;
				}
			}

			if (member.Name == "useRandomDelay" || member.Name == "maxLightPerStage" || member.Name == "minLightPerStage") {
				if (useStaggeredLight) {
					return true;
				} else {
					return false;
				}
			}
			return base.Interactible (member, parameters);
		}
	}


	public class CL_AviationLightsSettings : GameParameters.CustomParameterNode
	{
		public override string Title {
			get {
				return /*AviationLights effects*/Localizer.Format ("#autoLOC_CL_0060");
			}
		}

		public override string Section {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override string DisplaySection {
			get {
				return /*Crew Light*/Localizer.Format ("#autoLOC_CL_0001");
			}
		}

		public override int SectionOrder {
			get {
				return 3;
			}
		}

		public override GameParameters.GameMode GameMode {
			get {
				return GameParameters.GameMode.ANY;
			}
		}

		public override bool HasPresets {
			get {
				return false;
			}
		}

		[GameParameters.CustomParameterUI (/*Beacon light sync to engine*/"#autoLOC_CL_0061", toolTip = "#autoLOC_CL_0062")]
		public bool beaconOnEngine = true;

		[GameParameters.CustomParameterUI (/*Beacon amber*/"#autoLOC_CL_0076")]
		public string beaconAmber = "#autoLOC_CL_0064";

		[GameParameters.CustomParameterUI (/*Beacon red*/"#autoLOC_CL_0068")]
		public string beaconRed = "#autoLOC_CL_0064";

		[GameParameters.CustomParameterUI (/*Navigation blue*/"#autoLOC_CL_0069")]
		public string navBlue = "#autoLOC_CL_0067";

		[GameParameters.CustomParameterUI (/*Navigation green*/"#autoLOC_CL_0070")]
		public string navGreen = "#autoLOC_CL_0067";

		[GameParameters.CustomParameterUI (/*Navigation red*/"#autoLOC_CL_0071")]
		public string navRed = "#autoLOC_CL_0067";

		[GameParameters.CustomParameterUI (/*Navigation white*/"#autoLOC_CL_0072")]
		public string navWhite = "#autoLOC_CL_0067";

		[GameParameters.CustomParameterUI (/*Strobe white*/"#autoLOC_CL_0073")]
		public string strobeWhite = "#autoLOC_CL_0065";

		public override System.Collections.IList ValidValues (System.Reflection.MemberInfo member)
		{
			if (member.Name == "beaconAmber" || member.Name == "beaconRed" 
				|| member.Name == "navBlue" || member.Name == "navGreen" 
				|| member.Name == "navRed" || member.Name == "navWhite" 
				|| member.Name == "strobeWhite")
			{
				System.Collections.Generic.List<string> flashList = new System.Collections.Generic.List<string> ();
				flashList.Add (/*off*/Localizer.Format ("#autoLOC_CL_0063"));
				flashList.Add (/*flash*/Localizer.Format ("#autoLOC_CL_0064"));
				flashList.Add (/*double flash*/Localizer.Format ("#autoLOC_CL_0065"));
				flashList.Add (/*interval*/Localizer.Format ("#autoLOC_CL_0066"));
				flashList.Add (/*on*/Localizer.Format ("#autoLOC_CL_0067"));

				return flashList;
			} else {
				return null;
			}
		}

		public override bool Interactible (System.Reflection.MemberInfo member, GameParameters parameters)
		{
			if (parameters.CustomParams<CL_GeneralSettings> ().useAviationLightsEffect)
			{
				return true;
			} else {
				return false;
			}
		}
	}
}


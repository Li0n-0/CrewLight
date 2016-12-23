using System;
using UnityEngine;

namespace CrewLight
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class DisableLightAG : MonoBehaviour
	{
		private bool disableCrewAG;
		private bool disableAllAG;

		void Start ()
		{
			disableCrewAG = CLSettings.disableCrewAG;
			disableAllAG = CLSettings.disableAllAG;

			if (disableCrewAG || disableAllAG) {
				GameEvents.onEditorPartEvent.Add (CheckForLight);
			}
		}

		void OnDestroy ()
		{
			if (disableCrewAG || disableAllAG) {
				GameEvents.onEditorPartEvent.Remove (CheckForLight);
			}
		}

		void CheckForLight (ConstructionEventType constrE, Part part)
		{
			if (constrE == ConstructionEventType.PartCreated) {
				if (disableCrewAG) {
					if (part.CrewCapacity < 1) { return; }
				}
				if (part.Modules.Contains<ModuleColorChanger>() 
					|| part.Modules.Contains<ModuleLight>() 
					|| part.Modules.Contains<ModuleAnimateGeneric>() 
					|| part.Modules.Contains("WBILight"))
				{
					foreach (PartModule partM in part.Modules) {
						if (partM.Actions.Contains(KSPActionGroup.Light)) {
							foreach (BaseAction action in partM.Actions) {
								if (action.actionGroup == KSPActionGroup.Light) {
									action.actionGroup = KSPActionGroup.None;
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}
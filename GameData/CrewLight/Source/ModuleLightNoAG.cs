using System;
using UnityEngine;

namespace CrewLight
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class ModuleLihghtNoAG : MonoBehaviour
	{
		/*
		 * 
		 * Remove the light of crewable part from the light action group, 
		 * only for ModuleLight and WBILight (from Wild Blue Industries by Angel-125),
		 * ModuleAnimateGeneric and ModuleColorChanger get theirs action groups neutralize 
		 * with MM.
		 * 
		 */
		void Start ()
		{
			GameEvents.onEditorPartEvent.Add (CheckForLight);
		}

		void OnDestroy ()
		{
			GameEvents.onEditorPartEvent.Remove (CheckForLight);
		}

		void CheckForLight (ConstructionEventType constrE, Part part)
		{
			if (constrE == ConstructionEventType.PartCreated) {
				if (part.CrewCapacity > 0) {
					if (part.Modules.Contains<ModuleLight>() || part.Modules.Contains("WBILight")) {
						/* Other light modules are neutralize with MM */
						foreach (PartModule partM in part.Modules) {
							if (partM.Actions.Contains(KSPActionGroup.Light)) {
								foreach (BaseAction action in partM.Actions) {
									if (action.actionGroup == KSPActionGroup.Light) {
										action.actionGroup = KSPActionGroup.None;
										return;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
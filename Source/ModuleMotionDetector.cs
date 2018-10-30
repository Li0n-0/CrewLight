using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public class ModuleMotionDetector : PartModule
	{
		#region KSPField
		[KSPField (isPersistant = true)]
		public bool motionDetectorEnabled;
		[KSPField (isPersistant = true)]
		public float range;
		[KSPField (isPersistant = true)]
		public float timer;
		[KSPField (isPersistant = true)]
		public bool applyToSymPart;
		#endregion

		#region Internal
		public float rangeNew, timerNew;
		public bool applyToSymPartNew;

		private bool inTheEditor;
		private bool useOffset;

		private bool lightIsOn;
		private List<PartModule> lights;
		private List<Part> symParts;

		private GameObject detectionCone;
		private LogicMotionDetector logicModule;

		private bool showGUI;
		private Rect windowRect;
		#endregion

		public override void OnStart (StartState state)
		{
			if ((!HighLogic.CurrentGame.Parameters.CustomParams<CL_GeneralSettings> ().useMotionDetector)/* || (!part.Modules.Contains<ModuleLight> ())*/) {
				Events ["SetMotionDetector"].active = false;
//				Destroy (this);
				this.enabled = false;
				return;
			}

			inTheEditor = false;
			if (state == StartState.Editor) {
				inTheEditor = true;
			}

			useOffset = false;
			if (part.name == "spotLight1" || part.name == "spotLight2") {
				useOffset = true;
			}

			showGUI = false;
			windowRect = new Rect (0, 0, 120f, 180f);

			symParts = new List<Part> (part.symmetryCounterparts);
			part.symmetryCounterparts.Clear ();
			lights = new List<PartModule> (SwitchLight.GetLightModule (part));
			lightIsOn = SwitchLight.IsOn (lights);
			part.symmetryCounterparts = symParts;

			// Create the cone :
			detectionCone = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			detectionCone.layer = 1;
			detectionCone.transform.SetParent (lights[0].transform);
			detectionCone.transform.position = lights[0].transform.position;

			Color coneColor = Color.yellow;
			coneColor.a = .3f;

			MeshRenderer coneRenderer = detectionCone.GetComponent<MeshRenderer> ();
			coneRenderer.material = new Material (Shader.Find ("Transparent/Diffuse"));
			coneRenderer.material.color = coneColor;

			detectionCone.GetComponent<SphereCollider> ().isTrigger = true;
			ToggleMeshRenderer (false);
			if (inTheEditor) {
				detectionCone.GetComponent<SphereCollider> ().enabled = false;
				// dont think this is needed anymore, due to the change of layer
			} else {
				logicModule = detectionCone.AddComponent<LogicMotionDetector> ();
				//lightIsOn = SwitchLight.IsOn (lights);
			}

			rangeNew = range;
			timerNew = timer;
			applyToSymPartNew = applyToSymPart;
			ResetGUIName ();
			UpdateMeshScale ();
			ResetSymmetry ();

			GameEvents.onEditorSymmetryModeChange.Add (ResetSymmetry);
			GameEvents.onEditorPartEvent.Add (ResetSymmetry);
		}

		void OnDestroy ()
		{
			Destroy (detectionCone);
			GameEvents.onEditorSymmetryModeChange.Remove (ResetSymmetry);
			GameEvents.onEditorPartEvent.Remove (ResetSymmetry);
		}

		private void ResetSymmetry (int i = 0)
		{
			symParts = new List<Part> (part.symmetryCounterparts);
		}

		private void ResetSymmetry (ConstructionEventType constrE, Part p)
		{
			if (constrE != ConstructionEventType.PartDeleted && p == part) {
				ResetSymmetry ();
			}
		}

		public void UpdateMeshScale ()
		{
			detectionCone.transform.localScale = new Vector3 (range, range, range);

			// Offset the sphere from the part
			if (useOffset && lights.Count == 1) {
				// I won't be able to offset in the right direction if there is more than one light source
				float radius = detectionCone.transform.localScale.z / 2f;
				float offset = radius - (radius / 10f);
				detectionCone.transform.rotation = part.transform.rotation;
				detectionCone.transform.localPosition = Vector3.zero;
				detectionCone.transform.Translate (0, -offset, 0);
			}
		}

		private void SetMotionDetectorEnabled (bool enableValue)
		{
			motionDetectorEnabled = enableValue;
			ResetGUIName ();
			UpdateSymParts ();
		}

		private void SetRange (float rangeValue)
		{
			if (detectionCone != null) {
				range = rangeValue;
				UpdateMeshScale ();
			}
			UpdateSymParts ();
		}

		private void SetTimer (float timerValue)
		{
			timer = timerValue;
			UpdateSymParts ();
		}

		private void SetApplyToSymPart (bool applyToSymPartValue)
		{
			applyToSymPart = applyToSymPartValue;
			UpdateSymParts ();
		}

		private void UpdateSymParts ()
		{
			foreach (Part p in part.symmetryCounterparts) {
				ModuleMotionDetector m = p.Modules.GetModule<ModuleMotionDetector> ();
				if (applyToSymPart) {
					m.motionDetectorEnabled = motionDetectorEnabled;
					m.range = range;
					m.rangeNew = range;
					m.timer = timer;
					m.timerNew = timer;
					m.UpdateMeshScale ();
					m.ResetGUIName ();
				}
				m.applyToSymPart = applyToSymPart;
				m.applyToSymPartNew = applyToSymPart;
			}
		}

		public override void OnUpdate ()
		{
			// Logic stuff :
			if (motionDetectorEnabled) {
				if (lightIsOn != logicModule.lightToggle) {
					symParts = new List<Part> (part.symmetryCounterparts);
					part.symmetryCounterparts.Clear ();
					if (lightIsOn) {
						SwitchLight.Off (lights);
						lightIsOn = false;
					} else {
						SwitchLight.On (lights);
						lightIsOn = true;
					}
					part.symmetryCounterparts = symParts;
				}
			}
		}

		void Update ()
		{
			// GUI Stuff, OnUpdate run only in flight but the GUI is needed all times
			UpdateGUI ();
		}
		#region GUI
		[KSPEvent (active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = true, guiName = "Motion Detector")]
		public void SetMotionDetector ()
		{
			if (showGUI) {
				showGUI = false;
			} else {
				windowRect.position = new Vector2 (Mouse.screenPos.x - windowRect.width/* + 12f*/, Mouse.screenPos.y - windowRect.height / 2);
				showGUI = true;
			}
		}

		public void ResetGUIName ()
		{
			if (motionDetectorEnabled) {
				Events ["SetMotionDetector"].guiName = "Motion Detector : ON";
			} else {
				Events ["SetMotionDetector"].guiName = "Motion Detector : OFF";
			}
		}

		public void OnGUI ()
		{
			GUI.skin = HighLogic.Skin;
			GUI.skin.toggle.margin = new RectOffset (0, 0, 0, 0);

			if (showGUI) {
				GUILayout.BeginArea (windowRect);
				windowRect = GUILayout.Window (837190, windowRect, MotionDetectorWindow, "Motion Detector");
				GUILayout.EndArea ();
			}
		}

		public void MotionDetectorWindow (int id)
		{
			if (GUI.Button (new Rect (windowRect.size.x - 22, 2, 20, 20), "X")) {
				CloseGUI ();
			}

			GUILayout.BeginVertical ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (5f);
			if (GUILayout.Button ("ON")) {
				SetMotionDetectorEnabled (true);
				ToggleMeshRenderer (true);
				Events ["SetMotionDetector"].guiName = "Motion Detector : ON";
			}
			GUILayout.Space (15f);
			if (GUILayout.Button ("OFF")) {
				SetMotionDetectorEnabled (false);
				ToggleMeshRenderer (false);
				Events ["SetMotionDetector"].guiName = "Motion Detector : OFF";
			}
			GUILayout.Space (5f);
			GUILayout.EndHorizontal ();

			GUILayout.Label ("Range : " + rangeNew.ToString ("n0") + "m");
			rangeNew = GUILayout.HorizontalSlider (rangeNew, 1f, 25f);

			GUILayout.Label ("Timer : " + timerNew.ToString ("n0") + "s");
			timerNew = GUILayout.HorizontalSlider (timerNew, 1f, 60f);

			applyToSymPartNew = GUILayout.Toggle (applyToSymPartNew, "Symmetry");

			GUILayout.EndVertical ();
		}

		private void UpdateGUI ()
		{
			// checking value :
			if (rangeNew != range) {
				SetRange (rangeNew);
			}
			if (timerNew != timer) {
				SetTimer (timerNew);
			}
			if (applyToSymPartNew != applyToSymPart) {
				SetApplyToSymPart (applyToSymPartNew);
			}

			if (showGUI) {
				if (motionDetectorEnabled) {
					ToggleMeshRenderer (true);
				}

				// Input Lock :
				if (windowRect.Contains (Mouse.screenPos)) {
					InputLockManager.SetControlLock (
						ControlTypes.UI | ControlTypes.EDITOR_PAD_PICK_PLACE, "CrewLight_InputLock");
				} else {
					InputLockManager.RemoveControlLock ("CrewLight_InputLock");
				}

				// Close the window when right-click or left-click outside of it :
				if (Input.GetMouseButtonDown (1) ||
					(Input.GetMouseButtonDown (0) && !windowRect.Contains (Mouse.screenPos))) {
					CloseGUI ();
				}
			} else {
				ToggleMeshRenderer (false);
				InputLockManager.RemoveControlLock ("CrewLight_InputLock");
			}
		}

		private void CloseGUI ()
		{
			showGUI = false;
			ToggleMeshRenderer (false);
		}

		private void ToggleMeshRenderer (bool enable)
		{
			if (enable) {
				detectionCone.GetComponent<MeshRenderer> ().enabled = true;
			} else {
				detectionCone.GetComponent<MeshRenderer> ().enabled = false;
			}
		}
		#endregion
	}

	public class LogicMotionDetector : MonoBehaviour
	{
		private ModuleMotionDetector module;

		public bool lightToggle = false;

		void Start ()
		{
			module = this.GetComponentInParent<ModuleMotionDetector> ();
		}

		void OnDestroy ()
		{
			StopAllCoroutines ();
		}

		void OnTriggerEnter (Collider collider)
		{
			if (ColliderIsEVA (collider)) {
				lightToggle = true;
			}
		}

		void OnTriggerExit (Collider collider)
		{
			if (ColliderIsEVA (collider)) {
				StartCoroutine ("Countdown");
			}
		}

		void OnTriggerStay (Collider collider)
		{
			if (ColliderIsEVA (collider)) {
				lightToggle = true;
				StopCoroutine ("Countdown");
				StartCoroutine ("Countdown");
			}
		}

		private IEnumerator Countdown ()
		{
			float waitFor = module.timer;
			while (waitFor > 0) {
				yield return new WaitForSeconds (.5f);
				waitFor -= .5f;
			}
			lightToggle = false;
		}

		private bool ColliderIsEVA (Collider collider)
		{
			Vessel v = collider.GetComponentInParent<Vessel> ();
			if (v != null && v.isEVA) {
				return true;
			} else {
				return false;
			}
		}
	}
}


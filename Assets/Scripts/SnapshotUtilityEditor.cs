using UnityEditor;
using UnityEngine;

namespace InTheShadow.Editor
{
	// Custom Editor using SerializedProperties.
	// Automatic handling of multi-object editing, undo, and Prefab overrides.
	[CustomEditor(typeof(SnapShotComparasion))]
	public class SnapshotUtilityEditor : UnityEditor.Editor
	{
		private SerializedProperty _projectorCameraProp;
		private SerializedProperty _shadowCasterGroupProp;
		private SerializedProperty _comparisonShaderProp;
		private SerializedProperty _rotateShaderProp;
		private SerializedProperty _startDegreeProp;
		private SerializedProperty _endDegreeProp;
		private SerializedProperty _stepDegreeProp;

		void OnEnable()
		{
			// Setup the SerializedProperties.
			_projectorCameraProp = serializedObject.FindProperty("projectorCamera");
			_shadowCasterGroupProp = serializedObject.FindProperty("shadowCasterGroup");
			_comparisonShaderProp = serializedObject.FindProperty("comparisonShader");
			_rotateShaderProp = serializedObject.FindProperty("rotateShader");
			_startDegreeProp = serializedObject.FindProperty("startDegree");
			_endDegreeProp = serializedObject.FindProperty("endDegree");
			_stepDegreeProp = serializedObject.FindProperty("stepDegree");
		}
		
		public override void OnInspectorGUI()
		{
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObject.Update ();
			
			GUILayout.Label ("Save", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (_projectorCameraProp, new GUIContent ("Projector Camera"));
			//EditorGUILayout.PropertyField (_shadowCasterGroupProp, new GUIContent ("Shadow Casters"));
			
			EditorGUILayout.PropertyField (_rotateShaderProp, new GUIContent ("Rotate shader"));
			
			EditorGUILayout.PropertyField(_startDegreeProp, new GUIContent ("Start degree"));
			EditorGUILayout.PropertyField (_stepDegreeProp, new GUIContent ("Degree step"));
			EditorGUILayout.PropertyField (_endDegreeProp, new GUIContent ("End degree"));
			
			if (GUILayout.Button("Save"))
			{
				(target as SnapShotComparasion)?.MakeSnapshot();
			}

			GUILayout.Space(10.0f);
			GUILayout.Label("Comparison", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (_comparisonShaderProp, new GUIContent ("Comparison shader"));
			
			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
using UnityEditor;
using UnityEngine;

namespace Pepperon.Scripts.EditorExtensions.Attributes {
public class ConditionalDisplayAttribute : PropertyAttribute {
    public string ConditionalField { get; }

    public ConditionalDisplayAttribute(string conditionalField) {
        ConditionalField = conditionalField;
    }
}

[CustomPropertyDrawer(typeof(ConditionalDisplayAttribute))]
public class ConditionalDisplayDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        ConditionalDisplayAttribute conditional = (ConditionalDisplayAttribute)attribute;

        // Find the conditional field
        SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditional.ConditionalField);

        if (conditionalProperty != null && conditionalProperty.propertyType == SerializedPropertyType.Boolean) {
            // Only display the field if the conditional property is true
            if (conditionalProperty.boolValue) {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
        else {
            // Display a warning if the conditional field is not found or isn't a boolean
            EditorGUI.HelpBox(position,
                $"Conditional field '{conditional.ConditionalField}' is not a boolean or does not exist.",
                MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        ConditionalDisplayAttribute conditional = (ConditionalDisplayAttribute)attribute;
        SerializedProperty conditionalProperty = property.serializedObject.FindProperty(conditional.ConditionalField);

        if (conditionalProperty != null && conditionalProperty.propertyType == SerializedPropertyType.Boolean) {
            // Show field height only if the conditional property is true
            return conditionalProperty.boolValue ? EditorGUI.GetPropertyHeight(property, label, true) : 0f;
        }

        // Default height for warning message
        return EditorGUIUtility.singleLineHeight;
    }
}
}
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomPropertyDrawer(typeof(AttackStepParametersBase), true)]
public class SerializeReferenceDropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var baseType = typeof(AttackStepParametersBase);
        var types = TypeCache.GetTypesDerivedFrom(baseType)
            .Where(t => !t.IsAbstract)
            .ToArray();

        var typeNames = types.Select(t => t.Name).Prepend("None").ToArray();
        var currentType = property.managedReferenceValue?.GetType();
        var currentIndex = currentType != null ? Array.IndexOf(types, currentType) + 1 : 0;

        // Draw the type selector dropdown
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var popupRect = new Rect(position.x, position.y, position.width, lineHeight);
        
        EditorGUI.BeginChangeCheck();
        var newIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, typeNames);
        if (EditorGUI.EndChangeCheck())
        {
            property.managedReferenceValue = newIndex > 0 ? Activator.CreateInstance(types[newIndex - 1]) : null;
            property.serializedObject.ApplyModifiedProperties();
        }

        // Draw the nested fields using default Unity rendering
        if (property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            
            var childPosition = new Rect( 
                position.x, 
                position.y + lineHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width, 
                position.height - lineHeight - EditorGUIUtility.standardVerticalSpacing
            );

            // Draw all children with includeChildren = true to handle nesting
            var prop = property.Copy();
            var endProperty = prop.GetEndProperty();
            
            prop.NextVisible(true); // Enter children
            float yOffset = 0;
            
            do
            {
                if (SerializedProperty.EqualContents(prop, endProperty))
                    break;

                var height = EditorGUI.GetPropertyHeight(prop, true);
                var rect = new Rect(childPosition.x, childPosition.y + yOffset, childPosition.width, height);
                
                // includeChildren = true is KEY for nested objects like GridPosition
                EditorGUI.PropertyField(rect, prop, true);
                
                yOffset += height + EditorGUIUtility.standardVerticalSpacing;
            }
            while (prop.NextVisible(false)); // Don't enter children again, we're iterating siblings
            
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight;
        
        if (property.managedReferenceValue != null)
        {
            height += EditorGUIUtility.standardVerticalSpacing;
            
            var prop = property.Copy();
            var endProperty = prop.GetEndProperty();
            
            prop.NextVisible(true); // Enter children
            
            do
            {
                if (SerializedProperty.EqualContents(prop, endProperty))
                    break;
                    
                // includeChildren = true to get full height of nested objects
                height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
            }
            while (prop.NextVisible(false));
        }
        
        return height;
    }
}
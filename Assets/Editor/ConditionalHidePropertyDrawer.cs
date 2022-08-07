using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //get the attribute data
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        //check if the propery we want to draw should be enabled
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        //Enable/disable the property
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;

        //Check if we should draw the property
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        //Ensure that the next property that is being drawn uses the correct settings
        GUI.enabled = wasEnabled;
    }


    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = false;
        //Look for the sourcefield within the object that the property belongs to
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionPath);

        if (conditionProperty != null)
        {
            string sourcePropertyType = conditionProperty.type;

            //check only allows for bools and enums
            if (sourcePropertyType == "bool")
            {
                if (bool.TryParse(condHAtt.ConditionalValue, out bool value))
                    //string parsed to bool, check values match
                    enabled = value == conditionProperty.boolValue;
                else
                    Debug.LogWarning(condHAtt.ConditionalValue + " could not be parsed into a bool");
            }
            else if (sourcePropertyType == "Enum")
            {
                enabled = conditionProperty.enumNames[conditionProperty.enumValueIndex] == condHAtt.ConditionalValue;
            }
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField + " " + propertyPath);
        }

        return enabled;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}

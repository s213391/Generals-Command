using System;
using System.Collections;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |  AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    //The name of the field that will be in control
    public string ConditionalSourceField = "";
    //the value that the field must match to show
    public string ConditionalValue = "";
    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public bool HideInInspector = false;


    public ConditionalHideAttribute(string conditionalSourceField, string requiredValue, bool hideInInspector = true)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ConditionalValue = requiredValue;
        this.HideInInspector = hideInInspector;
    }
}

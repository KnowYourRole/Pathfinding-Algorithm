using UnityEngine;
using UnityEditor;

//Holds the UI properties
[CustomPropertyDrawer (typeof(Attributes))]
internal sealed class Drawers : PropertyDrawer
{

	public override void OnGUI (Rect transLocation, SerializedProperty aspect, GUIContent symbol)   //general attributesin the UI construct
	{
		var rangeAttribute = (Attributes)base.attribute;

		if (aspect.propertyType == SerializedPropertyType.Integer)

		{
            attribute = EditorGUI.IntSlider (transLocation, symbol, attribute, rangeAttribute.lowest, rangeAttribute.highest);

            attribute = (attribute / rangeAttribute.stage) * rangeAttribute.stage;
            aspect.intValue = attribute;
		}			
		
	}
    private int attribute;

}
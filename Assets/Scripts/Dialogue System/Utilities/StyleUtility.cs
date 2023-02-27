using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Utilities
{    
    public static class StyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames) //����� �������� ��������� ����� � ����������� ������
                                                                                                       //(�.�. ����� �� ���� dialogueNameTextField.AddToClassList("mary-node__textfield");)
        {
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }
            return element;
        }
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames) //params ��������, ��� � ������� ����� ���� ����� ���-�� ��������� (���� ����, ���� ����)
        {
            foreach(var styleSheetName in styleSheetNames) //��� ������� �������� ��������� �����
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName); //����� ��� ���
                element.styleSheets.Add(styleSheet); //� ���� ������ �������� ���� �����
            }
            return element;
        }
    }
}

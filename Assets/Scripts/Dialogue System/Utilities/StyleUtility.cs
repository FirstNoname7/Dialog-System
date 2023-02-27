using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Utilities
{    
    public static class StyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames) //чтобы заменить множество строк с добавлением стилей
                                                                                                       //(т.е. строк по типу dialogueNameTextField.AddToClassList("mary-node__textfield");)
        {
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }
            return element;
        }
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames) //params означает, что в массиве может быть любое кол-во элементов (даже один, если надо)
        {
            foreach(var styleSheetName in styleSheetNames) //для каждого элемента добавляем стиль
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName); //стиль для нод
                element.styleSheets.Add(styleSheet); //к фону окошка добавляю свой стиль
            }
            return element;
        }
    }
}


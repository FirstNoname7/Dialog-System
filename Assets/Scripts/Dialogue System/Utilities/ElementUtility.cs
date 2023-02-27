using MaryDialogSystem.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Utilities
{
    public static class ElementUtility
    {
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null) //value = как будет называться текстовое поле,
                                                                                                                               //onValueChanged = действие при изменении события
                                                                                                                               //тут создаем сами кнопки выбора
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            }; //Создаю переменную-пустышку, в текстовое поле которой ставлю значение из параметров value и label
            if (onValueChanged != null) //если значение как-то меняется в текстовом поле, то:
            {
                textField.RegisterValueChangedCallback(onValueChanged); //регистрируем это изменение 
            }
            return textField; //возвращаем текстовое поле
        }

        public static TextField CreateTextArea(string value = null, string label = null,  EventCallback<ChangeEvent<string>> onValueChanged = null) //тут создаем пространство для кнопок выбора
                                                                                                                              //(делаем так, чтоб можно было несколько кнопок выбора создать)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true; //чтобы можно было добавить несколько строк (для нескольких кнопок выбора)
            return textArea;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false) //для создания выпадающих функций (у нас фраза выпадает как подпапка в ноде) 
                                                                          //title = заголовок, collapse = проверка, раскрыта ли сейчас эта подпапка
        {
            Foldout foldOut = new Foldout()
            {
                text = title,
                value = !collapsed
            }; //вкладываем в пустышку-конструктор значения заголовка и проверяем, открыта ли эта подпапка.
               //Если открыта - тогда закрываем, если закрыта - тогда открываем
            return foldOut;
        }

        public static Button CreateButton(string name, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = name
            };
            return button;
        }

        public static Port CreatePort(this MyNode node, string portName = "", Orientation orientation = Orientation.Vertical, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
            //создаем как input, так и output порты, т.к. в параметры можно любые значения передать. this в первом параметре означает, что даем порты текущей ноде
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool)); //создаем порт
            port.portName = portName; //даем имя порту
            return port; //возвращаем созданный порт
        }
    }
}


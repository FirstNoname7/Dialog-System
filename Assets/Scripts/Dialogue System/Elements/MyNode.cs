using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using UnityEngine.UIElements;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using System;
using MaryDialogSystem.Data.Save;
using System.Linq;

namespace MaryDialogSystem.Elements
{
    public class MyNode : Node //отвечает за создание нод в окошке
    {
        //{ get; set; } у переменных для того, чтобы можно было посмотреть, где ссылаемся на эти переменные (если уберешь, то над переменными пропадет надпись "ссылок:кол-во")
        public string ID { get; set; } //идентификатор ноды (нужно для сохраняшки)
        public string dialogueName { get; set; } //имя того, кто говорит
        public List<ChoiceSaveData> choices { get; set; } //список с названиями для кнопок
        public string text { get; set; } //текст в диалоговом окне
        public DialogueType dialogueType { get; set; } //тип диалога (просто текст (SingleChoice) или кнопки выбора (MultipleChoice))

        protected MyGraphView myGraphView; //даю дочерним скриптам доступ к этой переменной через модификатор protected
        private Color defaultBackgroundColor; //стандартный цвет ноды
        public virtual void Initialize(MyGraphView currentGraphView, Vector2 position) //ставим дефолтные значения переменных
        {
            ID = Guid.NewGuid().ToString(); //инициализируем текущий идентификатор ноды (команда Guid.NewGuid() создает ID автоматически за нас) и преобразуем его в строчку, чтоб потом использовать (ToString())
            myGraphView = currentGraphView; //инициализируем окно переговорной
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f); //ставим дефолтный цвет ноды
            dialogueName = "Я";
            choices = new List<ChoiceSaveData>();
            text = "Пиши фразу сюды";
            SetPosition(new Rect(position,Vector2.zero)); //настраиваем позицию ноды, которую создаём, чтоб они не создавались на одном и том же месте
            extensionContainer.AddToClassList("mary-node__extension-container"); //добавляю стиль mary-node__extension-container из Style Sheet
            mainContainer.AddToClassList("mary-node__main-container");
        }

        public virtual void Draw() //рисует UI для переменных, которые есть в этом скрипте
        {
            //заголовок ноды
            TextField dialogueNameTextField = ElementUtility.CreateTextField(dialogueName, null, callback => 
            {
                TextField target = (TextField)callback.target; //возвращаем текстовое поле, чтобы в нём можно было ремувнуть пробелы
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters(); //удаляем пробелы и спец. символы с помощью методов из скрипта TextUtility

                if (string.IsNullOrEmpty(target.value)) //если пустое текстовое поле, то:
                {
                    if (!string.IsNullOrEmpty(dialogueName)) //если есть что-то в dialogueName (старое имя ноды), то:
                    {
                        ++myGraphView.RepeatedNamesAmount; //ссылаемся на RepeatedNamesAmount, где выводится ошибка. Делаем инкремент, т.к. dialogueName = старое имя ноды, а нам надо сказать об ошибке в текущей ноде, которая с пустым текстовым полем
                    }
                }
                else //в обратном случае (если текстовое поле не пусто)
                {
                    --myGraphView.RepeatedNamesAmount; //идём к старой ноде
                }

                myGraphView.RemoveNode(this); //удаляю старую ноду (со старым названием)
                dialogueName = target.value; //устанавливаю новое значение названия ноды (с удалением лишних пробелов)
                myGraphView.AddNode(this); //добавляю ноду  с новым названием
            
            }); //помещаю переменную dialogueName в UI. callback удаляет стиль текущей ноды и ее значение в словаре, происходит это когда мы меняем имя ноды

            dialogueNameTextField.AddClasses("mary-node__textfield", "mary-node__textfield", "mary-node__textfield__hidden"); //добавляю стиль текстовому полю

            titleContainer.Insert(0, dialogueNameTextField); //вывожу переменную dialogueName как заголовок ноды

            //входной параметр ноды (input)
            Port inputPort = ElementUtility.CreatePort(this, "Подключай сюда предыдущую фразу", Orientation.Vertical, Direction.Input, Port.Capacity.Multi);
            //ввод подробностей для проводков (портов), входящих в ноды:  Orientation.Horizontal - провода будут идти горизонтально, Direction.Input - провод будет принимать значения как входной параметр,
            //Port.Capacity.Multi - ко входу ноды могут подключаться несколько проводков, typeof(bool) - тип ноды
            inputContainer.Add(inputPort); //добавляем во вход ноды параметры, которые указали в переменной inputPort

            //наполнение ноды (всякие кастомные штучки)
            Foldout textFlodout = ElementUtility.CreateFoldout("Фраза"); //заголовок для диалогового окна (по клику на который откроется фраза)

            TextField textTextField = ElementUtility.CreateTextArea(text, null, callback=> 
            {
                text = callback.newValue;
            }); //окошко, где можно писать фразу диалога + обратный вызов, чтоб появлялся актуальный текст, если он поменялся
            textTextField.AddClasses("mary-node__textfield", "mary-node__quote-textfield"); //добавляю стиль текстовому полю, где надо фразу писать

            textFlodout.Add(textTextField); //к заголовку добавляем поле для ввода фраз диалога 

            VisualElement customDataContainer = new VisualElement(); //создаем визуальный элемент в UI переговорной (наполнение для ноды)
            customDataContainer.AddToClassList("mary-node__custom-data-container"); //стиль для поля рядом с фразой
            customDataContainer.Add(textFlodout); //наполняем ноду элементами
            extensionContainer.Add(customDataContainer); //расширяем стандартный контейнер (добавляем поле "Фраза")

        }

        #region Utility Methods
        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer); //inputContainer - стандартная перменная, отвечающая за входные проводки у ноды в шейдер графе
            DisconnectPorts(outputContainer); //inputContainer - стандартная перменная, отвечающая за выходные проводки у ноды в шейдер графе
        }
        private void DisconnectPorts(VisualElement container) //для удаления входныз/выходных проводков
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected) //если порты (проводки) откреплены (connected - это стандартная переменная из шейдер графа), то
                {
                    continue; //идем дальше
                }
                myGraphView.DeleteElements(port.connections); //удоляем открепленные порты (проводки) (connections - это стандартная переменная из шейдер графа)
            }
        }

        public bool IsStartingNode() //проверяем, текущая нода первая или нет
        {
            Port inputPort = (Port)inputContainer.Children().First(); //входной порт = из контейнера берём первый дочерний объект (и конвертируем в Port, т.к. по стандарту такое выражение возвращает IEnumerable)
            return !inputPort.connected; //возвращает true - если есть пустой входной параметр, false - если входной параметр не пуст. Именно это выражение, т.к. есть нет коннекта (connected), то вход пуст
        }
        public void SetErrorStyle(Color color) //настроить цвет нод с ошибкой
        {
            mainContainer.style.backgroundColor = color; //какой цвет передаю в параметр, таким он и будет у ноды
        }

        public void ResetStyle() //стандартный стиль нод
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}

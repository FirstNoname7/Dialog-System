using MaryDialogSystem.Data.Save;
using MaryDialogSystem.Enumerations;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaryDialogSystem.Elements
{
    public class MultipleChoiceNode : MyNode //нода дл€ кнопок выбора
    {
        ChoiceSaveData choiceData = new ChoiceSaveData()
        {
            Text = "Ќова€ кнопка"
        }; //сохраненные данные, нам нужен в этом скрипте текст

        public override void Initialize(MyGraphView currentGraphView, Vector2 position)
        {
            base.Initialize(currentGraphView, position);
            dialogueType = DialogueType.MultipleChoice; //указываю тип создаваемого блока в переменной dialogueType

            choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = ElementUtility.CreateButton("ƒобавить кнопу", ()=>
            {
                Port choicePort = CreateChoicePort(choiceData); //добавл€ем порт к новой кнопке
                choices.Add(choiceData); //добавл€ем новую кнопку

                outputContainer.Add(choicePort);

            }); //создаю кнопку дл€ добавлени€ кнопок выбора + добавл€ю foreach, чтоб можно было более одной подобной кнопки создать
                //(ну и добавл€ю каждую созданную кнопку через outputContainer.Add(choicePort))

            addChoiceButton.AddToClassList("mary-node__button"); //добавл€ю стиль кнопкам
            mainContainer.Insert(1, addChoiceButton); //добавл€ю в ноду кнопку дл€ добавлени€ кнопок выбора

            //выходной параметр ноды (output) и кнопка "удалить"
            foreach (ChoiceSaveData choice in choices) //дл€ каждой кнопки в общем списке кнопок создаЄм порт на выход
            {
                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            }
            RefreshExpandedState(); //дл€ того, чтобы когда ты видишь ноду, то видишь открытыми все скрытые вкладки (в моем случае была скрыта вкладка choice)

        }
        #region Elements Creation
        private Port CreateChoicePort(object userData) //userData = зарезервированна€ переменна€, котора€ может хранить любые данные
        {
            Port choicePort = ElementUtility.CreatePort(this); //указываю только на конкретную ноду, т.к. остальные стандартные параметры соответствуют тому, что должно быть здесь
            choicePort.userData = userData; //данные в порте кнопки выбора берЄм из параметра
            ChoiceSaveData choiceData = (ChoiceSaveData) userData; //конвертируем данные из параметра userData в данные дл€ текущей кнопки
            Button deleteChoiceButton = ElementUtility.CreateButton("”долить", ()=> 
            {
                if (choices.Count == 1) //если сейчас остаЄтс€ 1 кнопка выбора в ноде дл€ кнопок выбора, то:
                {
                    return; //не даем пройти на логику ниже, т.к. мы не можем все кнопки выбора удалить под ноль
                }
                if (choicePort.connected) //если к порту ноды прикреплены проводки, то:
                {
                    myGraphView.DeleteElements(choicePort.connections); //удол€ем их
                }
                choices.Remove(choiceData); //удол€ем текущую кнопку и данные в ней
                myGraphView.RemoveElement(choicePort); //удол€ем порт
            }); //кнопка дл€ удалени€ кнопок выбора
            deleteChoiceButton.AddToClassList("mary-node__button"); //добавл€ю стиль кнопке "удалить"

            TextField choiceTextField = ElementUtility.CreateTextField(choiceData.Text, null, callback=> 
            {
                choiceData.Text = callback.newValue;
            }); //дл€ вывода кнопок на экран с конкретным текстом (choiceData.Text = callback.newValue - чтобы текст обновл€лс€ каждый раз, когда мы вводим его в кнопку выбора)
            choiceTextField.AddClasses("mary-node__textfield", "mary-node__choice-textfield", "mary-node__textfield__hidden"); //добавл€ю стиль текстовому полю кнопки


            choicePort.Add(deleteChoiceButton); //вывожу кнопку удалени€ кнопок на экран
            choicePort.Add(choiceTextField); //вывожу поле дл€ создани€ кнопок выбора
            return choicePort;
        }
        #endregion
    }
}

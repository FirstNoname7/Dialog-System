using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaryDialogSystem.Enumerations;
using UnityEditor.Experimental.GraphView;
using MaryDialogSystem.Utilities;
using MaryDialogSystem.Windows;
using MaryDialogSystem.Data.Save;

namespace MaryDialogSystem.Elements
{
    public class SingleChoiceNode : MyNode //нода для диалога
    {
        public override void Initialize(MyGraphView currentGraphView, Vector2 position)
        {
            base.Initialize(currentGraphView, position);
            dialogueType = DialogueType.SingleChoice; //указываю тип создаваемого блока в переменной dialogueType
            ChoiceSaveData choiceData = new ChoiceSaveData()
            {
                Text = "Следующая фраза"
            }; //сохраненные данные, нам нужен в этом скрипте текст
            choices.Add(choiceData); //добавляю к переменной для кнопок фразу
        }
        public override void Draw()
        {
            base.Draw();
            //выходной параметр ноды (output)
            foreach (ChoiceSaveData choice in choices) //для каждой кнопки в общем списке кнопок создаём порт на выход
            {
                Port choicePort = ElementUtility.CreatePort(this, choice.Text); //ставлю только choice, потому что остальные стандартные параметры как раз под output сделаны
                choicePort.userData = choice; //подгружаем данные каждой кнопки в userData
                outputContainer.Add(choicePort);
            }
            RefreshExpandedState(); //для того, чтобы когда ты видишь ноду, то видишь открытыми все скрытые вкладки (в моем случае была скрыта вкладка choice)

        }
    }
}

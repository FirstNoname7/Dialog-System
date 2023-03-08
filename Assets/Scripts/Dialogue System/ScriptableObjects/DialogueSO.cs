using MaryDialogSystem.DataForSave;
using MaryDialogSystem.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueSO : ScriptableObject //ScriptableObject для сохранения диалогов
    {
        //[field: SerializeField] - чтоб все переменные, которые здесь есть, выводились в инспекторе в SO
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; } //[field: TextArea()] чтобы видеть текст в нескольких строках
        [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialog { get; set; } //проверяет, эта нода = начало диалога или нет. Логика: если во входе нет проводка, значит это стартовая нода
        
        public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialog) //инициализируем все переменные, которые есть в этом скрипте
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialog = isStartingDialog;
        }
    }

}

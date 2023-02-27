using MaryDialogSystem.DataForSave;
using MaryDialogSystem.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueSO : ScriptableObject //ScriptableObject для сохранения диалогов
    {
        [SerializeField] public string DialogueName { get; set; }
        [SerializeField] [field: TextArea()] public string Text { get; set; } //[field: TextArea()] чтобы видеть текст в нескольких строках
        [SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [SerializeField] public DialogueType DialogueType { get; set; }
        [SerializeField] public bool IsStartingDialog { get; set; } //проверяет, эта нода = начало диалога или нет. Логика: если во входе нет проводка, значит это стартовая нода
        
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

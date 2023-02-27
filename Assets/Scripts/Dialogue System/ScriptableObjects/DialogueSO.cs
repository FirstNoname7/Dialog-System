using MaryDialogSystem.DataForSave;
using MaryDialogSystem.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueSO : ScriptableObject //ScriptableObject ��� ���������� ��������
    {
        [SerializeField] public string DialogueName { get; set; }
        [SerializeField] [field: TextArea()] public string Text { get; set; } //[field: TextArea()] ����� ������ ����� � ���������� �������
        [SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [SerializeField] public DialogueType DialogueType { get; set; }
        [SerializeField] public bool IsStartingDialog { get; set; } //���������, ��� ���� = ������ ������� ��� ���. ������: ���� �� ����� ��� ��������, ������ ��� ��������� ����
        
        public void Initialize(string dialogueName, string text, List<DialogueChoiceData> choices, DialogueType dialogueType, bool isStartingDialog) //�������������� ��� ����������, ������� ���� � ���� �������
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialog = isStartingDialog;
        }
    }

}

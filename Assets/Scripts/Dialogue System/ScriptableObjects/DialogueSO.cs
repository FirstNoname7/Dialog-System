using MaryDialogSystem.DataForSave;
using MaryDialogSystem.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueSO : ScriptableObject //ScriptableObject ��� ���������� ��������
    {
        //[field: SerializeField] - ���� ��� ����������, ������� ����� ����, ���������� � ���������� � SO
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; } //[field: TextArea()] ����� ������ ����� � ���������� �������
        [field: SerializeField] public List<DialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialog { get; set; } //���������, ��� ���� = ������ ������� ��� ���. ������: ���� �� ����� ��� ��������, ������ ��� ��������� ����
        
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

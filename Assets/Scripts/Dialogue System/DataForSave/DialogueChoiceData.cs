using MaryDialogSystem.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.DataForSave
{
    [Serializable]
    public class DialogueChoiceData //���������� ������ ������� ������ ������ � ���������� ����
    {
        [SerializeField] public string Text { get; set; }
        [SerializeField] public DialogueSO NextDialogue { get; set; }
    }

}

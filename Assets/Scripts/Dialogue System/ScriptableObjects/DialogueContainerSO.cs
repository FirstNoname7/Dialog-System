using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueContainerSO: ScriptableObject //��������� ��� ��� ��� ������� � ������ ������
    {
        //[field: SerializeField] - ���� ��� ����������, ������� ����� ����, ���������� � ���������� � SO
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DialogueSO> Dialogues { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Dialogues = new List<DialogueSO>();
        }
    }
}

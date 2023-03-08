using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.ScriptableObjects
{
    public class DialogueContainerSO: ScriptableObject //контейнер для нод для диалога и кнопок выбора
    {
        //[field: SerializeField] - чтоб все переменные, которые здесь есть, выводились в инспекторе в SO
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DialogueSO> Dialogues { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Dialogues = new List<DialogueSO>();
        }
    }
}

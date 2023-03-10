using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    [Serializable]
    public class ChoiceSaveData //сохраняшка для кнопок выбора
    {
        [field: SerializeField] public string Text { get; set; } //что за текст в этой ноде
        [field: SerializeField] public string NodeID { get; set; } //какую именно ноду сохранить
    }
}

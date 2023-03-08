using MaryDialogSystem.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Save
{
    [Serializable] //сохраняем данные из класса ниже
    public class NodeSaveData
    {
        //какие данные сохраняем:
        public string ID { get; set; } //какую именно ноду сохранить
        public string Name { get; set; } //имя ноды
        public string Text { get; set; } //текст в ноде
        public List<ChoiceSaveData> Choices { get; set; } //кнопки в ноде
        public DialogueType DialogueType { get; set; } //тип ноды (диалоговое окно или кнопки выбора)
        public Vector2 Position { get; set; } //позиция ноды
    }

}

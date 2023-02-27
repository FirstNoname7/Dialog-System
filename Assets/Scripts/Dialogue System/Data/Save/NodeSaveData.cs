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
        [SerializeField] private string ID { get; set; } //какую именно ноду сохранить
        [SerializeField] private string Name { get; set; } //имя ноды
        [SerializeField] private string Text { get; set; } //текст в ноде
        [SerializeField] private List<ChoiceSaveData> Choices { get; set; } //кнопки в ноде
        [SerializeField] private DialogueType DialogueType { get; set; } //тип ноды (диалоговое окно или кнопки выбора)
        [SerializeField] private Vector2 Position { get; set; } //позиция ноды
    }

}

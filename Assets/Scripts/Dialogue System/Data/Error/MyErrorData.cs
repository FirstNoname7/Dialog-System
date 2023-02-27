using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaryDialogSystem.Data.Error
{
    public class MyErrorData
    {
        public Color color { get; set; }
        public MyErrorData()
        {
            GenerateRandomColor();
        }
        private void GenerateRandomColor()
        {
            color = new Color32((byte)Random.Range(65, 256), (byte)Random.Range(50,176), (byte)Random.Range(50,176), 255); //указываю рандомные цвета по rgb каналам,
                                                                                                                           //по alpha ставлю 255, чтобы цвет всегда был видимым
        }
    }
}


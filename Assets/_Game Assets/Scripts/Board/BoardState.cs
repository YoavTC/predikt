using System;
using System.Collections.Generic;

[Serializable]
public class BoardState
{
    public Dictionary<Circle, Cell> circleCellDictionary = new Dictionary<Circle, Cell>();

    public BoardState(List<Circle> circles)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            circleCellDictionary.Add(circles[i], circles[i].currentCell);
        }
    }
}
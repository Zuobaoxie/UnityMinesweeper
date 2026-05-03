using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellEventData 
{
    public Cell[,] state;
    public CellForProps[,] propState;

    public CellEventData(Cell[,] state, CellForProps[,] propState)
    {
        this.state = state;
        this.propState = propState;
    }
}

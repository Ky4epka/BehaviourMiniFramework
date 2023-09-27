using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;

public class TestSingleCellController : ObjectBehavioursBase
{
    public BehaviourContainer Controlled = null;

    public void Update()
    {   
        if (Controlled == null)
            return;

        Vector2Int direction = Vector2Int.zero;

        if (Input.GetKey(KeyCode.UpArrow))
            direction = Vector2Int.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            direction = Vector2Int.down;
        else if (Input.GetKey(KeyCode.LeftArrow))
            direction = Vector2Int.left;
        else if (Input.GetKey(KeyCode.RightArrow))
            direction = Vector2Int.right;

        if (Input.GetKey(KeyCode.Return))
        {
            Controlled.SharedProperty<Main.Aggregator.Properties.Behaviours.Movable.TargetCellsStepsQueryProperty>().Value = 
                new Vector2Int[4] 
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, 0),
                };
        }

        if (direction != Vector2Int.zero)
            Controlled.SharedProperty<Main.Aggregator.Properties.Behaviours.Movable.DirectionStepToCellProperty>().Value = direction;
    }
}

using UnityEngine;

public class CellObject : MonoBehaviour
{
    protected Vector2Int _cellPosition;

    public virtual void Init(Vector2Int cell)
    {
        _cellPosition = cell;
    }

    //Called when the player enter the cell in which that object is
    public virtual void PlayerEntered() { }

    public virtual bool PlayerTryEnter() { return true; }
}
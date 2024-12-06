using UnityEngine;

public class FoodObject : CellObject
{
    [SerializeField] private int _foodAmount;

    public override void PlayerEntered(PlayerController playerController)
    {
        Destroy(gameObject);

        GameManager.Instance.UpdateFoodAmount(_foodAmount);
    }
}

using UnityEngine;

public class FoodObject : CellObject
{
    [SerializeField] private int _foodAmount;

    [Header("Sounds")]
    [SerializeField] private AudioClip _foodUp;

    public override void PlayerEntered(PlayerController playerController)
    {
        Destroy(gameObject);

        GameManager.Instance.SoundManager.PlaySound(_foodUp, true);

        GameManager.Instance.UpdateFoodAmount(_foodAmount);
    }
}

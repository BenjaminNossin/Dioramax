using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public partial class InteractableEntityLocal : InteractableEntity
{
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;

    public override void ChangeColor()
    {
        base.ChangeColor();
        winCondition.UpdateWinCondition(isActive);
    }

    public override void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        base.SwapOrChangeBack(swap, remoteChangeArray);
        winCondition.UpdateWinCondition(isActive);
    }
}

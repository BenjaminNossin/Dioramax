using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public partial class InteractableEntityLocal : InteractableEntity
{
    public bool isDecoy; 
    [Header("Gameplay")]
    [SerializeField] private WinCondition winCondition;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color badColor = Color.red;
    [SerializeField] private Color goodColor = Color.green;

    public override void ChangeColor()
    {
        base.ChangeColor();
        meshRenderer.material.color = Color.blue; // bad override

        if (isDecoy) return; 
        winCondition.UpdateWinCondition(isActive);
    }

    public override void SwapOrChangeBack(bool swap, int[] remoteChangeArray = null)
    {
        base.SwapOrChangeBack(swap, remoteChangeArray);

        if (isDecoy) return;
        winCondition.UpdateWinCondition(isActive);
    }
}

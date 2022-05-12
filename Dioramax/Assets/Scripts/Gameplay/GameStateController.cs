using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public void SetGameState(GameState gameState)
    {
        LevelManager.Instance.SetGameState(gameState);
    }
}

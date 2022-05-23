using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public void SetGameState(GameState gameState)
    {
        LevelManager.Instance.SetGameState(gameState);
    }

    private bool play;  
    public void SwitchBetweenPauseAndPlay()
    {
        LevelManager.Instance.SetGameState(play ? GameState.Playing : GameState.Paused);
        play = !play; 
    }
}

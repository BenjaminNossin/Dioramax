using System.Collections.Generic;
using UnityEngine;

public enum DioramaPuzzleName { NONE = -1, TutoButton, TutoBall, Tuyaux, Rats, Carrousel, Train, Tente, Mine }
public enum DioramaName { NONE = -1, Tutorial, Diorama1, Diorama2 }
[CreateAssetMenu(fileName = "New Diorama infos", menuName = "Dioravity/Diorama/Infos")]
public class DioramaInfos : ScriptableObject 
{
    public DioramaName dioramaName;

    [Space, TextArea(10, 20)] public string dioramaDescription;
    [Range(1, 5)] public int puzzleAmount = 3;
    public List<PuzzleInfos> puzzleInfos; 
}

[System.Serializable]
public class PuzzleInfos
{
    public DioramaPuzzleName puzzleName;
    public int entitiesAmount;
    public bool winConditionIsMet;
}
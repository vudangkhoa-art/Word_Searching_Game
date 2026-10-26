using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu]

public class GameData : ScriptableObject
{
    public string selectedCategoryName;
    public BoardData selectedBoardData;
     
}

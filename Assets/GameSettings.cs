using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / Game Settings", fileName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("人數")]
    public int peoplesV;
    [Header("籌碼")]
    public int chipsV;
    [Header("底注")]
    public int betsV;
}

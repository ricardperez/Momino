using UnityEngine;
using System.Collections;

public enum GameType
{
	kGameTypeMomino,
	kGameTypeGod,
}

public class GameProperties : MonoBehaviour
{
//	public static GameType gameType = GameType.kGameTypeGod;
	public static GameType gameType = GameType.kGameTypeMomino;
	public static bool paused = false;
	public static bool gameSuccess = false;
	public static int level = 0;
}

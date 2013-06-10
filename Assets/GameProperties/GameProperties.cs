using UnityEngine;
using System.Collections;

public enum GameType
{
	kGameTypeMomino,
	kGameTypeGod,
	kGameTypeMominoTargets,
}

public enum EditMode
{
	kEditModeDominos,
	kEditModePrefabs,
}

public class GameProperties : MonoBehaviour
{
//	public static GameType gameType = GameType.kGameTypeMominoTargets;
//	public static GameType gameType = GameType.kGameTypeGod;
	public static GameType gameType = GameType.kGameTypeMomino;
	public static bool paused = false;
	public static bool gameSuccess = false;
	public static int level = 0;
	public static EditMode editMode = EditMode.kEditModeDominos;
	
}

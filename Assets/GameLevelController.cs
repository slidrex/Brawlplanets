using UnityEngine;
using Mirror;

public static class GameLevelController
{
    public static string[] Nicknames = new string[]
    {
        "Fox5",
        "Mishka",
        "robot236",
        "ubiyca_porazheniy",
        "killer_laro",
        "saga_o_lirexe",
        "wolk",
        "limon_mikrofon",
        "lomatel_unichtozheniy",
        "sobaka_povodir",
        "kolchan_s_govnov",
        "govno_govna",
        "pirozhok_s_govnom"
    };
    public static PlayerEntity[] GetAllLevelPlayers() => MonoBehaviour.FindObjectsOfType<PlayerEntity>();
    [Command]
    public static void ReloadScene()
    {
        PlayerEntity[] players = GetAllLevelPlayers();
        foreach(PlayerEntity player in players) player.OnGameLoad();
    }
}

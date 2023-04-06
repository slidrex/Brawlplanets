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
        "pirozhok_s_govnom",
        "gonduras",
        "math_lier",
        "gleb",
        "rodion raskolnikov",
        "lomatel_surikat",
        "Subordinacia228"
    };
    public static PlayerEntity[] GetAllLevelPlayers() => MonoBehaviour.FindObjectsOfType<PlayerEntity>();
    public static GameObject GetObjectWithAuthority()
    {
        foreach(var player in NetworkServer.spawned.Values)
        {
            if(player.isOwned) return player.gameObject;
        }
        return null;
    }
}

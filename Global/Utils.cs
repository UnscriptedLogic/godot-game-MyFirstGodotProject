using Godot;
using Godot.Collections;
using System;

public partial class Utils : Node
{
    //Usually this should be "users/"
    public const string SAVE_PATH = "res://savegame.bin";

    public static void SaveGame()
    {
        GD.Print("Saving Game...");

        FileAccess fileAccess = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);

        fileAccess.Store8(Convert.ToByte(GameManager.PlayerHP));
        fileAccess.Store8(Convert.ToByte(GameManager.Gold));

        fileAccess.Close();
    }

    public static void LoadGame() 
    { 
        if (!FileAccess.FileExists(SAVE_PATH))
        {
            GD.Print("No save file found. Creating new file");

            GameManager.ResetHPToDefault();
            SaveGame();

            return;
        }

        GD.Print("A save file has been found! Loading file...");

        FileAccess fileAccess = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
        GameManager.PlayerHP = Convert.ToInt32(fileAccess.Get8());
        GameManager.Gold = Convert.ToInt32(fileAccess.Get8());

        if (GameManager.PlayerHP == 0)
        {
            GameManager.ResetHPToDefault();
        }

        fileAccess.Close();
    }
}
using Godot;
using System;

public partial class GameManager : Node
{
    public static int PlayerHP = 0;
    public static int Gold = 0;

    public static void ResetHPToDefault()
    {
        PlayerHP = 10;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            Utils.SaveGame();
        }
    }
}

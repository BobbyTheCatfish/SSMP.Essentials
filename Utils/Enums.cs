namespace SSMPEssentials.Utils
{
    internal enum PacketIDs
    {
        Huddle,
        TeleportRequest,
        TeleportAccept,
        Message,
        PlayerDeath,
        PlayerHealth,
        Settings,
        Color
    }

    internal enum Messages
    {
        TeleportDenied,
    }

    internal enum CauseOfDeath
    {
        Generic,
        Player,
        Enemy,
        Spikes,
        Acid,
        Lava,
        Pit,
        Coal,
        Zap,
        Explosion,
        Sink,
        Steam,
        Frost
    }

    internal enum Colors
    {
        Black,
        White,
        Blue,
        Green,
        Orange,
        Purple,
        Red,
        Yellow
    }
}

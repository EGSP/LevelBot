namespace LevelBot.Code.Models;

public class GuildUser
{
    public Guild Guild { get; set; }
    public ulong UserId { get; set; }

    public GuildUser()
    {
    }
    
    public GuildUser(Guild guild, ulong userId)
    {
        Guild = guild;
        UserId = userId;
    }
}
namespace LevelBot.Code.Models;

public class Guild
{
    public ulong GuildId { get; set; }
    
    public ICollection<GuildUser> GuildUsers { get; set; }
    
    public Guild(){}
    
    public Guild(ulong guildId)
    {
        GuildId = guildId;
        GuildUsers = new List<GuildUser>();
    }
}
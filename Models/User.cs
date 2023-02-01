namespace dotnet_rpg.Models;

public class User
{
    public int Id { get; set; }
    public  string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public List<Character>? Characters { get; set; }
}
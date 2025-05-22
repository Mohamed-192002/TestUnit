namespace ConsoleApp1.DTOS;

public class AuthData
{
    public Tokens Tokens { get; set; }
    public int AccountStatus { get; set; }
    public ProfileDto Profile { get; set; }
}
public class ProfileDto
{
    public long Id { get; set; }
}



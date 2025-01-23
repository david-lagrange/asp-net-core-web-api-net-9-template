namespace Shared.DataTransferObjects;

public record TokenDto
{
    public TokenDto(string? accessToken, string? refreshToken) => (AccessToken, RefreshToken) = (accessToken, refreshToken);
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}
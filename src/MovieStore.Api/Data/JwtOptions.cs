namespace MovieStore.Api.Data;
public record class JwtOptions(
    string Issuer,
    string Audience,
    string Subject,
    string SigningKey,
    int ExpirationSeconds
);
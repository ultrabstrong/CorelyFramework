namespace Corely.Sage300HH2.Connection
{
    public enum AuthenticationResult
    {
        InvalidUserCredentials = 1,
        InvalidWebApiClientCredentials = 2,
        UserAccountLocked = 3,
        WebApiClientLocked = 4,
        Validated = 5
    }
}

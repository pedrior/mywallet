namespace MyWallet.Features.Users.Shared;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        code: "user.invalid_credentials",
        description: "Email or password is incorrect.");
}
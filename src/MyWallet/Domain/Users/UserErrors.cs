namespace MyWallet.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "user.not_found",
        description: "The User does not exist.");

    public static readonly Error EmailNotUnique = Error.Conflict(
        code: "user.email_not_unique",
        description: "The email is already taken.");

    public static readonly Error InvalidPassword = Error.Failure(
        code: "user.invalid_password",
        description: "The password is invalid.");
    
    public static readonly Error CannotChangeToSameEmail = Error.Failure(
        code: "user.cannot_change_to_same_email",
        description: "The new email cannot be the same as the old email.");
    
    public static readonly Error CannotChangeToSamePassword = Error.Failure(
        code: "user.cannot_change_to_same_password",
        description: "The new password cannot be the same as the old password.");
}
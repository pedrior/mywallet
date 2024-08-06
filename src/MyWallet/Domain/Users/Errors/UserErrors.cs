namespace MyWallet.Domain.Users.Errors;

public static class UserErrors
{
    public static readonly Error EmailNotUnique = Error.Conflict(
        code: "user.email_not_unique",
        description: "The email is already taken.");

    public static readonly Error InvalidPassword = Error.Failure(
        code: "user.invalid_password",
        description: "The password is invalid.");
    
    public static readonly Error CannotChangeToSamePassword = Error.Failure(
        code: "user.cannot_change_to_same_password",
        description: "The new password cannot be the same as the old password.");
    
    public static readonly Error CategoryNotFound = Error.NotFound(
        code: "user.category_not_found",
        description: "The user does not have the category.");
    
    public static readonly Error CategoryAlreadyExists = Error.Conflict(
        code: "user.category_already_exists",
        description: "The user already has the category.");
}
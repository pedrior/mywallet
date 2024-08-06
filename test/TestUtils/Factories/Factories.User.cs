using Microsoft.Extensions.DependencyInjection;
using MyWallet.Domain.Users.Services;
using MyWallet.Domain.Users.ValueObjects;

namespace TestUtils.Factories;

public static partial class Factories
{
    public static class User
    {
        public static Task<ErrorOr<MyWallet.Domain.Users.User>> CreateDefaultWithServiceProvider(
            IServiceProvider services,
            UserId? id = null,
            Email? email = null,
            UserName? name = null,
            Password? password = null)
        {
            return CreateDefault(
                id,
                email,
                name,
                password,
                services.GetRequiredService<IEmailUniquenessChecker>(),
                services.GetRequiredService<IPasswordHasher>());
        }

        public static Task<ErrorOr<MyWallet.Domain.Users.User>> CreateDefault(
            UserId? id = null,
            Email? email = null,
            UserName? name = null,
            Password? password = null,
            IEmailUniquenessChecker? emailUniquenessChecker = null,
            IPasswordHasher? passwordHasher = null)
        {
            passwordHasher = PreparePasswordHasher(passwordHasher);
            emailUniquenessChecker = PrepareEmailUniquenessChecker(emailUniquenessChecker);

            return MyWallet.Domain.Users.User.CreateAsync(
                id ?? Constants.Constants.User.Id,
                name ?? Constants.Constants.User.Name,
                email ?? Constants.Constants.User.Email,
                password ?? Constants.Constants.User.Password,
                emailUniquenessChecker,
                passwordHasher);
        }

        private static IPasswordHasher PreparePasswordHasher(IPasswordHasher? passwordHasher)
        {
            if (passwordHasher is null)
            {
                passwordHasher = A.Fake<IPasswordHasher>();
                A.CallTo(() => passwordHasher.Hash(A<Password>._))
                    .Returns("hashed-password");
            }

            return passwordHasher;
        }

        private static IEmailUniquenessChecker PrepareEmailUniquenessChecker(
            IEmailUniquenessChecker? emailUniquenessChecker)
        {
            if (emailUniquenessChecker is null)
            {
                emailUniquenessChecker = A.Fake<IEmailUniquenessChecker>();
                A.CallTo(() => emailUniquenessChecker.IsUniqueAsync(A<Email>._, A<CancellationToken>._))
                    .Returns(true);
            }

            return emailUniquenessChecker;
        }
    }
}
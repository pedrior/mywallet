using MyWallet.Domain.Users;

namespace MyWallet.IntegrationTests.Shared;

public abstract class UserAwareIntegrationTests(TestApplicationFactory app) : IntegrationTest(app)
{
    protected Task<(UserId userId, string accessToken)> CreateUserAsync() => CreateUserInternalAsync();

    protected Task<(UserId userId, string accessToken)> CreateUser2Async() => CreateUserInternalAsync(
        id: Constants.User.Id2,
        email: Constants.User.Email2);

    private async Task<(UserId userId, string accessToken)> CreateUserInternalAsync(
        UserId? id = null,
        Email? email = null,
        UserName? name = null,
        Password? password = null)
    {
        var user = await Factories.User.CreateDefaultWithServiceProvider(
            services: Services,
            id: id,
            email: email,
            name: name,
            password: password);


        await GetRequiredService<IUserRepository>()
            .AddAsync(user.Value);

        return (user.Value.Id, CreateAccessToken(user.Value));
    }
}
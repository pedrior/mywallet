namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Users
    {
        public static HttpRequestMessage ViewProfile() => new(HttpMethod.Get, $"{BasePath}/users/me");

        public static HttpRequestMessage Login(string? email = null, string? password = null)
        {
            email ??= Constants.User.Email.Value;
            password ??= Constants.User.Password.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/users/login")
            {
                Content = ToJsonStringContent(new
                {
                    email,
                    password
                })
            };
        }

        public static HttpRequestMessage Register(
            string? name = null,
            string? email = null,
            string? password = null)
        {
            name ??= Constants.User.Name.Value;
            email ??= Constants.User.Email.Value;
            password ??= Constants.User.Password.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/users/register")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    email,
                    password
                })
            };
        }
        
        public static HttpRequestMessage UpdateProfile(string? name = null)
        {
            name ??= Constants.User.Name2.Value;

            return new HttpRequestMessage(HttpMethod.Put, $"{BasePath}/users/me/profile")
            {
                Content = ToJsonStringContent(new
                {
                    name
                })
            };
        }

        public static HttpRequestMessage ChangePassword(string? oldPassword = null, string? newPassword = null)
        {
            oldPassword ??= Constants.User.Password.Value;
            newPassword ??= Constants.User.Password2.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/users/me/change-password")
            {
                Content = ToJsonStringContent(new
                {
                    oldPassword,
                    newPassword
                })
            };
        }
    }
}
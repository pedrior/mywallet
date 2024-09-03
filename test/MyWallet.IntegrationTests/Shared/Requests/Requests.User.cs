namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Users
    {
        public static HttpRequestMessage Register(string name, string email, string password)
        {
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

        public static HttpRequestMessage Login(string email, string password)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/users/login")
            {
                Content = ToJsonStringContent(new
                {
                    email,
                    password
                })
            };
        }

        public static HttpRequestMessage ViewProfile() => new(HttpMethod.Get, $"{BasePath}/users/me");

        public static HttpRequestMessage UpdateProfile(string name)
        {
            return new HttpRequestMessage(HttpMethod.Patch, $"{BasePath}/users/me/profile")
            {
                Content = ToJsonStringContent(new
                {
                    name
                })
            };
        }

        public static HttpRequestMessage ChangeEmail(string newEmail, string password)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/users/me/change-email")
            {
                Content = ToJsonStringContent(new
                {
                    newEmail,
                    password
                })
            };
        }

        public static HttpRequestMessage ChangePassword(string oldPassword, string newPassword)
        {
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
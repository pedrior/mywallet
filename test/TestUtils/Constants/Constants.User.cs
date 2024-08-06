using MyWallet.Domain.Users.ValueObjects;

namespace TestUtils.Constants;

public static partial class Constants
{
    public static class User
    {
        public static readonly UserId Id = UserId.New();
        public static readonly Email Email = Email.Create("john@doe.com").Value;
        public static readonly Email Email2 = Email.Create("pedro@doe.com").Value;
        public static readonly UserName Name = UserName.Create("John Doe").Value;
        public static readonly UserName Name2 = UserName.Create("Pedro Doe").Value;
        public static readonly Password Password = Password.Create("JohnDoe123").Value;
        public static readonly Password Password2 = Password.Create("J0hn@Do3").Value;
    }
}
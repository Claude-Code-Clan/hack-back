using Microsoft.EntityFrameworkCore;

namespace XakUjin2026
{
    public static class DbSeeder
    {
        public static void SeedFakeUser(ApplicationDbContext context)
        {
            var fakeUser = context.Users.FirstOrDefault(u => u.Username == Const.FakeUserUsername);

            if (fakeUser == null)
            {
                context.Users.Add(new ApplicationUser
                {
                    Username = Const.FakeUserUsername,
                    Email = Const.FakeUserEmail,
                    PasswordHash = Const.FakeUserPassword,
                    EmailConfirmed = true,
                    UjinToken = Const.FakeUserUjinToken
                });
                context.SaveChanges();
                return;
            }

            if (fakeUser.UjinToken != Const.FakeUserUjinToken)
            {
                fakeUser.UjinToken = Const.FakeUserUjinToken;
                context.SaveChanges();
            }
        }
    }
}

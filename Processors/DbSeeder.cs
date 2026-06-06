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

        public static void SeedDeviceTypes(ApplicationDbContext context)
        {
            var deviceTypeNames = new[] { "Холл", "Лифтовая кабина", "Ресепшн" };

            var existing = context.DeviceTypes
                .Select(d => d.Name)
                .ToHashSet();

            var missing = deviceTypeNames
                .Where(name => !existing.Contains(name))
                .Select(name => new DeviceType { Name = name })
                .ToList();

            if (missing.Count > 0)
            {
                context.DeviceTypes.AddRange(missing);
                context.SaveChanges();
            }
        }
    }
}

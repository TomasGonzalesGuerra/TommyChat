using Microsoft.EntityFrameworkCore;
using TommyChat.API.Helpers;
using TommyChat.Shared.Entities;
using TommyChat.Shared.Enums;

namespace TommyChat.API.Data
{
    public class SeedDb(DataContext datacontext, IUserHelper userHelper)
    {
        private readonly DataContext _datacontext = datacontext;
        private readonly IUserHelper _userHelper = userHelper;

        public async Task SeedAsync()
        {
            await _datacontext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckSuperAdminAsync("El Tommy Gonzales", "eltommy@yopmail.com", "322 311 420", "naruto.gif", UserType.SuperAdmin);
            await CheckUserAsync("Ledys Bedoya", "ledys@yopmail.com", "322 311 460", "NoUser.png", UserType.User);
            await CheckUserAsync("Brad Pitt", "brad@yopmail.com", "322 311 462", "NoUser.png", UserType.User);
            await CheckUserAsync("Angelina Jolie", "angelina@yopmail.com", "322 311 620", "NoUser.png", UserType.User);
            await CheckUserAsync("Bob Marley", "bob@yopmail.com", "322 314 620", "NoUser.png", UserType.User);
            await CheckUserAsync("Nancy Ace", "Nancy@yopmail.com", "392 314 620", "NoUser.png", UserType.User);
            await CheckUserAsync("Mila Azul", "mila_azul@yopmail.com", "382 314 620", "NoUser.png", UserType.User);
            await CheckUserAsync("Miha", "miha@yopmail.com", "377 314 620", "NoUser.png", UserType.User);
            await CheckUserAsync("Tommy Zama", "hokage@tommy.com", "928 172 124", "NoUser.png", UserType.User);
            await CheckUserAsync("Hynata Hyuga", "hyna@tommy.com", "928 172 126", "NoUser.png", UserType.User);
            await CheckUserAsync("Ino Sarutobi", "ino@tommy.com", "928 172 129", "NoUser.png", UserType.User);
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.SuperAdmin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckSuperAdminAsync(string fullName, string email, string phone, string imageName, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);

            if (user == null)
            {
                string imagePath = $"https://schoolbook2024.blob.core.windows.net/users/{imageName}";

                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Photo = imagePath,
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }

        private async Task<User> CheckUserAsync(string fullName, string email, string phone, string imageName, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);

            if (user == null)
            {
                string imagePath = $"https://schoolbook2024.blob.core.windows.net/default/{imageName}";

                user = new User
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Photo = imagePath,
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }
    }
}

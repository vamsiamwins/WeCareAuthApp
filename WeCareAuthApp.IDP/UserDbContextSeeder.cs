//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Threading.Tasks;

//namespace WeCareAuthApp.IDP
//{
//    public static class UserDbContextSeeder
//    {
//        public static async Task SeedTestData(IServiceProvider serviceProvider)
//        {
//            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
//            if(await userManager.FindByNameAsync("testUser")== null)
//            {
//                var user = new User
//                {
//                    UserName = "testUser",
//                    Email = "test@test.com",
//                    EmailConfirmed = true
//                };
//                var result = await userManager.CreateAsync(user,"Test@1234");
//            }

//        }
//    }
//}

//using IdentityModel;
//using System.Security.Claims;
//using System.Text.Json;
//using Duende.IdentityServer;
//using Duende.IdentityServer.Test;


//namespace WeCareAuthApp.IDP
//{
//    public class TestUsers
//    {
//        public static List<TestUser> Users{
//            get
//            {
//                return new List<TestUser>
//                {
//                    new TestUser
//                    {
//                        SubjectId = "testsubId1",
//                        Username = "TestUser1",
//                        Password = "password",

//                        Claims = new List<Claim>
//                        {
//                            new Claim(JwtClaimTypes.GivenName , "TestGivenName1"),
//                            new Claim(JwtClaimTypes.FamilyName , "TestFamilyName1")
//                        }
//                    },
//                    new TestUser
//                    {
//                        SubjectId = "testsubId2",
//                        Username = "TestUser2",
//                        Password = "password",

//                        Claims = new List<Claim>
//                        {
//                            new Claim(JwtClaimTypes.GivenName , "TestGivenName2"),
//                            new Claim(JwtClaimTypes.FamilyName , "TestFamilyName2")
//                        }
//                    }
//                };
//            }
//        }
//    }   
//}

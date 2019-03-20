using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Collections;

namespace Server
{
    struct UserToken
    {
        public string token;
        public DateTime expirationDay;
        public int levelOfPermissions;

        public UserToken(string t, DateTime d, int l)
        {
            token = t;
            expirationDay = d;
            levelOfPermissions = l;
        }
    };

    class AccessControl
    {
        public BSKdbContext context;
        private List<UserToken> listOfUsersToken;
        private static Random random;

        public AccessControl(BSKdbContext c)
        {
            this.context = c;
            listOfUsersToken = new List<UserToken>();
            random = new Random();
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuiopasdfghjklzxcvbnm";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }

        //returns user token if user was authenticated, or otherwise returns ""
        public string UserLogIn(string login, string passward)
        {
            SHA256 hasher = SHA256.Create();
            //get users salt
            User user = context.Users.First(u => u.login == login);
            string hashedPass = passward + user.salt;

            //Console.Out.WriteLine("AUTHENTICATING. Passward: {0}\nSalt: {1}\nHash(plain): {2}\nHash: {4}\nUser password(DB): {3}", passward, user.salt, hashedPass, user.password, sha256_hash(hashedPass));

            if(sha256_hash(hashedPass).ToUpper().CompareTo(user.password) == 0)
            {
                string token = RandomString(128);
                listOfUsersToken.Add(new UserToken(token , DateTime.Today.AddHours(1), user.levelOfPermissions));

                return token;
            }

            return "";
        }

        public AUTHORIZATION_RESPONSE checkPermissions(string token, string tableName)
        {
            bool isAutorize = false;
            int userPermissions = 0;
            for (int i = listOfUsersToken.Count - 1; i >= 0; i--)
            {
                UserToken user = listOfUsersToken.ElementAt(i);
                if(user.token == token)
                {
                    if (DateTime.Compare(user.expirationDay, DateTime.Today) > 0)
                    {
                        Console.Out.WriteLine("There is token and there is authentification");
                        isAutorize = true;
                        userPermissions = user.levelOfPermissions;
                        user.expirationDay.AddMinutes(10);
                    }
                    else
                    {
                        //remove expired tokens
                        listOfUsersToken.RemoveAt(i);
                        return AUTHORIZATION_RESPONSE.NO_USER;
                    }

                    break;
                }
            }
            if (!isAutorize) return AUTHORIZATION_RESPONSE.NO_USER;

            int? tablePermissions = context.TablePermissions.FirstOrDefault(table => table.nameOfTable == tableName)?.levelOfPermissions;

            if (!tablePermissions.HasValue) return AUTHORIZATION_RESPONSE.NO_PERMISSION;

            if (userPermissions >= tablePermissions.Value) return AUTHORIZATION_RESPONSE.OK;
            else return AUTHORIZATION_RESPONSE.NO_PERMISSION;
        }

        public void Logout(string token)
        {
            for (int i = listOfUsersToken.Count - 1; i >= 0; i--)
            {
                if (listOfUsersToken.ElementAt(i).token == token)
                {
                    listOfUsersToken.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

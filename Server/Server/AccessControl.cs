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
        private BSKdbContext context;
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

        //returns user token if user was authenticated, or otherwise returns ""
        public string UserLogIn(string login, string passward)
        {
            SHA256 hasher = SHA256.Create();
            //get users salt
            User user = context.Users.First(u => u.login == login);
            string hashedPass = passward + user.salt;
            byte[] bytes = Encoding.ASCII.GetBytes(hashedPass);

            if(Encoding.ASCII.GetString(hasher.ComputeHash(bytes)) == user.password)
            {
                string token = RandomString(128);
                listOfUsersToken.Add(new UserToken(token , DateTime.Today.AddHours(1), user.levelOfPermissions));

                return token;
            }

            return "";
        }

        public bool checkPermissions(string token, string tableName)
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
                        isAutorize = true;
                        userPermissions = user.levelOfPermissions;
                        user.expirationDay.AddMinutes(10);
                    }
                    else
                    {
                        //remove expired tokens
                        listOfUsersToken.RemoveAt(i);
                    }

                    break;
                }
            }
            if (!isAutorize) return false;

            int? tablePermissions = context.TablePermissions.FirstOrDefault(table => table.nameOfTable == tableName)?.levelOfPermissions;

            if (!tablePermissions.HasValue) return false;

            return userPermissions >= tablePermissions.Value;
        }

    }
}

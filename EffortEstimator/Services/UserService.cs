using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using EffortEstimator.Helpers;
using EffortEstimator.Models;

namespace EffortEstimator.Services
{
    public class UserService
    {
        private readonly MySQL sql;
        private readonly KeyGenerator keyGenerator;
        private readonly MailOperator mailOperator;
        private readonly string JwtSecret;
        public UserService(string connectionString, string jwtSecret)
        {
            mailOperator = new MailOperator();
            keyGenerator = new KeyGenerator();
            sql = new MySQL(connectionString);
            JwtSecret = jwtSecret;
        }

        public string Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email is required");

            if (email.Length > 300)
                throw new Exception("Email is too long!");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            UserEntity user = sql.GetUser(email);
            if (user.Email == null)
                throw new Exception("User does not exists!");

            PasswordHasher ph = new PasswordHasher();
            bool verified = ph.Check(user.Hash, password).Verified;
            if (!verified)
                throw new Exception("Wrong password!");

            if (!user.Active)
                throw new Exception("User is not activated!");

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),

                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public bool Register(string email, string password, string name, string surname)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
                throw new Exception("Not every field is filled!");

            if (email.Length > 300)
                throw new Exception("We are sorry, your email address is too long!");

            if (name.Length > 100 || surname.Length > 100)
                throw new Exception("Name/surname is too long!");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (!MailOperator.IsValidEmail(email))
                throw new Exception("This email is not valid!");

            UserEntity user = sql.GetUser(email);
            if (user.Email != null)
                throw new Exception("User already exists!");

            PasswordHasher ph = new PasswordHasher();
            string hash = ph.Hash(password);

            string activationKey = keyGenerator.GetUserActivactionKey();
            if (sql.Register(email, hash, name, surname, activationKey))
                mailOperator.SendUserActivationKey(email, activationKey);
            else
                throw new Exception("An error occured. Try again later!");

            return true;
        }

        public bool ActivateUser(string email, string activationKey)
        {
            if (!MailOperator.IsValidEmail(email))
                throw new Exception("This email is not valid!");

            if (activationKey.Length > 20)
                throw new Exception("Wrong activation key!");

            return sql.ActivateUser(email, activationKey);
        }
    }
}

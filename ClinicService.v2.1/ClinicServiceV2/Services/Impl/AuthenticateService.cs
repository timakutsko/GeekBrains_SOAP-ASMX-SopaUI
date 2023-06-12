using AuthenticateServiceNamespace;
using ClinicService.Data;
using ClinicService.Data.Context;
using ClinicServiceV2.Utils;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AuthenticateServiceNamespace.AuthenticateService;

namespace ClinicServiceV2.Services.Impl
{
    [Authorize]
    public class AuthenticateService : AuthenticateServiceBase
    {
        public const string SecretKey = "kYp3s6v9y/B?E(H+";

        private readonly ClinicServiceDbContext _context;

        public AuthenticateService(ClinicServiceDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public override Task<AuthenticationResponse> Login(AuthenticationRequest request, ServerCallContext context)
        {
            AuthenticationResponse response;
            try
            {
                Account? account = !string.IsNullOrWhiteSpace(request.UserName) ? FindAccountByLogin(_context, request.UserName) : null;
                if (account == null)
                {
                    response = new AuthenticationResponse
                    {
                        ErrCode = 100,
                        ErrMessage = $"Такого пользователя - нет",
                    };
                    return Task.FromResult(response);
                }

                if (!PasswordUtils.VerifyPassword(request.Password, account.PasswordSalt, account.PasswordHash))
                {
                    response = new AuthenticationResponse
                    {
                        ErrCode = 101,
                        ErrMessage = $"Неверный пароль",
                    };
                    return Task.FromResult(response);
                }

                AccountSession session = new AccountSession
                {
                    AccountId = account.AccountId,
                    SessionToken = CreateSessionToken(account),
                    TimeCreated = DateTime.Now,
                    TimeLastRequest = DateTime.Now,
                    IsClosed = false,
                };
                _context.AccountSessions.Add(session);
                _context.SaveChanges();

                AccountDto accountDto = new AccountDto
                {
                    AccountId = account.AccountId,
                    EMail = account.EMail,
                    Locked = account.Locked,
                    Surname = account.LastName,
                    FirstName = account.FirstName
                };


                response = new AuthenticationResponse();
                response.SessionContext = new SessionContext()
                {
                    SessionId = session.SessionId,
                    SessionToken = session.SessionToken,
                    SessionAccount = accountDto
                };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                response = new AuthenticationResponse
                {
                    ErrCode = 110,
                    ErrMessage = $"Ошибка аутентификации: {ex.Message}",
                };

                return Task.FromResult(response);
            }
        }

        private Account? FindAccountByLogin(ClinicServiceDbContext context, string login) => context.Accounts.FirstOrDefault(account => account.EMail == login);

        private string CreateSessionToken(Account account)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                        new Claim(ClaimTypes.Name, account.EMail),
                    }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

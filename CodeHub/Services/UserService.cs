using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CodeHub.Data;
using CodeHub.Data.Entities;
using CodeHub.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace CodeHub.Services
{
    public class UserService
    {
        private IDbContextFactory<DatabaseContext> _dbContextFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IDbContextFactory<DatabaseContext> dbContextFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbContextFactory = dbContextFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext?.Response.HasStarted == false)
            {
                _httpContextAccessor.HttpContext.Session.SetString("initialized", "true");
            }
        }

        public async Task AddUserAsync(User user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task EditUserAsync(User user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(User user)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Users.ToListAsync();
            }
        }

        public async Task<User?> CreateUserFromRegistrationModelAsync(RegistrationModel rm)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var user = new User()
                {
                    Username = rm.Username,
                    Email = rm.Email,
                    PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(rm.Password),
                    CreatedAt = DateTime.Now,
                    Role = "Študent",
                    ProfileImage = GetDefaultProfileImage()
                };

                await AddUserAsync(user);
                return user;
            }
        }

        public async Task<User?> LoginUser(User storedUser)
        {
            //string token = CreateToken(storedUser);
            //var httpContext = _httpContextAccessor.HttpContext;

            //if (httpContext != null)
            //{
            //    httpContext.Response.Cookies.Append("authToken", token, new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Secure = true,
            //        SameSite = SameSiteMode.Strict,
            //        Expires = DateTime.Now.AddDays(1)
            //    });
            //}

            //return storedUser;
            string token = CreateToken(storedUser);
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString("authToken", token);
            }

            return storedUser;
        }

        public async Task LogoutUserAsync()
        {
            //var httpContext = _httpContextAccessor.HttpContext;
            //if (httpContext != null)
            //{
            //    httpContext.Response.Cookies.Delete("authToken");
            //}
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove("authToken");
            session?.Clear();
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            }
        }

        private byte[] GetDefaultProfileImage()
        {
            string defaultImagePath = "wwwroot/images/profileImage.png";
            if (File.Exists(defaultImagePath))
            {
                return File.ReadAllBytes(defaultImagePath);
            }
            else
            {
                throw new FileNotFoundException("Default profile image not found.");
            }
        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), 
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}

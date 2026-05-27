using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MartialArtsClubManagement.API.Models.Entities;
using MartialArtsClubManagement.API.Models.DTOs;
using MartialArtsClubManagement.API.Models.Config;

namespace MartialArtsClubManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly QuanLyCLBVoThuatContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(QuanLyCLBVoThuatContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public LoginResponse Authenticate(LoginRequest request)
    {
        // Tìm tài khoản theo email
        var user = _context.TaiKhoans.FirstOrDefault(tk => tk.Email == request.Email);
        if (user == null)
        {
            return new LoginResponse
            {
                Token = string.Empty,
                Role = string.Empty,
                Message = "Email không tồn tại"
            };
        }

        // Kiểm tra mật khẩu (hash)
        bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.MatKhauHash);
        if (!isValid)
        {
            return new LoginResponse
            {
                Token = string.Empty,
                Role = string.Empty,
                Message = "Mật khẩu không đúng"
            };
        }

        // Tạo JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.VaiTro)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new LoginResponse
        {
            Token = tokenString,
            Role = user.VaiTro,
            Message = "Đăng nhập thành công"
        };
    }
}

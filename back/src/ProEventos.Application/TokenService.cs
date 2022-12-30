using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain.Identity;

namespace ProEventos.Application
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        public readonly SymmetricSecurityKey key;

        public TokenService(IConfiguration config, UserManager<User> userManager, IMapper mapper)
        {
            this.config = config;
            this.userManager = userManager;
            this.mapper = mapper;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(UserUpdateDto userUpdateDto)
        {
            var user = mapper.Map<User>(userUpdateDto);

            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            }; //Claims fazem parte do usuario, sao "afirmacoes", e aqui acima, as 2 claims sao o ID (nameIdentifier) e o nome.

            var roles =  await userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); //Aqui adicionamos mais algumas claims, que seria as roles do usuario (ADM, Professor e Diretor).

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //A chave de criptografia.

            var tokenDescription = new SecurityTokenDescriptor{ //Aqui criamos a descricao do token, baseado nas claims, na data de expiracao e numa chave de criptografia.
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler(); //Pegamos o token e colocamos ele no formato JWT.

            var token = tokenHandler.CreateToken(tokenDescription); //Criamos o token propriamente.

            return tokenHandler.WriteToken(token); //E por fim escrevemos o token no formato JWT e o retornamos.
        }
    }
}
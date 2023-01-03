using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;

        public AccountController(IAccountService accountService,
                                ITokenService tokenService)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
        }

        [HttpGet("GetUser/{userName}")]
        [AllowAnonymous] //Permite que o metodo abaixo seja chamado externamente por alguem que nao tem autorizacao. (Pula a etapa de autorizacao, sem ele, ira dar erro de unauthorized, caso ainda nao haja um token).
        public async Task<IActionResult> GetUser(string userName){
            try
            {
                var user = await accountService.GetUserByUserNameAsync(userName);

                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar Usu�rio. Erro: {ex.Message}");
            }
        }


        [HttpPost("Register")]
        [AllowAnonymous] 
        public async Task<IActionResult> Register(UserDto userDto){
            try
            {
                if(await accountService.UserExists(userDto.UserName))
                    return BadRequest("Usu�rio j� existe!");

                var user = await accountService.CreateAccountAsync(userDto);
                if(user != null)
                    return Ok(user);
                
                return BadRequest("Usu�rio n�o criado, tente novamente mais tarde!");
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar cadastrar Usu�rio. Erro: {ex.Message}");
            }
        }


        [HttpPost("Login")]
        [AllowAnonymous] //Permite que o metodo abaixo seja chamado externamente por alguem que nao tem autorizacao. (Pula a etapa de autorizacao, sem ele, ira dar erro de unauthorized, caso ainda nao haja um token).
        public async Task<IActionResult> Login(UserLoginDto userLogin){
            try
            {
                var user = await accountService.GetUserByUserNameAsync(userLogin.Username);
                if (user == null) return Unauthorized("Usu�rio ou Senha est� errado");

                var result = await accountService.CheckUserPasswordAsync(user, userLogin.Password);
                if (!result.Succeeded) return Unauthorized();

                return Ok(new
                {
                    userName = user.UserName,
                    PrimeroNome = user.PrimeiroNome,
                    token = tokenService.CreateToken(user).Result
                });
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar Usu�rio. Erro: {ex.Message}");
            }
        }
        


    }
}
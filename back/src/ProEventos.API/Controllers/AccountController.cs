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
                $"Erro ao tentar recuperar Usuário. Erro: {ex.Message}");
            }
        }


        [HttpPost("Register")]
        [AllowAnonymous] //Permite que o metodo abaixo seja chamado externamente por alguem que nao tem autorizacao. (Pula a etapa de autorizacao, sem ele, ira dar erro de unauthorized, caso ainda nao haja um token).
        public async Task<IActionResult> Register(UserDto userDto){
            try
            {
                if(await accountService.UserExists(userDto.UserName))
                    return BadRequest("Usuário já existe!");

                var user = await accountService.CreateAccountAsync(userDto);
                if(user != null)
                    return Ok(user);
                
                return BadRequest("Usuário não criado, tente novamente mais tarde!");
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar cadastrar Usuário. Erro: {ex.Message}");
            }
        }
        


    }
}
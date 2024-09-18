using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UsuariosAPI.Data.Dtos;
using UsuariosAPI.Models;

namespace UsuariosAPI.Services
{
    public class UsuarioService
    {
        private IMapper _mapper;
        private UserManager<Usuario> _userManager;
        private SignInManager<Usuario> _signInManager;
        private TokenService _tokenService;

        public UsuarioService(UserManager<Usuario> userManager, 
            IMapper mapper,
            SignInManager<Usuario> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> CadastraAsync(CreateUsuarioDto dto)
        {
            Usuario usuario = _mapper.Map<Usuario>
               (dto);

            IdentityResult resultado = await _userManager.CreateAsync(usuario, dto.Password);

            if (!resultado.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Ocorreu um erro ao cadastrar o usuário."});
            }

            return resultado;  
        }

        public async Task<string> Login(LoginUsuarioDto dto)
        {
            var usuario = await _userManager.FindByNameAsync(dto.Username);

            if (usuario == null)
            {
                throw new ApplicationException("Usuário não encontrado!");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(usuario, dto.Password);

            if (!isPasswordValid)
            {
                throw new ApplicationException("Usuário não autenticado!");
            }


            var token = _tokenService.GenerateToken(usuario);

            return token;
        }

    }
}

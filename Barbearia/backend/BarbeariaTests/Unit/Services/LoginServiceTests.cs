//using Barbearia.Core.Domain.Entities;
//using Barbearia.Core.Domain.ValueObjects;
//using Barbearia.Core.Enum;
//using Barbearia.Core.Exceptions;
//using Barbearia.Core.Interface;
//using Barbearia.Core.Repository;
//using Barbearia.Core.Service;
//using Microsoft.Extensions.Logging;
//using Moq;
//namespace Barbearia.Tests.Services
//{
//    public class LoginServiceTests
//    {

//        private readonly Mock<ILoginRepository> _login;
//        private readonly Mock<ITokenService> _token;
//        private readonly Mock<IRefreshRepository> _refresh;
//        private readonly Mock<IUnitOfWork> _unit;
//        private readonly Mock<ILogger<LoginService>> _logger;

//        private readonly LoginService _service;

//        public LoginServiceTests()
//        {
//            _login = new Mock<ILoginRepository>();
//            _token = new Mock<ITokenService>();
//            _refresh = new Mock<IRefreshRepository>();
//            _unit = new Mock<IUnitOfWork>();
//            _logger = new Mock<ILogger<LoginService>>();
//            _service = new LoginService(
//            _login.Object,
//            _token.Object,
//            _refresh.Object,
//            _unit.Object,
//            _logger.Object);
//        }
//        // Usuário criado porque os itens são private e não pode ser criado além do próprio domain
//        private Usuario UsuarioTeste(bool ativado = true)
//        {
//            return new Usuario(
//            "admin",
//            new Email("admin@gmail.com"),
//            new Phone("27999999999"),
//            new Cpf("22178204007"),
//            "admin",
//            Senha.Criar("123456"),
//            RolePerson.Admin,
//            ativado,
//            null);
//        }

//        [Theory]
//        [InlineData("", "123456")]
//        [InlineData("    ", "123456")]
//        [InlineData("admin", "")]
//        [InlineData("admin", "   ")]
//        [InlineData("", "")]
//        public async Task Deve_Lancar_Excecao_Quando_Login_Ou_Senha_Estiverem_Vazios(string login, string senha)
//        {
//            var ex = await Assert.ThrowsAsync<DomainException>(() => _service.RealizarLoginAsync(login, senha));

//            Assert.Equal("EMPTY_FIELDS", ex.Code);

//            _login.Verify(x => x.ObterPorLoginAsync(It.IsAny<string>()), Times.Never);
//            _login.Verify(x => x.Atualizar(It.IsAny<Usuario>()), Times.Never);
//            _refresh.Verify(x => x.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
//            _unit.Verify(x => x.SaveChangesAsync(), Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Normalizar_Login_Antes_De_Buscar_Usuario()
//        {
//            var usuario = UsuarioTeste();

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            _token.Setup(x => x.GenerateToken(usuario)).Returns("access-token");

//            _token.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

//            await _service.RealizarLoginAsync("  aDmin  ", "123456");

//            _login.Verify(x => x.ObterPorLoginAsync("admin"), Times.Once);

//            _login.Verify(x => x.ObterPorLoginAsync("  aDmin  "), Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Usuario_Nao_Existe()
//        {
//            _login.Setup(x => x.ObterPorLoginAsync("pessoa")).ReturnsAsync((Usuario?)null);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//            _service.RealizarLoginAsync("pessoa", "123456"));

//            Assert.Equal("Credenciais inválidas!", ex.Message);

//            _login.Verify(x => x.Atualizar(It.IsAny<Usuario>()), Times.Never);

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Usuario_Estiver_Desativado()
//        {
//            var usuario = UsuarioTeste(false);

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//            _service.RealizarLoginAsync("admin", "123456"));

//            Assert.Equal("Credenciais inválidas!", ex.Message);

//            _login.Verify(x => x.Atualizar(It.IsAny<Usuario>()), Times.Never());

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }
//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Usuario_Estiver_Bloqueado()
//        {

//            var usuario = UsuarioTeste();

//            while (usuario.TentativasLogin < 5)
//            {
//                usuario.RegistrarFalhaLogin();
//            }

//            Assert.False(usuario.PodeLogar());

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//            _service.RealizarLoginAsync("admin", "123456"));

//            Assert.Equal("Credenciais inválidas!", ex.Message);

//            _token.Verify(x => x.GenerateToken(It.IsAny<Usuario>()), Times.Never);

//            _token.Verify(x => x.GenerateRefreshToken(), Times.Never);

//            _refresh.Verify(x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);
//        }
//        [Fact]
//        public async Task Deve_Incrementar_Tentativas_Quando_Senha_For_Invalida()
//        {
//            var usuario = UsuarioTeste();

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            await Assert.ThrowsAsync<DomainException>(() =>
//            _service.RealizarLoginAsync("admin", "123445"));

//            Assert.Equal(1, usuario.TentativasLogin);
//        }
//        [Fact]
//        public async Task Deve_Persistir_Falha_Quando_Senha_For_Invalida()
//        {
//            var usuario = UsuarioTeste();

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            await Assert.ThrowsAsync<DomainException>(() =>
//            _service.RealizarLoginAsync("admin", "123445"));

//            _login.Verify(x => x.Atualizar(usuario), Times.Once);

//            _unit.Verify(x => x.SaveChangesAsync(), Times.Once);

//            _token.Verify(
//               x => x.GenerateToken(It.IsAny<Usuario>()),
//               Times.Never);

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);
//        }
//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Senha_For_Invalida()
//        {
//            var usuario = UsuarioTeste();

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarLoginAsync("admin", "123424"));

//            Assert.Equal("Credenciais inválidas!", ex.Message);
//        }

//        [Fact]
//        public async Task Deve_Resetar_Tentativas_Apos_Login_Com_Sucesso()
//        {

//            var usuario = UsuarioTeste();

//            usuario.RegistrarFalhaLogin();
//            usuario.RegistrarFalhaLogin();
//            usuario.RegistrarFalhaLogin();

//            Assert.Equal(3, usuario.TentativasLogin);

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);

//            _token
//                .Setup(x => x.GenerateToken(usuario))
//                .Returns("access-token");

//            _token
//              .Setup(x => x.GenerateRefreshToken())
//              .Returns("refresh-token");

//            await _service.RealizarLoginAsync("admin", "123456");

//            Assert.Equal(0, usuario.TentativasLogin);

//            _login.Verify(x => x.Atualizar(usuario), Times.Once);

//            _unit.Verify(x => x.SaveChangesAsync(), Times.Once);
//        }
//        [Fact]
//        public async Task Deve_Gerar_Tokens_Quando_Login_For_Valido()
//        {
//            var usuario = UsuarioTeste();

//            _login.Setup(x => x.ObterPorLoginAsync("admin")).ReturnsAsync(usuario);


//            _token
//                .Setup(x => x.GenerateToken(usuario))
//                .Returns("access-token");

//            _token
//              .Setup(x => x.GenerateRefreshToken())
//              .Returns("refresh-token");

//            var resultado = await _service.RealizarLoginAsync("admin", "123456");

//            Assert.NotNull(resultado);

//            Assert.Equal("access-token", resultado.accessToken);
//            Assert.Equal("refresh-token", resultado.refreshToken);

//            _token.Verify(
//               x => x.GenerateToken(usuario),
//               Times.Once);

//            _token.Verify(
//                x => x.GenerateRefreshToken(),
//                Times.Once);
//        }

//        [Fact]
//        public async Task Deve_Armazenar_RefreshToken_Quando_Login_For_Valido()
//        {
//            var usuario = UsuarioTeste();

//            _login
//                .Setup(x => x.ObterPorLoginAsync("admin"))
//                .ReturnsAsync(usuario);

//            _token
//                .Setup(x => x.GenerateToken(usuario))
//                .Returns("access-token");

//            _token
//                .Setup(x => x.GenerateRefreshToken())
//                .Returns("refresh-token");

//            await _service.RealizarLoginAsync(
//                "admin",
//                "123456");

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    usuario.Id,
//                    "refresh-token",
//                    It.Is<DateTime>(data => data > DateTime.UtcNow)),
//                Times.Once);
//        }
//        [Fact]
//        public async Task Deve_Persistir_Alteracoes_Quando_Login_For_Valido()
//        {
//            var usuario = UsuarioTeste();

//            _login
//                .Setup(x => x.ObterPorLoginAsync("admin"))
//                .ReturnsAsync(usuario);

//            _token
//                .Setup(x => x.GenerateToken(usuario))
//                .Returns("access-token");

//            _token
//                .Setup(x => x.GenerateRefreshToken())
//                .Returns("refresh-token");

//            await _service.RealizarLoginAsync(
//                "admin",
//                "123456");

//            _login.Verify(
//                x => x.Atualizar(usuario),
//                Times.Once);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Once);
//        }
//        [Fact]
//        public async Task Nao_Deve_Persistir_Quando_Login_For_Vazio()
//        {
//            await Assert.ThrowsAsync<DomainException>(
//                () => _service.RealizarLoginAsync(
//                    "",
//                    "123456"));

//            _login.Verify(
//                x => x.ObterPorLoginAsync(It.IsAny<string>()),
//                Times.Never);

//            _login.Verify(
//                x => x.Atualizar(It.IsAny<Usuario>()),
//                Times.Never);

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }
//        [Fact]
//        public async Task Nao_Deve_Persistir_Quando_Senha_For_Vazia()
//        {
//            await Assert.ThrowsAsync<DomainException>(
//                () => _service.RealizarLoginAsync(
//                    "admin",
//                    ""));

//            _login.Verify(
//                x => x.ObterPorLoginAsync(It.IsAny<string>()),
//                Times.Never);

//            _login.Verify(
//                x => x.Atualizar(It.IsAny<Usuario>()),
//                Times.Never);

//            _refresh.Verify(
//                x => x.SaveAsync(
//                    It.IsAny<int>(),
//                    It.IsAny<string>(),
//                    It.IsAny<DateTime>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }
//    }
//}


//using Barbearia.Core.Domain.Entities;
//using Barbearia.Core.Domain.ValueObjects;
//using Barbearia.Core.Enum;
//using Barbearia.Core.Exceptions;
//using Barbearia.Core.Interface;
//using Barbearia.Core.Service;
//using Moq;

//namespace Barbearia.Tests.Services
//{
//    public class TrocaSenhaTests
//    {
//        private readonly Mock<ITrocaSenhaRepository> _repository;
//        private readonly Mock<IUnitOfWork> _unit;
//        private readonly TrocaSenhaService _service;

//        public TrocaSenhaTests()
//        {
//            _repository = new Mock<ITrocaSenhaRepository>();
//            _unit = new Mock<IUnitOfWork>();

//            _service = new TrocaSenhaService(
//                _repository.Object,
//                _unit.Object);
//        }

//        private Usuario UsuarioTest(bool ativado = true)
//        {
//            return new Usuario(
//                "admin",
//                new Email("admin@gmail.com"),
//                new Phone("27999999999"),
//                new Cpf("22178204007"),
//                "admin",
//                Senha.Criar("123456"),
//                RolePerson.Admin,
//                ativado,
//                null);
//        }

//        private static void PrepararCodigoValido(Usuario usuario, string codigo = "123456")
//        {
//            // GerarCodigo ativa o código, zera as tentativas e define expiração.
//            usuario.GerarCodigo(codigo);
//            usuario.ValorTempo(DateTime.Now.AddMinutes(10));
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Usuario_Nao_Existir()
//        {
//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync((Usuario?)null);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal("Dados inválidos!", ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Usuario_Estiver_Desativado()
//        {
//            var usuario = UsuarioTest(false);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal("Dados inválidos!", ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Codigo_Nao_Estiver_Ativo()
//        {
//            var usuario = UsuarioTest();

//            usuario.ValorCodigo("");

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal(
//                "Codigo expirado! Solicite um novo código!",
//                ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Codigo_Estiver_Expirado()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);
//            usuario.ValorTempo(DateTime.Now.AddMinutes(-5));

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal(
//                "Codigo expirado! Solicite um novo código!",
//                ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Registrar_Falha_Quando_Codigo_For_Incorreto()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "codigo-incorreto",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal("Dados inválidos!", ex.Message);
//            Assert.Equal(1, usuario.TentativasCodigo);

//            _repository.Verify(
//                x => x.AtualizaUsuario(usuario),
//                Times.Once);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Once);
//        }

//        [Fact]
//        public async Task Deve_Incrementar_Tentativas_Quando_Codigo_For_Incorreto()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            usuario.RegistrarFalhaTrocaSenha();
//            usuario.RegistrarFalhaTrocaSenha();

//            Assert.Equal(2, usuario.TentativasCodigo);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "codigo-incorreto",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal(3, usuario.TentativasCodigo);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Limite_De_Tentativas_For_Atingido()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            while (usuario.TentativasCodigo < 5)
//            {
//                usuario.RegistrarFalhaTrocaSenha();
//            }

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal(
//                "Codigo expirado! Solicite um novo código!",
//                ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Senhas_Forem_Diferentes()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654322"));

//            Assert.Equal("Dados inválidos!", ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Senha_Estiver_Vazia()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "",
//                    ""));

//            Assert.Equal("Senha inválida.", ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Lancar_Excecao_Quando_Senha_Tiver_Menos_De_6_Caracteres()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var ex = await Assert.ThrowsAsync<DomainException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "12345",
//                    "12345"));

//            Assert.Equal(
//                "Senha deve ter no mínimo 6 caracteres.",
//                ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(It.IsAny<Usuario>()),
//                Times.Never);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Never);
//        }

//        [Fact]
//        public async Task Deve_Alterar_Senha_Quando_Dados_Forem_Validos()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            var resultado = await _service.RealizarTrocaSenha(
//                "123456",
//                "admin@gmail.com",
//                "654321",
//                "654321");

//            Assert.NotNull(resultado);
//            Assert.True(resultado.Sucesso);
//            Assert.Equal("Senha alterada!", resultado.Mensagem);

//            Assert.True(usuario.Senha.Verify("654321"));
//            Assert.False(usuario.Senha.Verify("123456"));

//            _repository.Verify(
//                x => x.AtualizaUsuario(usuario),
//                Times.Once);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Once);
//        }

//        [Fact]
//        public async Task Deve_Buscar_Usuario_Pelo_Email_Informado()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            await _service.RealizarTrocaSenha(
//                "123456",
//                "admin@gmail.com",
//                "654321",
//                "654321");

//            _repository.Verify(
//                x => x.PegaInformacaoUsuario("admin@gmail.com"),
//                Times.Once);
//        }

//        [Fact]
//        public async Task Deve_Propagar_Excecao_Quando_Falhar_Ao_Salvar_No_Banco()
//        {
//            var usuario = UsuarioTest();

//            PrepararCodigoValido(usuario);

//            _repository
//                .Setup(x => x.PegaInformacaoUsuario("admin@gmail.com"))
//                .ReturnsAsync(usuario);

//            _unit
//                .Setup(x => x.SaveChangesAsync())
//                .ThrowsAsync(new InvalidOperationException(
//                    "Falha ao salvar no banco"));

//            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
//                _service.RealizarTrocaSenha(
//                    "123456",
//                    "admin@gmail.com",
//                    "654321",
//                    "654321"));

//            Assert.Equal("Falha ao salvar no banco", ex.Message);

//            _repository.Verify(
//                x => x.AtualizaUsuario(usuario),
//                Times.Once);

//            _unit.Verify(
//                x => x.SaveChangesAsync(),
//                Times.Once);
//        }
//    }
//}
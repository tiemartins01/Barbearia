using Barbearia.Core.Domain.Entities;
using Barbearia.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Barbearia.Core.Infrastructure.Configuration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> e)
        {
            e.ToTable("usuario");

            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nome).IsRequired().HasMaxLength(128).HasColumnName("nome");
            e.Property(x => x.Login).IsRequired().HasMaxLength(128).HasColumnName("login");
            e.Property(x => x.Role).HasColumnName("tipo");
            e.Property(x => x.Ativado).HasColumnName("ativado");
            e.Property(x => x.Foto).HasColumnName("foto");
            e.Property(x => x.TentativasLogin).HasDefaultValue(0).HasColumnName("tentativaslogin");
            // timestamp without time zone INSERIDO PARA QUE FOSSE TRABALHADO EM HORÁRIO BRASILEIRO
            e.Property(x => x.BloqueioAte).HasColumnName("bloqueioate");
            e.Property(x => x.Codigo).HasColumnName("codigo");
            // timestamp without time zone INSERIDO PARA QUE FOSSE TRABALHADO EM HORÁRIO BRASILEIRO
            e.Property(x => x.CodigoRecuperacaoExpiraEm).HasColumnName("tempocodigo");
            e.Property(x => x.TentativasCodigo).HasDefaultValue(0).HasColumnName("tentativascodigo");
            e.Property(x => x.CodigoAtivo).HasColumnName("codigovalido");


            // VALUE OBJECTS
            e.OwnsOne(x => x.Senha, senha =>
            {
                senha.Property(p => p.SenhaHash).HasColumnName("senha").HasMaxLength(100).IsRequired();
            });

            e.OwnsOne(x => x.Email, email =>
            {
                email.Property(p => p.EmailPessoa).HasColumnName("email").IsRequired();
                email.HasIndex(p => p.EmailPessoa).IsUnique().HasDatabaseName("ux_usuario_email");
            });

            e.OwnsOne(x => x.Phone, phone =>
            {
                phone.Property(p => p.Telefone).HasColumnName("numero").IsRequired();
                phone.HasIndex(p => p.Telefone).IsUnique().HasDatabaseName("ux_usuario_telefone");
            });

            e.OwnsOne(x => x.CPF, cpf =>
            {
                cpf.Property(p => p.Numero).HasColumnName("cpf").IsRequired();
                cpf.HasIndex(p => p.Numero).IsUnique().HasDatabaseName("ux_usuario_cpf");
            });

            e.HasIndex(x => x.Login).IsUnique().HasDatabaseName("ux_usuario_login"); ;
            e.HasIndex(x => new { x.Ativado, x.Role }).HasDatabaseName("ix_usuario_ativado_tipo");
        }
   }
}

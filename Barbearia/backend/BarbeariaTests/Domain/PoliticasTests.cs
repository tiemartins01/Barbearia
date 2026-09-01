using BarbeariaCore.Domain.Policies;

namespace BarbeariaTests.Domain;

public sealed class PoliticasTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]
    public void PoliticaSenha_Deve_Rejeitar_Senha_Invalida(string senha)
    {
        var ex = Assert.Throws<DomainException>(() => PoliticaSenha.Validar(senha));
        Assert.Equal("USER_INVALID_PASSWORD", ex.Code);
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("abcdef")]
    [InlineData("Senha@123")]
    public void PoliticaSenha_Deve_Aceitar_Seis_Ou_Mais_Caracteres(string senha)
        => PoliticaSenha.Validar(senha);

    [Fact]
    public void PoliticaAutenticacao_Deve_Manter_Parametros_Centrais()
    {
        Assert.Equal(5, PoliticaAutenticacao.LimiteTentativas);
        Assert.Equal(TimeSpan.FromMinutes(5), PoliticaAutenticacao.DuracaoBloqueio);
        Assert.Equal(TimeSpan.FromMinutes(15), PoliticaAutenticacao.TempoCodigo);
    }

    [Fact]
    public void Horario_Futuro_Deve_Ser_Aceito()
    {
        var agora = new DateTime(2026, 8, 28, 8, 0, 0);
        PoliticaAgenda.ValidarHorarioFuturo(agora.AddMinutes(30), agora);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Horario_Igual_Ou_Passado_Deve_Falhar(int minutos)
    {
        var agora = new DateTime(2026, 8, 28, 8, 0, 0);
        var ex = Assert.Throws<DomainException>(() =>
            PoliticaAgenda.ValidarHorarioFuturo(agora.AddMinutes(minutos), agora));
        Assert.Equal("APPOINTMENT_DATE_INVALID", ex.Code);
    }

    [Fact]
    public void Data_Passada_Deve_Falhar()
    {
        var ex = Assert.Throws<DomainException>(() =>
            PoliticaAgenda.ValidarDataNaoPassada(new DateOnly(2026, 8, 27), new DateOnly(2026, 8, 28)));
        Assert.Equal("APPOINTMENT_DATE_INVALID", ex.Code);
    }

    [Fact]
    public void Data_De_Hoje_Deve_Ser_Aceita()
        => PoliticaAgenda.ValidarDataNaoPassada(new DateOnly(2026, 8, 28), new DateOnly(2026, 8, 28));

    [Theory]
    [InlineData(8, 0)]
    [InlineData(8, 30)]
    [InlineData(12, 0)]
    [InlineData(17, 30)]
    public void Horario_Na_Grade_Deve_Ser_Aceito(int hora, int minuto)
        => PoliticaAgenda.ValidarHorarioNaGrade(new DateTime(2026, 8, 29, hora, minuto, 0));

    [Theory]
    [InlineData(7, 30, "APPOINTMENT_OUTSIDE_BUSINESS_HOURS")]
    [InlineData(18, 0, "APPOINTMENT_OUTSIDE_BUSINESS_HOURS")]
    [InlineData(18, 30, "APPOINTMENT_OUTSIDE_BUSINESS_HOURS")]
    [InlineData(8, 15, "APPOINTMENT_INVALID_TIME_SLOT")]
    [InlineData(10, 45, "APPOINTMENT_INVALID_TIME_SLOT")]
    public void Horario_Fora_Da_Grade_Deve_Falhar(int hora, int minuto, string code)
    {
        var ex = Assert.Throws<DomainException>(() =>
            PoliticaAgenda.ValidarHorarioNaGrade(new DateTime(2026, 8, 29, hora, minuto, 0)));
        Assert.Equal(code, ex.Code);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(90)]
    public void Duracao_Multipla_De_30_Deve_Ser_Aceita(int duracao)
        => PoliticaAgenda.ValidarDuracao(duracao);

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    [InlineData(15)]
    [InlineData(45)]
    public void Duracao_Invalida_Deve_Falhar(int duracao)
    {
        var ex = Assert.Throws<DomainException>(() => PoliticaAgenda.ValidarDuracao(duracao));
        Assert.Equal("APPOINTMENT_INVALID_DURATION", ex.Code);
    }

    [Fact]
    public void Atendimento_Que_Termina_As_18_Deve_Caber_No_Expediente()
        => Assert.True(PoliticaAgenda.CabeNoExpediente(new DateTime(2026, 8, 29, 17, 0, 0), 60));

    [Fact]
    public void Atendimento_Que_Passa_Das_18_Nao_Deve_Caber()
        => Assert.False(PoliticaAgenda.CabeNoExpediente(new DateTime(2026, 8, 29, 17, 30, 0), 60));

    [Fact]
    public void Termino_Fora_Do_Expediente_Deve_Falhar()
    {
        var ex = Assert.Throws<DomainException>(() =>
            PoliticaAgenda.ValidarTerminoDentroDoExpediente(new DateTime(2026, 8, 29, 17, 30, 0), 60));
        Assert.Equal("APPOINTMENT_EXCEEDS_BUSINESS_HOURS", ex.Code);
    }

    [Theory]
    [InlineData(9,0,10,0,9,30,10,30,true)]
    [InlineData(9,0,10,0,10,0,10,30,false)]
    [InlineData(10,0,11,0,9,0,10,0,false)]
    [InlineData(9,30,10,0,9,0,11,0,true)]
    public void Sobreposicao_Deve_Seguir_Intervalos_Semiabertos(
        int h1,int m1,int h2,int m2,int h3,int m3,int h4,int m4,bool esperado)
    {
        var d = new DateTime(2026, 8, 29);
        Assert.Equal(esperado, PoliticaAgenda.ExisteSobreposicao(
            d.AddHours(h1).AddMinutes(m1), d.AddHours(h2).AddMinutes(m2),
            d.AddHours(h3).AddMinutes(m3), d.AddHours(h4).AddMinutes(m4)));
    }

    [Fact]
    public void Conflito_Deve_Falhar_Com_Codigo_Correto()
    {
        var ex = Assert.Throws<DomainException>(() => PoliticaAgenda.GarantirDisponibilidade(true));
        Assert.Equal("APPOINTMENT_TIME_CONFLICT", ex.Code);
    }

    [Fact]
    public void Sem_Conflito_Deve_Ser_Aceito()
        => PoliticaAgenda.GarantirDisponibilidade(false);

    [Fact]
    public void Grade_Deve_Ter_20_Horarios_De_8_A_17_30()
    {
        var grade = PoliticaAgenda.GerarGradeHorario().ToArray();
        Assert.Equal(20, grade.Length);
        Assert.Equal(new TimeOnly(8, 0), grade.First());
        Assert.Equal(new TimeOnly(17, 30), grade.Last());
        Assert.DoesNotContain(new TimeOnly(18, 0), grade);
    }

    [Fact]
    public void GerarSlotsOcupados_De_90_Minutos_Deve_Gerar_3_Slots()
    {
        var inicio = new DateTime(2026, 8, 29, 10, 0, 0);
        var slots = PoliticaAgenda.GerarSlotsOcupados(inicio, 90).ToArray();
        Assert.Equal(3, slots.Length);
        Assert.Equal(inicio, slots[0]);
        Assert.Equal(inicio.AddMinutes(30), slots[1]);
        Assert.Equal(inicio.AddMinutes(60), slots[2]);
    }
}

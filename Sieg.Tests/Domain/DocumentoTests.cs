using FluentAssertions;
using Sieg.Domain.Entities;
using Sieg.Domain.Exceptions;

namespace Sieg.Tests.Domain;

public class Tests
{
    [TestFixture]
    public class DocumentoTests
    {
        private const string ConteudoXml = "<xml><nota>123</nota></xml>";
        private const string NomeArquivo = "nota_fiscal.xml";
        private const string CaminhoArquivo = "/docs/nota_fiscal.xml";

        [Test]
        public void Deve_Criar_Documento_Valido()
        {
            // Act
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            // Assert
            documento.NomeOriginalArquivo.Should().Be(NomeArquivo);
            documento.CaminhoXml.Should().Be(CaminhoArquivo);
            documento.Tamanho.Should().Be(1024);
            documento.Processado.Should().BeFalse();
            documento.XmlHash.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void Deve_Lancar_Excecao_Quando_Criar_Com_Nome_Vazio()
        {
            // Act
            Action act = () => new Documento("", CaminhoArquivo, 1024, ConteudoXml);

            // Assert
            act.Should()
                .Throw<DominioException>()
                .WithMessage("*Nome original do arquivo não pode ser vazio*");
        }

        [Test]
        public void Deve_Lancar_Excecao_Quando_Criar_Com_Caminho_Vazio()
        {
            Action act = () => new Documento(NomeArquivo, "", 1024, ConteudoXml);

            act.Should()
                .Throw<DominioException>()
                .WithMessage("*Caminho do arquivo XML não pode ser vazio*");
        }

        [Test]
        public void Deve_Lancar_Excecao_Quando_Tamanho_For_Menor_Ou_Igual_A_Zero()
        {
            Action act = () => new Documento(NomeArquivo, CaminhoArquivo, 0, ConteudoXml);

            act.Should()
                .Throw<DominioException>()
                .WithMessage("*O tamanho do arquivo deve ser maior que zero*");
        }

        [Test]
        public void Deve_Lancar_Excecao_Quando_Arquivo_Nao_For_Xml()
        {
            Action act = () => new Documento("nota.txt", "/docs/nota.txt", 1024, ConteudoXml);

            act.Should()
                .Throw<DominioException>()
                .WithMessage("*O arquivo deve ser do tipo XML*");
        }

        [Test]
        public void Deve_Atualizar_Documento_Com_Sucesso()
        {
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            documento.AtualizarDocumento("nova_nota.xml", "/novo/novo_arquivo.xml");

            documento.NomeOriginalArquivo.Should().Be("nova_nota.xml");
            documento.CaminhoXml.Should().Be("/novo/novo_arquivo.xml");
        }

        [Test]
        public void Deve_Alterar_Caminho_Com_Sucesso()
        {
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            documento.AlterarCaminhoXml("/novo/caminho.xml");

            documento.CaminhoXml.Should().Be("/novo/caminho.xml");
        }

        [Test]
        public void Deve_Lancar_Excecao_Ao_Alterar_Caminho_Vazio()
        {
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            Action act = () => documento.AlterarCaminhoXml("");

            act.Should()
                .Throw<DominioException>()
                .WithMessage("*Caminho do arquivo XML não pode ser vazio*");
        }

        [Test]
        public void Deve_Alterar_Nome_Original_Com_Sucesso()
        {
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            documento.AlterarNomeOriginalArquivo("novo_nome.xml");

            documento.NomeOriginalArquivo.Should().Be("novo_nome.xml");
        }

        [Test]
        public void Deve_Lancar_Excecao_Ao_Alterar_Nome_Original_Vazio()
        {
            var documento = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);

            Action act = () => documento.AlterarNomeOriginalArquivo("");

            act.Should()
                .Throw<DominioException>()
                .WithMessage("*Nome original do arquivo não pode ser vazio*");
        }

        [Test]
        public void Deve_Gerar_Hash_Consistente_Para_Mesmo_Conteudo()
        {
            var doc1 = new Documento(NomeArquivo, CaminhoArquivo, 1024, ConteudoXml);
            var doc2 = new Documento("outro_nome.xml", "/outro/caminho.xml", 2048, ConteudoXml);

            doc1.XmlHash.Should().Be(doc2.XmlHash);
        }

        [Test]
        public void Deve_Gerar_Hash_Diferente_Para_Conteudo_Diferente()
        {
            var doc1 = new Documento(NomeArquivo, CaminhoArquivo, 1024, "<xml>1</xml>");
            var doc2 = new Documento(NomeArquivo, CaminhoArquivo, 1024, "<xml>2</xml>");

            doc1.XmlHash.Should().NotBe(doc2.XmlHash);
        }
    }
}

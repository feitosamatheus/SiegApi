using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Api.Domain.Exceptions;
using Sieg.Api.Domain.Interfaces.Services;
using Sieg.Application.Dtos;
using Sieg.Domain.Entities;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.Services;
using Sieg.Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Sieg.Application.UseCases.Documentos.SalvarDocumento;

public sealed class SalvarDocumentoHandler : IRequestHandler<SalvarDocumentoCommand, SalvarDocumentoResult>
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly IArmazenamentoService _armazenamentoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SalvarDocumentoHandler> _logger;
    private readonly IBrokerService _publisher;

    public SalvarDocumentoHandler(IDocumentoRepository documentoRepository, IArmazenamentoService armazenamentoService, IUnitOfWork unitOfWork, ILogger<SalvarDocumentoHandler> logger, IBrokerService publisher)
    {
        _documentoRepository = documentoRepository;
        _armazenamentoService = armazenamentoService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task<SalvarDocumentoResult> Handle(SalvarDocumentoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var xml = await LerECriarXmlDocumentAsync(request.ArquivoUploadDTO.Conteudo);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml.OuterXml));
            var caminhoXml = await _armazenamentoService.SalvarAsync(stream);

            var documento = new Documento(request.ArquivoUploadDTO.NomeArquivo, caminhoXml, request.ArquivoUploadDTO.Tamanho, xml.OuterXml);

            await _documentoRepository.AdicionarAsync(documento, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var evento = new SalvarDocumentoEvent(Guid.NewGuid(), documento.Id);
            var json = JsonSerializer.Serialize(evento);
            await _publisher.PublishAsync(json, cancellationToken);

            var documentoDto = new DocumentoDTO(documento.Id, documento.NomeOriginalArquivo, documento.CaminhoXml, documento.Tamanho, documento.DataCriacao, documento.XmlHash);

            return new SalvarDocumentoResult(true, "Documento salvo com sucesso!", documentoDto);
        }
        catch (DominioDadosInvalidosException ex)
        {
            _logger.LogWarning(ex, "Upload de documento fiscal com dados inválidos: {Mensagem}", ex.Message);
            return new SalvarDocumentoResult(false, ex.Message);
        }
        catch (RegistroDuplicadoException ex)
        {
            _logger.LogWarning(ex, "Tentativa de upload de documento já existente: {Mensagem}", ex.Message);
            return new SalvarDocumentoResult(false, ex.Message);
        }
        catch (DominioException ex)
        {
            _logger.LogWarning(ex, "Violação de regra de negócio no upload do documento: {Mensagem}", ex.Message);
            return new SalvarDocumentoResult(false, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar o documento: {Mensagem}", ex.Message);
            return new SalvarDocumentoResult(false, "Falha ao processar o documento. Por favor, tente novamente ou contate o suporte.");
        }
    }

    private async Task<XmlDocument> LerECriarXmlDocumentAsync(Stream conteudoStream)
    {
        string conteudoXml;
        using (var reader = new StreamReader(conteudoStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true))
        {
            conteudoXml = await reader.ReadToEndAsync();
        }

        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            var xml = new XmlDocument { XmlResolver = null };
            using var stringReader = new StringReader(conteudoXml);
            using var xmlReader = XmlReader.Create(stringReader, settings);
            xml.Load(xmlReader);

            return xml;
        }
        catch
        {
            throw new DominioDadosInvalidosException("O XML do documento fiscal está malformado");
        }
    }
}

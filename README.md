
# ?? Sieg.Api – API de Ingestão e Gerenciamento de Documentos Fiscais

?? Repositório Api: `https://github.com/feitosamatheus/SiegApi` 
?? Repositório Service: `https://github.com/feitosamatheus/SiegWorker` 

## Visão Geral

A **Sieg.Api** é o ponto de entrada *performático* do ecossistema Sieg. Trata-se de uma API RESTful construída em **.NET 9.0** com base na **Clean Architecture**, responsável por gerenciar a ingestão inicial de Documentos Fiscais (NFe, CTe, NFSe) e prover endpoints para gerenciamento (CRUD).

Seu design foca em **integridade transacional**, **baixa latência** na resposta HTTP e **processamento desacoplado**, delegando o enriquecimento e a consolidação final dos dados para o microserviço **SiegWorker** via mensageria assíncrona.

---

## ?? Funcionalidades Chave (Endpoints REST)

A API está organizada em dois Controllers distintos, por prefixo de rota, para melhor separação de responsabilidades.

### 1. Controller: `/api/documentos` (Ingestão e CRUD Básico)

Este prefixo é o ponto de entrada para a **Ingestão** e gerencia o **Ciclo de Vida Básico** do documento.

| Funcionalidade | Endpoint | Método | Descrição |
| :--- | :--- | :--- | :--- |
| **Ingestão/Upload** | `/api/documentos` | `POST` | Ponto de entrada para o upload de XMLs fiscais. Realiza persistência síncrona e publicação assíncrona. |
| **Listar Todos** | `/api/documentos` | `GET` | Lista todos os documentos (simples). |
| **Consultar por ID** | `/api/documentos/{id}` | `GET` | Consulta detalhes de um documento pelo seu ID principal. |
| **Atualizar Documento** | `/api/documentos/{id}` | `PUT` | Atualiza metadados do documento. |
| **Remover Documento** | `/api/documentos/{id}` | `DELETE` | Remove (ou desativa) o documento. |

### 2. Controller: `/api/documentos-fiscais` (Consultas Avançadas e Gerenciamento)

Este prefixo é focado em **Consultas complexas** e **Gerenciamento** de documentos já processados.

| Funcionalidade | Endpoint | Método | Descrição |
| :--- | :--- | :--- | :--- |
| **Listar com Filtros** | `/api/documentos-fiscais` | `GET` | **Paginação e filtros avançados** (por CNPJ, UF, período de datas, etc.). |
| **Consultar Detalhes (por ID)** | `/api/documentos-fiscais/{id}` | `GET` | Consulta o documento pelo seu ID principal. |
| **Consultar por Doc ID** | `/api/documentos-fiscais/por-documento/{documentoId}` | `GET` | Consulta o documento pelo **ID de Documento Fiscal** (chave externa que pode ser usada pelo SiegWorker). |
| **Atualizar Documento** | `/api/documentos-fiscais/{id}` | `PUT` | Atualiza campos permitidos. |
| **Remover Documento** | `/api/documentos-fiscais/{id}` | `DELETE` | Remove o documento. |

---

## ??? Arquitetura e Organização (Clean Architecture)

A aplicação segue o padrão **Ports and Adapters** (Arquitetura Hexagonal), isolando o **Domain** das implementações de **Infrastructure**.

| Camada | Padrão / Função | Responsabilidade Principal |
| :--- | :--- | :--- |
| **API** | Adapter (Interface de Usuário) | Recebe requisições, valida *commands* (DTOs) e aciona a camada de Application. |
| **Application** | Core / Use Cases (MediatR) | Contém a lógica de orquestração (calcula hash, coordena S3, EF e MassTransit). |
| **Domain** | Núcleo de Negócio | Define entidades, *Value Objects* (CNPJ, Hash), *Domain Interfaces* e regras de negócio. |
| **Infrastructure** | Adapter (Integrações) | Implementa contratos de repositórios (EF Core/SQL), provedores de serviços externos (Amazon S3) e mensageria (RabbitMQ). |
| **IoC** | Composition Root | Configuração centralizada da Injeção de Dependências (*Microsoft.Extensions.DependencyInjection*). |

---

## ?? Decisões de Arquitetura e Modelagem

| Decisão | Justificativa | Atende Requisito |
| :--- | :--- | :--- |
| **Persistência Híbrida** | O **SQL Server (EF Core)** foi escolhido para metadados por sua garantia de **atomicidade/consistência** (essencial para checagem de **Idempotência**). O **Amazon S3** é usado para o armazenamento de **arquivos brutos (XMLs)**, garantindo escalabilidade e menor custo. | Performance e Integridade |
| **Design Assíncrono com Commit First** | A persistência dos metadados é realizada **sincronamente** (Commit First) para garantir a **integridade transacional** e a segurança do controle de **Idempotência**. O processamento *custoso* e o enriquecimento de dados são delegados assincronamente ao `SiegWorker` via RabbitMQ, otimizando o tempo de resposta da API. | Performance, Integridade e Resiliência |
| **Uso do MediatR** | Desacopla a camada **API** dos **Use Cases (Handlers)**, facilitando testes unitários e mantendo os *Controllers* enxutos e focados em HTTP. | Boas Práticas |

---

## ??? Segurança, Idempotência e Resiliência

### 1. Idempotência e Reprocessamento Seguro

* O hash do XML é calculado na camada **Application** e usado como chave de unicidade.
* A verificação atômica de duplicidade no SQL Server é feita de forma **transacional** antes de realizar qualquer inserção ou publicar o evento.

### 2. Resiliência na Comunicação

* O uso de **MassTransit** na comunicação com o RabbitMQ permite alta confiabilidade.
* **Retries e Backoff:** Políticas de `Retry` (tentativas) com *exponential backoff* são implementadas para lidar com falhas transitórias na conexão.
* **Dead Letter Queue (DLQ):** Mensagens que falham permanentemente são direcionadas para uma DLQ.

---

## ?? Estratégia de Testes

O projeto implementa uma abordagem em pirâmide de testes para garantir a qualidade, com foco inicial nas camadas de lógica de negócio:

* **Testes Unitários (NUnit + FluentAssertions):** **(Realizados)** Focam em **Domain** (regras de negócio) e **Application** (lógica dos *Use Cases*), utilizando *Mocks* para isolar dependências externas.

---

## ?? Stack Tecnológica

| Categoria | Tecnologia | Uso |
| :--- | :--- | :--- |
| **Framework** | **.NET 9.0** | Performance e desenvolvimento moderno. |
| **Mensageria** | **RabbitMQ / MassTransit** | Comunicação confiável e assíncrona. |
| **Armazenamento** | **SQL Server + EF Core** | Persistência de metadados e implementação de repositórios. |
| **Storage** | **Amazon S3** | Armazenamento de arquivos brutos (XMLs). |
| **Design Pattern** | **MediatR** | Desacoplamento entre *Handlers* (Use Cases) e *Controllers*. |
| **Testes** | **NUnit + FluentAssertions** | Frameworks para testes unitários e de integração. |
| **Documentação** | **Swagger / Swashbuckle** | Documentação interativa e auto-gerada dos endpoints. |

---

## ? Requisitos e Setup

* .NET SDK 9
* SQL Server, RabbitMQ e Bucket S3 já provisionados (endpoints acessíveis pela rede da API).

### ?? Configuração de Acesso (Credenciais)

Antes de executar, você **deve** garantir que as credenciais do ambiente estão configuradas. O projeto lê as seguintes configurações no `appsettings.json` ou via **Variáveis de Ambiente**:

| Configuração | Descrição | Exemplo (`appsettings.json`) |
| :--- | :--- | :--- |
| **`ConnectionStrings:SqlServerConnection`** | String de conexão completa para o SQL Server. | `Server=...,Database=...,User Id=...,Password=...` |
| **`AWS:AccessKey`** | Chave de acesso do usuário AWS com permissão para S3. | `SUA_ACCESS_KEY` |
| **`AWS:SecretKey`** | Chave secreta do usuário AWS. | `SUA_SECRET_KEY` |
| **`AWS:Region`** | Região AWS onde estão os serviços. | `us-east-2` |
| **`AWS:S3:BucketName`** | Nome do bucket S3 para armazenamento dos XMLs. | `xml-fiscais` |

## ?? Execução Local

Para iniciar o projeto localmente, siga os passos abaixo:

1.  **Navegar para o diretório da API:**
    ```bash
    cd Sieg.Api
    ```

2.  **Restaurar dependências e Compilar o Projeto:**
    ```bash
    dotnet build
    ```

4.  **Executar a API:**
    ```bash
    dotnet run
    ```

A API será iniciada na porta **5005** (conforme configurado no `launchSettings.json`). A documentação interativa estará disponível via Swagger no link: **`http://localhost:5005/swagger/index.html`**

---

## ?? Possíveis Melhorias (Se Houvesse Mais Tempo)

1.  **Testes de Integração (Fluxo Completo):** Implementar testes de integração (NUnit + WebApplicationFactory) para validar o fluxo completo da API (Controller ? Application ? Infrastructure), utilizando **TestContainers** ou um banco de dados **InMemory** para checar a persistência e o controle de idempotência.
2.  **Observabilidade Completa:** Implementar *Distributed Tracing* (via OpenTelemetry ou similar) para ter uma visão unificada do fluxo da requisição entre `Sieg.Api`, RabbitMQ e `SiegWorker`.
3.  **Teste de Arquitetura:** Criar testes de arquitetura (*ArchUnit.NET* ou similar) para garantir que as regras da Clean Architecture sejam mantidas.
4.  **Teste de Carga:** Criar um teste de carga simples (k6 ou NBomber) para validar a performance da Ingestão e das consultas sob pressão.
5. **Testes Unitários**: Teste unitarios completos do **Application** (lógica dos *Use Cases*).

# ?? Sieg.Api � API de Ingest�o e Gerenciamento de Documentos Fiscais

?? Reposit�rio Api: `https://github.com/feitosamatheus/SiegApi` 
?? Reposit�rio Service: `https://github.com/feitosamatheus/SiegWorker` 

## Vis�o Geral

A **Sieg.Api** � o ponto de entrada *perform�tico* do ecossistema Sieg. Trata-se de uma API RESTful constru�da em **.NET 9.0** com base na **Clean Architecture**, respons�vel por gerenciar a ingest�o inicial de Documentos Fiscais (NFe, CTe, NFSe) e prover endpoints para gerenciamento (CRUD).

Seu design foca em **integridade transacional**, **baixa lat�ncia** na resposta HTTP e **processamento desacoplado**, delegando o enriquecimento e a consolida��o final dos dados para o microservi�o **SiegWorker** via mensageria ass�ncrona.

---

## ?? Funcionalidades Chave (Endpoints REST)

A API est� organizada em dois Controllers distintos, por prefixo de rota, para melhor separa��o de responsabilidades.

### 1. Controller: `/api/documentos` (Ingest�o e CRUD B�sico)

Este prefixo � o ponto de entrada para a **Ingest�o** e gerencia o **Ciclo de Vida B�sico** do documento.

| Funcionalidade | Endpoint | M�todo | Descri��o |
| :--- | :--- | :--- | :--- |
| **Ingest�o/Upload** | `/api/documentos` | `POST` | Ponto de entrada para o upload de XMLs fiscais. Realiza persist�ncia s�ncrona e publica��o ass�ncrona. |
| **Listar Todos** | `/api/documentos` | `GET` | Lista todos os documentos (simples). |
| **Consultar por ID** | `/api/documentos/{id}` | `GET` | Consulta detalhes de um documento pelo seu ID principal. |
| **Atualizar Documento** | `/api/documentos/{id}` | `PUT` | Atualiza metadados do documento. |
| **Remover Documento** | `/api/documentos/{id}` | `DELETE` | Remove (ou desativa) o documento. |

### 2. Controller: `/api/documentos-fiscais` (Consultas Avan�adas e Gerenciamento)

Este prefixo � focado em **Consultas complexas** e **Gerenciamento** de documentos j� processados.

| Funcionalidade | Endpoint | M�todo | Descri��o |
| :--- | :--- | :--- | :--- |
| **Listar com Filtros** | `/api/documentos-fiscais` | `GET` | **Pagina��o e filtros avan�ados** (por CNPJ, UF, per�odo de datas, etc.). |
| **Consultar Detalhes (por ID)** | `/api/documentos-fiscais/{id}` | `GET` | Consulta o documento pelo seu ID principal. |
| **Consultar por Doc ID** | `/api/documentos-fiscais/por-documento/{documentoId}` | `GET` | Consulta o documento pelo **ID de Documento Fiscal** (chave externa que pode ser usada pelo SiegWorker). |
| **Atualizar Documento** | `/api/documentos-fiscais/{id}` | `PUT` | Atualiza campos permitidos. |
| **Remover Documento** | `/api/documentos-fiscais/{id}` | `DELETE` | Remove o documento. |

---

## ??? Arquitetura e Organiza��o (Clean Architecture)

A aplica��o segue o padr�o **Ports and Adapters** (Arquitetura Hexagonal), isolando o **Domain** das implementa��es de **Infrastructure**.

| Camada | Padr�o / Fun��o | Responsabilidade Principal |
| :--- | :--- | :--- |
| **API** | Adapter (Interface de Usu�rio) | Recebe requisi��es, valida *commands* (DTOs) e aciona a camada de Application. |
| **Application** | Core / Use Cases (MediatR) | Cont�m a l�gica de orquestra��o (calcula hash, coordena S3, EF e MassTransit). |
| **Domain** | N�cleo de Neg�cio | Define entidades, *Value Objects* (CNPJ, Hash), *Domain Interfaces* e regras de neg�cio. |
| **Infrastructure** | Adapter (Integra��es) | Implementa contratos de reposit�rios (EF Core/SQL), provedores de servi�os externos (Amazon S3) e mensageria (RabbitMQ). |
| **IoC** | Composition Root | Configura��o centralizada da Inje��o de Depend�ncias (*Microsoft.Extensions.DependencyInjection*). |

---

## ?? Decis�es de Arquitetura e Modelagem

| Decis�o | Justificativa | Atende Requisito |
| :--- | :--- | :--- |
| **Persist�ncia H�brida** | O **SQL Server (EF Core)** foi escolhido para metadados por sua garantia de **atomicidade/consist�ncia** (essencial para checagem de **Idempot�ncia**). O **Amazon S3** � usado para o armazenamento de **arquivos brutos (XMLs)**, garantindo escalabilidade e menor custo. | Performance e Integridade |
| **Design Ass�ncrono com Commit First** | A persist�ncia dos metadados � realizada **sincronamente** (Commit First) para garantir a **integridade transacional** e a seguran�a do controle de **Idempot�ncia**. O processamento *custoso* e o enriquecimento de dados s�o delegados assincronamente ao `SiegWorker` via RabbitMQ, otimizando o tempo de resposta da API. | Performance, Integridade e Resili�ncia |
| **Uso do MediatR** | Desacopla a camada **API** dos **Use Cases (Handlers)**, facilitando testes unit�rios e mantendo os *Controllers* enxutos e focados em HTTP. | Boas Pr�ticas |

---

## ??? Seguran�a, Idempot�ncia e Resili�ncia

### 1. Idempot�ncia e Reprocessamento Seguro

* O hash do XML � calculado na camada **Application** e usado como chave de unicidade.
* A verifica��o at�mica de duplicidade no SQL Server � feita de forma **transacional** antes de realizar qualquer inser��o ou publicar o evento.

### 2. Resili�ncia na Comunica��o

* O uso de **MassTransit** na comunica��o com o RabbitMQ permite alta confiabilidade.
* **Retries e Backoff:** Pol�ticas de `Retry` (tentativas) com *exponential backoff* s�o implementadas para lidar com falhas transit�rias na conex�o.
* **Dead Letter Queue (DLQ):** Mensagens que falham permanentemente s�o direcionadas para uma DLQ.

---

## ?? Estrat�gia de Testes

O projeto implementa uma abordagem em pir�mide de testes para garantir a qualidade, com foco inicial nas camadas de l�gica de neg�cio:

* **Testes Unit�rios (NUnit + FluentAssertions):** **(Realizados)** Focam em **Domain** (regras de neg�cio) e **Application** (l�gica dos *Use Cases*), utilizando *Mocks* para isolar depend�ncias externas.

---

## ?? Stack Tecnol�gica

| Categoria | Tecnologia | Uso |
| :--- | :--- | :--- |
| **Framework** | **.NET 9.0** | Performance e desenvolvimento moderno. |
| **Mensageria** | **RabbitMQ / MassTransit** | Comunica��o confi�vel e ass�ncrona. |
| **Armazenamento** | **SQL Server + EF Core** | Persist�ncia de metadados e implementa��o de reposit�rios. |
| **Storage** | **Amazon S3** | Armazenamento de arquivos brutos (XMLs). |
| **Design Pattern** | **MediatR** | Desacoplamento entre *Handlers* (Use Cases) e *Controllers*. |
| **Testes** | **NUnit + FluentAssertions** | Frameworks para testes unit�rios e de integra��o. |
| **Documenta��o** | **Swagger / Swashbuckle** | Documenta��o interativa e auto-gerada dos endpoints. |

---

## ? Requisitos e Setup

* .NET SDK 9
* SQL Server, RabbitMQ e Bucket S3 j� provisionados (endpoints acess�veis pela rede da API).

### ?? Configura��o de Acesso (Credenciais)

Antes de executar, voc� **deve** garantir que as credenciais do ambiente est�o configuradas. O projeto l� as seguintes configura��es no `appsettings.json` ou via **Vari�veis de Ambiente**:

| Configura��o | Descri��o | Exemplo (`appsettings.json`) |
| :--- | :--- | :--- |
| **`ConnectionStrings:SqlServerConnection`** | String de conex�o completa para o SQL Server. | `Server=...,Database=...,User Id=...,Password=...` |
| **`AWS:AccessKey`** | Chave de acesso do usu�rio AWS com permiss�o para S3. | `SUA_ACCESS_KEY` |
| **`AWS:SecretKey`** | Chave secreta do usu�rio AWS. | `SUA_SECRET_KEY` |
| **`AWS:Region`** | Regi�o AWS onde est�o os servi�os. | `us-east-2` |
| **`AWS:S3:BucketName`** | Nome do bucket S3 para armazenamento dos XMLs. | `xml-fiscais` |

## ?? Execu��o Local

Para iniciar o projeto localmente, siga os passos abaixo:

1.  **Navegar para o diret�rio da API:**
    ```bash
    cd Sieg.Api
    ```

2.  **Restaurar depend�ncias e Compilar o Projeto:**
    ```bash
    dotnet build
    ```

4.  **Executar a API:**
    ```bash
    dotnet run
    ```

A API ser� iniciada na porta **5005** (conforme configurado no `launchSettings.json`). A documenta��o interativa estar� dispon�vel via Swagger no link: **`http://localhost:5005/swagger/index.html`**

---

## ?? Poss�veis Melhorias (Se Houvesse Mais Tempo)

1.  **Testes de Integra��o (Fluxo Completo):** Implementar testes de integra��o (NUnit + WebApplicationFactory) para validar o fluxo completo da API (Controller ? Application ? Infrastructure), utilizando **TestContainers** ou um banco de dados **InMemory** para checar a persist�ncia e o controle de idempot�ncia.
2.  **Observabilidade Completa:** Implementar *Distributed Tracing* (via OpenTelemetry ou similar) para ter uma vis�o unificada do fluxo da requisi��o entre `Sieg.Api`, RabbitMQ e `SiegWorker`.
3.  **Teste de Arquitetura:** Criar testes de arquitetura (*ArchUnit.NET* ou similar) para garantir que as regras da Clean Architecture sejam mantidas.
4.  **Teste de Carga:** Criar um teste de carga simples (k6 ou NBomber) para validar a performance da Ingest�o e das consultas sob press�o.
5. **Testes Unit�rios**: Teste unitarios completos do **Application** (l�gica dos *Use Cases*).
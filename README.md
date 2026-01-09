# Golden Raspberry Awards API

![CI](https://github.com/LeandroZavaski/golden-raspberry-awards/actions/workflows/ci.yml/badge.svg)
[![codecov](https://codecov.io/gh/LeandroZavaski/golden-raspberry-awards/branch/main/graph/badge.svg)](https://codecov.io/gh/LeandroZavaski/golden-raspberry-awards)

API RESTful para leitura da lista de indicados e vencedores da categoria **Pior Filme** do Golden Raspberry Awards.

## 📋 Requisitos

- .NET 10 SDK
- Visual Studio 2022+ ou VS Code

## 🚀 Como Executar

### Clone o repositório
```bash
git clone https://github.com/LeandroZavaski/golden-raspberry-awards.git
cd golden-raspberry-awards
```

### Restaure as dependências e execute
```bash
dotnet restore
dotnet run --project src/GoldenRaspberryAwards.API
```

A API estará disponível em: `http://localhost:5000`

### Swagger UI
Acesse a documentação interativa em: `http://localhost:5000/swagger`

## 🧪 Executar Testes

```bash
dotnet test --verbosity normal
```

### Testes com cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Estrutura do Projeto

```
├── src/
│   ├── GoldenRaspberryAwards.API/          # Camada de apresentação (Controllers)
│   ├── GoldenRaspberryAwards.Domain/       # Entidades, DTOs e Interfaces
│   └── GoldenRaspberryAwards.Infrastructure/ # Repositórios, Serviços e Dados
├── tests/
│   └── GoldenRaspberryAwards.Tests/        # Testes de integração
└── GoldenRaspberryAwards.sln
```

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**:

- **Domain**: Entidades de negócio, DTOs e interfaces (sem dependências externas)
- **Infrastructure**: Implementações concretas (Dapper, SQLite, Serviços)
- **API**: Controllers REST seguindo Richardson Maturity Model Level 2

### Tecnologias Utilizadas

| Tecnologia | Propósito |
|------------|-----------|
| .NET 10 | Framework principal |
| Dapper | Micro ORM para acesso a dados |
| SQLite In-Memory | Banco de dados em memória |
| xUnit | Framework de testes |
| FluentAssertions | Assertions fluentes |
| Swashbuckle | Documentação OpenAPI/Swagger |

## 📡 Endpoints

### GET /api/producers/awards-interval

Retorna o produtor com **maior intervalo** entre dois prêmios consecutivos e o que obteve dois prêmios **mais rápido**.

#### Response
```json
{
  "min": [
    {
      "producer": "Joel Silver",
      "interval": 1,
      "previousWin": 1990,
      "followingWin": 1991
    }
  ],
  "max": [
    {
      "producer": "Matthew Vaughn",
      "interval": 13,
      "previousWin": 2002,
      "followingWin": 2015
    }
  ]
}
```

## 📊 Importação de Dados

Os dados são importados automaticamente na inicialização da aplicação a partir do arquivo `movielist.csv` localizado em `src/GoldenRaspberryAwards.API/Data/`.

O arquivo CSV deve seguir o formato:
```csv
year;title;studios;producers;winner
1980;Can't Stop the Music;Associated Film Distribution;Allan Carr;yes
```

## ✅ Testes de Integração

O projeto inclui 9 testes de integração que validam:

- ✓ Retorno correto do endpoint
- ✓ Estrutura do JSON de resposta
- ✓ Cálculo correto de intervalos mínimos e máximos
- ✓ Tratamento de múltiplos produtores
- ✓ Cenários com banco vazio
- ✓ Cenários sem vencedores múltiplos

## 🔄 CI/CD

O projeto utiliza **GitHub Actions** para integração contínua:

- Build e testes em cada push/PR
- Testes multi-plataforma (Ubuntu, Windows, macOS)
- Relatório de cobertura de código via Codecov

## 📝 Licença

Este projeto está sob a licença MIT.

---

Desenvolvido como parte do desafio técnico Outsera.

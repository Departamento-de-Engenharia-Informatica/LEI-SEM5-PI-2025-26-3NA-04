# APDL - Sistema de Gestão de Operações Portuárias

Sistema de gestão de operações portuárias desenvolvido com arquitetura de microserviços, incluindo módulos para visualização, gestão de dados e planeamento de operações.

## Estrutura do Projeto

O projeto está organizado em vários módulos:

- **APDL.API** - Backend principal em ASP.NET Core (.NET 8)
- **APDL.SPA** - Frontend em Angular
- **APDL.OEM** - Backend Node.js para gestão de operações e execuções
- **APDL.Planning** - Algoritmos de planeamento em Prolog

## Arquitetura C4 Model

### Nível 1 - Context Diagram (Diagrama de Contexto)

O diagrama de contexto mostra o sistema APDL e as suas interações com utilizadores e sistemas externos.

![Nível 1 - Context Diagram](Documentation/Sprint%20C/ARQSI/Level%201/nv1_logica.drawio.png)

**Descrição:**
- **Utilizadores do Sistema**: Port Authority, Logistics Operators, Shipping Agents
- **Sistema APDL**: Sistema principal de gestão de operações portuárias
- **Sistemas Externos**: Auth0 (autenticação), Sistemas externos de navegação

### Nível 2 - Container Diagram (Diagrama de Contentores)

O diagrama de contentores mostra os principais componentes do sistema e como interagem entre si.

![Nível 2 - Container Diagram](Documentation/Sprint%20C/ARQSI/Level%202/c_nv2_logica.drawio.png)

**Componentes Principais:**

1. **Visualization (Angular)**
   - Frontend SPA desenvolvido em Angular
   - Comunica com Backend API e OEM System via HTTP/HTTPS
   - Autenticação via Auth0

2. **Backend API (.NET)**
   - API REST desenvolvida em ASP.NET Core
   - Utiliza Domain-Driven Design (DDD)
   - Base de dados SQLite para persistência
   - Autenticação e autorização via Auth0

3. **OEM System (Node.js)**
   - Backend Node.js/Express para gestão de operações
   - Base de dados MongoDB para persistência
   - Comunica com Backend API para obter dados de VVNs
   - Executa algoritmos Prolog para geração de planos de operação

4. **Database (SQLite)**
   - Base de dados relacional para o Backend API
   - Gerida via Entity Framework Core

5. **Database (MongoDB)**
   - Base de dados NoSQL para o OEM System
   - Gerida via Mongoose ODM

6. **Planning Module (Prolog)**
   - Algoritmos de planeamento em Prolog
   - Executados pelo OEM System via child processes

7. **Auth0 Service**
   - Serviço externo de autenticação e autorização
   - Utilizado por todos os módulos do sistema

### Nível 3 - Component Diagram (Diagrama de Componentes)

#### Backend API (.NET) - Component View

![Nível 3 - Backend API Components](Documentation/Sprint%20C/ARQSI/Level%203/nv3_logica_back.drawio.png)

**Estrutura de Componentes:**

- **Controllers**: Endpoints REST que recebem requisições HTTP
- **Services**: Lógica de negócio e orquestração
- **Domain**: Entidades, agregados e value objects (DDD)
- **Infrastructure**: Repositórios, configurações de base de dados, integrações externas

#### OEM System (Node.js) - Component View

![Nível 3 - OEM System Components](Documentation/Sprint%20C/ARQSI/Level%203/oem_nv3.drawio.png)

**Estrutura de Componentes:**

- **Routes**: Definição de rotas e endpoints da API
- **Middleware**: Autenticação e tratamento de erros
- **Validators**: Validação de dados de entrada
- **Controllers**: Manipulação de requisições HTTP
- **Services**: Lógica de negócio e orquestração
- **Models**: Schemas Mongoose para persistência
- **Database (MongoDB)**: Persistência de dados

#### Frontend (Angular) - Component View

![Nível 3 - Frontend Components](Documentation/Sprint%20C/ARQSI/Level%203/nv3_front_logica.drawio.png)

**Estrutura de Componentes:**

- **Pages**: Componentes de página principais
- **Components**: Componentes reutilizáveis (header, footer, etc.)
- **3D Module**: Módulo de visualização 3D utilizando Three.js para renderizar a cena do porto
- **Services**: Serviços para comunicação com APIs
- **Guards**: Route guards para autenticação e autorização
- **Interceptors**: HTTP interceptors para adicionar tokens
- **Models**: Interfaces TypeScript para tipagem

## Como Iniciar o Projeto

### Pré-requisitos

- .NET 8 SDK
- Node.js (versão 18 ou superior)
- MongoDB (para o módulo OEM)
- SWI-Prolog (para execução de algoritmos de planeamento)

## Integração do Sistema

O sistema APDL é composto por três módulos principais que trabalham em conjunto para fornecer uma solução completa de gestão de operações portuárias:

1. **Backend API (.NET)** - Servidor principal que gere toda a informação do porto (embarcações, docas, agentes de navegação, etc.) e utiliza uma base de dados SQLite para persistência. Este módulo fornece endpoints REST autenticados via Auth0.

2. **OEM System (Node.js)** - Servidor especializado para gestão de operações e execuções de visitas de embarcações. Utiliza MongoDB para armazenar planos de operação, execuções e incidentes. Este módulo comunica com o Backend API para obter informações sobre notificações de visitas de embarcações (VVNs) e executa algoritmos Prolog para gerar planos de operação otimizados.

3. **Frontend (Angular)** - Interface web que permite aos utilizadores interagir com o sistema. O frontend comunica tanto com o Backend API como com o OEM System através de chamadas HTTP/HTTPS, utilizando tokens JWT do Auth0 para autenticação. Inclui também um módulo de visualização 3D (localizado em `APDL.SPA/src/app/3d/`) que utiliza Three.js para gerar uma representação visual 3D do porto. Este módulo 3D faz requisições ao Backend API para obter uma estrutura JSON que define quais estruturas (docas, áreas de armazenamento, etc.) devem ser renderizadas e as suas posições na cena.

**Fluxo de Integração:**
- O utilizador acede ao frontend e autentica-se via Auth0
- O frontend faz requisições ao Backend API para obter dados do porto
- O módulo 3D solicita ao Backend API uma estrutura JSON com informações sobre as estruturas físicas do porto
- O módulo 3D utiliza Three.js para renderizar a cena baseada na estrutura JSON recebida
- O frontend faz requisições ao OEM System para gerar planos de operação
- O OEM System obtém dados de VVNs do Backend API quando necessário
- O OEM System executa algoritmos Prolog para calcular sequências otimizadas
- Todos os módulos validam tokens JWT do Auth0 para garantir segurança

### Instruções de Início

Para iniciar o sistema completo, é necessário executar os três módulos em terminais separados:

**Terminal 1 - Backend API (.NET):**
```bash
cd APDL.API
dotnet restore
dotnet run
```
O servidor estará disponível em `https://localhost:5001`

**Terminal 2 - OEM System (Node.js):**
```bash
cd APDL.OEM
npm install
npm run dev
```
O servidor estará disponível em `http://localhost:3000`

**Terminal 3 - Frontend (Angular):**
```bash
cd APDL.SPA
npm install
ng serve
```
A aplicação estará disponível em `http://localhost:4200`

### Modo de Produção (Frontend)

Para fazer o frontend comunicar com o Backend API em produção (ex: `10.9.11.75`):

**Nota:** De momento modo de produção não está a funcionar, a vm tem muita pouca memória e estou com dificuldades em fazer o deploy pela pipeline criada.

```bash
cd APDL.SPA
ng serve --configuration production
```

## Estrutura de Diretórios

```
LEI-SEM5-PI-2025-26-3NA-04/
├── APDL.API/                 # Backend principal (.NET)
│   ├── Controllers/          # Controladores REST
│   ├── Domain/               # Lógica de domínio (DDD)
│   ├── Infrastructure/       # Repositórios e configurações
│   └── Migrations/          # Migrações da base de dados
│
├── APDL.SPA/                # Frontend (Angular)
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/  # Componentes reutilizáveis
│   │   │   ├── pages/       # Páginas da aplicação
│   │   │   ├── 3d/          # Módulo de visualização 3D (Three.js)
│   │   │   ├── services/    # Serviços Angular
│   │   │   └── guards/      # Route guards
│   │   └── environments/    # Configurações de ambiente
│
├── APDL.OEM/                # Backend Node.js
│   ├── src/
│   │   ├── controllers/     # Controladores
│   │   ├── services/         # Serviços de negócio
│   │   ├── models/          # Modelos Mongoose
│   │   ├── routes/           # Definição de rotas
│   │   ├── middleware/       # Middleware Express
│   │   └── validators/      # Validadores
│
├── APDL.Planning/           # Algoritmos Prolog
│   └── algorithms/          # Ficheiros .pl
│
└── Documentation/           # Documentação do projeto
    ├── Sprint A/
    ├── Sprint B/
    └── Sprint C/
```

## Tecnologias Utilizadas

- **Backend API**: ASP.NET Core 8, Entity Framework Core, SQLite
- **OEM System**: Node.js, Express.js, MongoDB, Mongoose
- **Frontend**: Angular, TypeScript
- **Visualização 3D**: Three.js
- **Autenticação**: Auth0
- **Planeamento**: SWI-Prolog
- **Documentação API**: Swagger/OpenAPI


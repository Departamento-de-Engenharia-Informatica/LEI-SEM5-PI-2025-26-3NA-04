# Glossário - Sistema de Gestão Portuária

| Termo | Definição |
|-------|-----------|
| **Navio (Vessel)** | Embarcação que chega ao porto para operações de carga e descarga. Identificado por número IMO. |
| **Visita de Navio (Vessel Visit)** | Chegada e partida planeada de um navio ao porto, incluindo todas as operações associadas. |
| **Notificação de Visita** | Documento submetido pelo agente de navegação anunciando a chegada de um navio. |
| **Cais / Doca (Dock)** | Local de atracação onde navios são amarrados para carga/descarga. |
| **Calado (Draft)** | Profundidade do navio abaixo da linha de água. |
| **Contentor (Container)** | Unidade de carga padronizada segundo ISO 6346:2022. |
| **TEU** | Twenty-foot Equivalent Unit - unidade de medida padrão (1 contentor 20 pés = 1 TEU). |
| **Manifesto de Carga** | Lista de contentores a carregar ou descarregar de um navio. |
| **Manifesto de Descarregamento** | Lista de contentores a retirar do navio à chegada. |
| **Manifesto de Carregamento** | Lista de contentores a colocar no navio à partida. |
| **Grua STS** | Ship-to-Shore Crane - grua fixa no cais para carregar/descarregar navios. |
| **Grua de Pátio** | Grua móvel para movimentar contentores dentro do pátio de armazenamento. |
| **Camião Terminal** | Veículo para transportar contentores entre cais, pátios e armazéns. |
| **Equipamento Móvel** | Recursos móveis como empilhadeiras e veículos especializados. |
| **Pátio de Contentores** | Área de armazenamento temporário de contentores no porto. |
| **Armazém (Warehouse)** | Instalação coberta para carga que necessita inspeção ou manuseamento adicional. |
| **Agente de Navegação** | Organização que representa armadores ou operadores de navios nas operações portuárias. |
| **Representante do Agente** | Pessoa autorizada a submeter notificações em nome do agente de navegação. |
| **Janela Operacional** | Período semanal em que um recurso ou staff está disponível para operações. |
| **Capacidade Operacional** | Taxa de processamento de um recurso (ex: contentores por hora). |
| **Tempo de Setup** | Tempo necessário para preparar equipamento antes de iniciar operações. |
| **Qualificação (Qualification)** | Certificação ou competência necessária para operar equipamento específico. |
| **Staff Operacional** | Pessoal qualificado para operar recursos (operadores de grua, motoristas, etc.). |
| **Número Mecanográfico** | Identificador único do membro do staff. |
| **ETA (Estimated Time of Arrival)** | Hora estimada de chegada do navio ao porto. |
| **ETD (Estimated Time of Departure)** | Hora estimada de partida do navio do porto. |
| **Atraso (Delay)** | Diferença entre hora de partida real e desejada. Medido em horas. |
| **Estado de Disponibilidade** | Estado atual de um recurso: Disponível, Indisponível, Em Manutenção. |
| **Manutenção Programada** | Período planeado em que equipamento está indisponível para operações. |
| **Baía (Bay)** | Secção longitudinal da estrutura de carga do navio. |
| **Fila (Row)** | Posição transversal na estrutura de carga do navio. |
| **Nível (Tier)** | Camada vertical de contentores (acima ou abaixo do convés). |
| **Carga Perigosa (HAZMAT)** | Materiais perigosos que requerem manuseamento especial e oficiais de segurança. |
| **Oficial de Segurança** | Membro da tripulação designado para supervisionar carga perigosa. |
| **Capitão (Captain)** | Comandante do navio. |
| **Tripulação (Crew)** | Número total de pessoas a bordo do navio. |

---

## Papéis no Sistema

| Papel | Descrição |
|-------|-----------|
| **Oficial da Autoridade Portuária** | Revê e aprova/rejeita notificações de visita. Atribui cais aos navios. |
| **Representante do Agente de Navegação** | Submete notificações de visita e manifestos de carga. |
| **Operador Logístico** | Define e agenda tarefas operacionais. Aloca recursos. |
| **Administrador de Sistema** | Gere contas de utilizador e permissões. |

---

## Estados do Sistema

| Entidade | Estados Possíveis |
|----------|-------------------|
| **Notificação de Visita** | Em Progresso, Submetida, Aprovada, Rejeitada |
| **Recurso Físico** | Disponível, Indisponível, Em Manutenção |
| **Staff Operacional** | Disponível, Indisponível, Temporariamente Reatribuído |

---

## Termos Técnicos (DDD)

| Termo | Definição |
|-------|-----------|
| **Aggregate Root** | Entidade principal que controla acesso a outras entidades num agregado. |
| **Entity** | Objeto definido por identidade única que persiste ao longo do tempo. |
| **Value Object** | Objeto imutável definido pelos seus atributos, não por identidade. |
| **Repository** | Padrão para aceder e persistir aggregate roots na base de dados. |
| **Unit of Work** | Padrão para gerir transações de base de dados em múltiplas operações. |
| **DTO (Data Transfer Object)** | Objeto para transferir dados entre camadas da aplicação. |

---

## Acrónimos

| Acrónimo | Significado |
|----------|-------------|
| **IMO** | International Maritime Organization |
| **STS** | Ship-to-Shore (tipo de grua) |
| **RTG** | Rubber-Tired Gantry (grua sobre pneus) |
| **TEU** | Twenty-foot Equivalent Unit |
| **ETA** | Estimated Time of Arrival |
| **ETD** | Estimated Time of Departure |
| **HAZMAT** | Hazardous Materials |
| **ISO** | International Organization for Standardization |
| **IAM** | Identity and Access Management |
| **RBAC** | Role-Based Access Control |
| **SPA** | Single Page Application |
| **API** | Application Programming Interface |
| **REST** | Representational State Transfer |
| **CRUD** | Create, Read, Update, Delete |
| **GDPR** | General Data Protection Regulation |
| **CI/CD** | Continuous Integration / Continuous Deployment |
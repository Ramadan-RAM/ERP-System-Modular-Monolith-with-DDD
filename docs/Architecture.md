# Architecture Overview

The project follows a **Modular Monolith** architecture, designed with **Domain-Driven Design (DDD)** principles.  
While currently implemented as a modular monolith, the structure is prepared for **future decomposition into Microservices**.

---

## Bounded Contexts

- **HR Module**: Employee management, attendance, leave, payroll, payslip generation.  
- **Finance Module**: General Ledger, journal entries, purchases, cost centers, profit/loss forecasting.  
- **Users Module**: Authentication, profile, security questions, and role-based access.  
- **Logging Module**: Error and event tracking for IT support and auditing.  
- **CRM Module**: Reserved as a placeholder for future business expansion.  
- **SharedKernel**: Core abstractions, base classes, domain events, value objects, and cross-cutting utilities shared across modules.  

---

## Integrations

- **HR ↔ Finance via RabbitMQ**  
  - Creating a new employee in HR automatically creates a Finance account.  
  - Updating employee salary or profile in HR reflects in Finance.  

- **Hybrid Mode Support**  
  - In-memory messaging for local performance.  
  - RabbitMQ for distributed scenarios.  
  - Outbox Pattern ensures reliability and avoids message loss.  

---

## Databases

- Each module has its **own database schema** (Bounded Context).  
- Logging has a **dedicated database** for tracking and auditing.  
- Implemented using **EF Core Migrations + Seed Data**.  

**Schemas:**
- `HR_DB`  
- `Finance_DB`  
- `Users_DB`  
- `Logging_DB`  

---

## Tech Stack

- **Backend**: .NET Core 8, Entity Framework Core, MediatR  
- **Messaging**: RabbitMQ, MassTransit, InMemory transport  
- **Mapping**: AutoMapper (HR), Mapster (Finance)  
- **Frontend**: Angular CLI 19 (Standalone APIs) + AdminLTE Template  
- **Authentication**: JWT + Roles  

---

## Diagrams

### High-Level Architecture
```mermaid
flowchart TD
    subgraph SharedKernel [SharedKernel]
        SK[Base Entities, ValueObjects, Domain Events, Utilities]
    end

    subgraph HR [HR Module]
        HRA[HR.API] --> HRAppl[HR.Application]
        HRAppl --> HRD[HR.Domain]
        HRD --> HRI[HR.Infrastructure]
    end

    subgraph Finance [Finance Module]
        FA[Finance.API] --> FAppl[Finance.Application]
        FAppl --> FD[Finance.Domain]
        FD --> FI[Finance.Infrastructure]
    end

    subgraph Users [Users Module]
        UA[Users.API] --> UAppl[Users.Application]
        UAppl --> UD[Users.Domain]
        UD --> UI[Users.Infrastructure]
    end

    subgraph Logging [Logging Module]
        LA[Logging.API] --> LAppl[Logging.Application]
        LAppl --> LD[Logging.Domain]
        LD --> LI[Logging.Infrastructure]
    end

    %% Integrations
    HRA -- EmployeeCreated --> FA
    HRA -- PayrollPosted --> FA
    UA --> HRA
    UA --> FA
    LA --> HRA
    LA --> FA
    LA --> UA

    %% Shared Dependencies
    SK --> HRAppl
    SK --> FAppl
    SK --> UAppl
    SK --> LAppl

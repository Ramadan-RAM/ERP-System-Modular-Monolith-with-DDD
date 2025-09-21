# 🏢 ERPSys_DDD_ModularMonolith

A **Modular Monolith ERP System** built with **Domain-Driven Design (DDD)** principles.  
Supports **HR, Finance, User Management, and Logging** with cross-module integration.  
Frontend is built using **Angular 19 + AdminLTE**.

---

## 🌟 Highlights

- 🧩 **Architecture**: Modular Monolith + DDD Layers (Domain, Application, Infrastructure, Presentation).  
- 📦 **Messaging**: RabbitMQ, InMemory, or Hybrid transport (with **Outbox Pattern** for reliability).  
- 🔐 **Security**: JWT Authentication + Role-based Authorization.  
- 🧑‍💼 **HR Module**: Employee CRUD, Payroll, Leave Management.  
- 💰 **Finance Module**: GL Accounts, Journal Entries, Payroll Posting, Reports.  
- 👥 **User Management**: Security, Roles, and Permissions.  
- 📊 **Logging/Tracking**: Issue tracking & system monitoring module for IT/Production.  
- 🎨 **Frontend (Angular + AdminLTE)**:  
  - Login & Role-based auth  
  - Employee CRUD (MVP)  
- 🔀 **Mapping**:  
  - HR → `AutoMapper`  
  - Finance → `Mapster` (chosen for lightweight, high-performance mapping).

---

## 📂 Project Structure

```plaintext
ERPSys_DDD_ModularMonolith.Solution/
├── HR/
│   ├── HR.Application/
│   ├── HR.Domain/
│   ├── HR.Infrastructure/
│   └── HR.Presentation/
├── Finance/
│   ├── Finance.Application/
│   ├── Finance.Domain/
│   ├── Finance.Infrastructure/
│   └── Finance.Presentation/
├── Users/
│   ├── Users.Application/
│   ├── Users.Domain/
│   ├── Users.Infrastructure/
│   └── Users.Presentation/
├── Logging/
│   ├── Logging.Application/
│   ├── Logging.Domain/
│   └── Logging.Infrastructure/
├── CRM/ (planned)
├── ERP.API/ (composition root)
├── SharedKernel/ (Domain abstractions, messaging, persistence, events, etc.)

## Documentation

- [Architecture Overview](docs/Architecture.md)  
- [Roadmap](docs/Roadmap.md)  
- [Frontend Prototype](docs/FrontendPrototype.md)  
- [Future Work](docs/FutureWork.md)  
- [Implemented Pages](docs/ImplementedPages.md)  
- [Database Schema & ERD](docs/Database.md)  

## Status

| Module      | Status          | Notes                                      |
|-------------|-----------------|--------------------------------------------|
| HR          | Completed       | Employees, Attendance, Payroll, Payslip    |
| Finance     | In Progress     | GL, Journals, Purchases, Cost Centers      |
| Users       | Completed       | Login, Profile, Security Questions         |
| Logging     | Completed       | Error/Event Tracking                       |
| CRM         | Planned         | Placeholder for future extension           |

## Tech Stack

- Backend: .NET Core 8, Entity Framework Core, MediatR  
- Messaging: RabbitMQ / MassTransit  
- Mapping: AutoMapper, Mapster  
- Frontend: Angular CLI 19 (standalone APIs)  


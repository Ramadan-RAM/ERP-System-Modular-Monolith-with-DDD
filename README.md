# 🏢 ERP System - Modular Monolith with DDD

A **Modular Monolith ERP System** built with **Domain-Driven Design (DDD)** principles.  
The architecture is designed as a **Modular Monolith**, but structured in a way that allows future decomposition into **Microservices** if needed.  
Supports **HR, Finance, User Management, and Logging** with cross-module integration.  
Frontend is built using **Angular 19 + AdminLTE**.

---

## 🌟 Highlights

- 🧩 **Architecture**: Modular Monolith + DDD Layers (Domain, Application, Infrastructure, Presentation).  
- 📦 **Messaging**: RabbitMQ, InMemory, or Hybrid transport (with **Outbox Pattern** for reliability).  
- 🔐 **Security**: JWT Authentication + Role-based Authorization.  
- 👥 **User Management**: Security, Roles, and Permissions.  
- 🧑‍💼 **HR Module**: Employee CRUD, Payroll, Leave Management.  
- 💰 **Finance Module**: GL Accounts, Journal Entries, Payroll Posting, Reports.  
- 📊 **Logging/Tracking**: Issue tracking & system monitoring module for IT/Production.  
- 🎨 **Frontend (Angular + AdminLTE)**:  
  - Login & Role-based auth  
  - Employee CRUD (MVP)  
- 🔀 **Mapping**:  
  - HR → `AutoMapper`  
  - Finance → `Mapster` (chosen for lightweight, high-performance mapping).

---

## 📚 Documentation

- [Architecture Overview](docs/Architecture.md)  
- [Roadmap](docs/Roadmap.md)  
- [Frontend Prototype](docs/FrontendPrototype.md)  
- [Future Work](docs/FutureWork.md)  
- [Implemented Pages](docs/ImplementedPages.md)  
- [Database Schema & ERD](docs/Database.md)  

---

## 📌 Status

| Module      | Status          | Notes                                      |
|-------------|-----------------|--------------------------------------------|
| HR          | ✅ Completed     | Employees, Attendance, Payroll, Payslip    |
| Finance     | 🚧 In Progress   | GL, Journals, Purchases, Cost Centers      |
| Users       | ✅ Completed     | Login, Profile, Security Questions         |
| Logging     | ✅ Completed     | Error/Event Tracking                       |
| CRM         | 📝 Planned       | Placeholder for future extension           |

---

## ⚙️ Tech Stack
- 🖥️ Backend: .NET Core 8, Entity Framework Core, APIs , RabbitMQ
- 🗄️ Databases: SQL Server (HR_DB, Finance_DB, Security_DB, Logging_DB)
- 📡 Messaging: RabbitMQ, InMemory, Hybrid + Outbox Pattern
- 🔄 Mapping: AutoMapper (HR), Mapster (Finance)
- 🌐 Frontend: Angular CLI 19 (standalone APIs), AdminLTE Theme
- 🔑 Authentication: JWT + Roles

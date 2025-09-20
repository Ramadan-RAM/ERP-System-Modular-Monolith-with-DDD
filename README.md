# ERP System - Modular Monolith with DDD

This repository contains a self-study ERP system built using **.NET Core 8** and **Angular CLI 19**.  
The architecture follows **Modular Monolith** principles with **Domain-Driven Design (DDD)**.  

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


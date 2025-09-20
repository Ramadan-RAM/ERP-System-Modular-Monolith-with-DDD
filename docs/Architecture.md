# Architecture Overview

The project follows a **Modular Monolith** approach with **Domain-Driven Design (DDD)**.

## Bounded Contexts
- HR Module: Employees, attendance, leave, payroll.  
- Finance Module: General Ledger, journal entries, purchases, cost centers.  
- Users Module: Authentication, profile, security questions.  
- Logging Module: Error and event tracking.  
- CRM Module: Placeholder for future extension.  

## Integrations
- HR ↔ Finance integration via RabbitMQ.  
- Adding a new employee in HR creates a Finance account.  
- Updating salary or profile in HR updates Finance.  
- Supports Hybrid Mode: in-memory + message broker.  

## Databases
- Each module has its own schema (Bounded Context).  
- Logging uses a separate database for IT support and auditing.  
- EF Core migrations and seed data are included.  

## Tech Stack
- .NET Core 8  
- Entity Framework Core  
- MediatR  
- RabbitMQ / MassTransit  
- AutoMapper / Mapster  
- Angular CLI 19  


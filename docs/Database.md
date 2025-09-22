# Database Schema 

The system uses separate databases per bounded context (per module).  
Each module has its own schema for clear **bounded context isolation**.

---

## HR
- Employees  
- Branches  
- Departments  
- JobTitles  
- Attendance  
- FingerprintDevices  
- Leave  
- Payslips  
- OutboxMessages  

## Finance
- GLAccounts  
- DepartmentCostLink  
- JournalEntries  
- JournalLines  
- Currencies  
- ExchangeRates  
- GeneralExpenses  
- PurchaseOrders  
- PurchaseItems  
- InventorySnapshots  
- ProfitLossForecasts  
- CostCenters  

## Users
- Users  
- Roles  
- Permissions  
- UserRoles  
- UserPermissions  
- RolePermissions  
- UserProfiles  
- RefreshTokens  
- SecurityQuestions  
- UserSecurityAnswers  
- StoreBranches  

## Logging
- ProcessedEventLogs  
- ErrorLogs  

---

## ERD (Mermaid)

```mermaid
erDiagram
    %% HR Context
    Employee ||--o{ Attendance : "records"
    Employee ||--o{ Leave : "requests"
    Employee ||--o{ Payslip : "generates"
    Department ||--o{ Employee : "employs"
    Branch ||--o{ Department : "hosts"
    JobTitle ||--o{ Employee : "assigns"

    %% Finance Context
    GLAccount ||--o{ JournalLine : "maps"
    JournalEntry ||--o{ JournalLine : "contains"
    PurchaseOrder ||--o{ PurchaseItem : "includes"
    PurchaseItem ||--o{ InventorySnapshot : "tracked in"
    InventorySnapshot ||--o{ ProfitLossForecast : "predicts"
    Currency ||--o{ ExchangeRate : "defines"

    %% Users Context
    User ||--o{ UserProfile : "has"
    User ||--o{ UserRole : "assigned"
    Role ||--o{ UserRole : "contains"
    Role ||--o{ RolePermission : "grants"
    Permission ||--o{ RolePermission : "assigned"
    User ||--o{ UserPermission : "overrides"
    User ||--o{ RefreshToken : "issues"
    User ||--o{ UserSecurityAnswer : "answers"
    SecurityQuestion ||--o{ UserSecurityAnswer : "asked"

    %% Logging Context
    ProcessedEventLog ||--o{ ErrorLog : "relates"

    %% Shared Kernel
    OutboxMessage ||--o{ Employee : "integration events"
    OutboxMessage ||--o{ JournalEntry : "integration events"

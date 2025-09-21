# Database Schema

The system uses separate databases per bounded context.

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
- Permissions
- RefreshTokens
- RolePermissions
- Roles  
- SecurityQuestions
- UserSecurityAnswers
- StoreBranches
- UserPermissions
- UserProfiles
- UserRoles
- Users
- 

## Logging
- ProcessedEventLogs  
- ErrorLogs  

## ERD (Mermaid)

\\\mermaid
erDiagram
    HR ||--o{ Employee : has
    Employee ||--o{ Attendance : records
    Employee ||--o{ Leave : requests
    Employee ||--o{ Payslip : generates

    Finance ||--o{ GLAccount : contains
    JournalEntry ||--o{ JournalLine : includes
    GLAccount ||--o{ JournalLine : mapped
    PurchaseOrder ||--o{ PurchaseItem : contains
    PurchaseItem ||--o{ InventorySnapshot : tracks
    InventorySnapshot ||--o{ ProfitLossForecast : predicts
    Currency ||--o{ ExchangeRate : defines

    Users ||--o{ UserAccount : manages
    UserAccount ||--o{ SecurityQuestion : secures

    Logging ||--o{ ProcessedEventLog : logs
\\\



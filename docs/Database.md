# Database Schema

The system uses separate databases per bounded context.

## HR
- Employees  
- Departments  
- Attendance  
- Leave  
- Payslips  

## Finance
- GLAccounts  
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
- UserAccounts  
- SecurityQuestions  

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

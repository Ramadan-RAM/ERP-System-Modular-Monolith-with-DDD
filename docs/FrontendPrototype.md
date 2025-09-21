# Frontend Prototype

The frontend is built using **Angular CLI 19** with a modular structure.

## Structure
erp-frontend/  
├── src/  
│   ├── app/  
│   │   ├── core/ → shared services (interceptors, guards, auth)  
│   │   ├── shared/ → shared components (error pages, profile sidebar)  
│   │   ├── features/  
│   │   │   ├── hr/ → HR module  
│   │   │   ├── users/ → Users module  
│   │   │   ├── crm/ → CRM module  
│   │   │   ├── finances/ → Finance module  
│   │   │   └── logging/ → Logging module  
│   │   ├── layout/ → main layout (sidebar, header, footer)  
│   │   └── app.module.ts  
├── assets/  
└── environments/  

## Features
- Login page (authentication).  
- Profile page (photo, personal info, security questions).  
- Employees CRUD (HR module).
- 
  ![Alt Text](./images/login.png)
  ![Alt Text](./images/profile.png)
  ![Alt Text](./images/changepassword.png)
  ![Alt Text](./images/securirtyQuestion.png)
  ![Alt Text](./images/Employeeform.png)
  ![Alt Text](./images/confirmation.png)
  ![Alt Text](./images/don.png)



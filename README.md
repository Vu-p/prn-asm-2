# FUNewsManagementSystem (PRN222 Assignment 02)

A specialized News Management System (NMS) designed for universities to efficiently manage, organize, and publish educational content. This project follows the **3-Layer Architecture** and features **Real-time updates** using SignalR.

## 🚀 Key Features

- **Role-Based Access Control (RBAC):**
  - **Admin:** Manage accounts, view system-wide reports, and full access to management tools.
  - **Staff:** Manage categories, news articles, and personal profiles.
  - **Lecturer/Guest:** View active news articles with real-time updates.
- **Real-time News Feed:** Integrated with **SignalR** to dynamically update news lists (Cards/Table) without page reloads.
- **Advanced News Management:** 
  - Comma-separated tag system (auto-create/link).
  - Modal-based CRUD for seamless UX.
  - Category deletion restrictions (cannot delete categories with linked news).
- **User Registration:** Support for creating Staff and Lecturer accounts.
- **Reporting:** Admin tool to generate news statistics by date range.

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8.0 (Razor Pages)
- **Data Access:** Entity Framework Core (Repository Pattern)
- **Database:** Microsoft SQL Server
- **Real-time:** ASP.NET Core SignalR
- **Frontend:** Bootstrap 5, Vanilla JS, jQuery Validation
- **Architecture:** 3-Layer (Models -> Repositories -> Services -> Presentation)

## 📂 Project Structure

- `app/`: Razor Pages, Hubs, and Static Files (Presentation Layer).
- `services/`: Business logic and interfaces.
- `repositories/`: Database context, migrations, and repository implementations.
- `models/`: Domain entities and Enums.

## ⚙️ Setup Instructions

### 1. Database Configuration
Update the connection string in `app/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 2. Apply Migrations
Run the following command in the root directory:
```powershell
dotnet ef database update --project repositories --startup-project app
```

### 3. Running the Application
```powershell
dotnet run --project app
```

## 🔐 Default Credentials

- **Admin Account:**
  - Email: `admin@FUNewsManagementSystem.org`
  - Password: `@@abc123@@`
  - *(Loaded from appsettings.json)*

- **Default Staff (Seeded):**
  - Email: `staff@FUNewsManagementSystem.org`
  - Password: `staff123`

- **Default Lecturer (Seeded):**
  - Email: `lecturer@FUNewsManagementSystem.org`
  - Password: `lecturer123`

---
*Created by VuTra - Student @ FPT University*
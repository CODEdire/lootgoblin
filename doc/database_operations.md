# **Database Management Guide – LootGoblin Bot**

This document provides steps and best practices for managing the **Entity Framework Core (EF Core)** database operations for the LootGoblin project.

---

## **1. EF Core Tooling**

### **Installing EF Core CLI Tools**

Install EF Core tools globally (recommended for system-wide use):

```bash
dotnet tool install --global dotnet-ef
```

Update the tools to the latest version:

```bash
dotnet tool update --global dotnet-ef
```

> **Tip**: Omit `--global` to install tools locally for the project.

---

## **2. Managing Database Changes**

### **Create a New Migration**

Use migrations to track model changes in the database schema.

```bash
dotnet ef migrations add <MigrationName> \
  --startup-project src/LootGoblin.Bot \
  --output-dir Data/Migrations
```

**Parameters**:

- `<MigrationName>`: A descriptive name for your change (e.g., `AddLootPiles`, `UpdateEventStructure`).
- `--startup-project`: The entry point project that hosts the EF `DbContext` (in this case, the bot).
- `--output-dir`: Directory to store generated migration files.

---

### **Drop Database (Local Development Only!)**

**Warning**: This will permanently delete your database and all data.

```bash
dotnet ef database drop --startup-project src/LootGoblin.Bot
```

The app can regenerate the schema from scratch during development.

---

### **Update Database**

Apply all pending migrations to your database:

```bash
dotnet ef database update --verbose \
  --startup-project src/LootGoblin.Bot
```

---

### **Generate Update or Create Script**

For controlled deployment or CI/CD pipelines, generate SQL scripts for migrations:

#### **Script for Specific Migration Range**
```bash
dotnet ef migrations script <FromMigration> <ToMigration> \
  --startup-project src/LootGoblin.Bot \
  --output ./data/scripts/Update-<version>.sql
```

- `<FromMigration>`: Name of the starting migration (or `0` for an empty database).
- `<ToMigration>`: Target migration name (or omit for latest).

#### **Create Full Schema Script**
```bash
dotnet ef migrations script 0 \
  --startup-project src/LootGoblin.Bot \
  --output ./data/scripts/Create-<latest Version>.sql
```

---

## **3. Database Optimization**

After creating or updating your model, precompile it for **faster startup and NativeAOT compatibility**:

```bash
dotnet ef dbcontext optimize \
  --startup-project src/LootGoblin.Bot \
  -o ./Data/CompiledModels \
  --nativeaot
```

This command:

- Generates a compiled model for improved runtime performance.
- Prepares for **NativeAOT publishing**.
- Creates an optional precompiled query set (**experimental** — see note below).

---

## **4. Deployment Notes**

- Always verify migrations in a **staging environment** before applying to production.
- For production deployment, consider:
  - `dotnet ef migrations script` to generate SQL scripts for controlled execution.
  - Avoid `dotnet ef database update` directly on production servers.

---

### **Useful Commands Recap**

| Action                    | Command                                                                                                  |
| ------------------------- | -------------------------------------------------------------------------------------------------------- |
| Install EF Tools          | `dotnet tool install --global dotnet-ef`                                                                 |
| Update EF Tools           | `dotnet tool update --global dotnet-ef`                                                                  |
| Add Migration             | `dotnet ef migrations add <Name> --startup-project src/LootGoblin.Bot`                                   |
| Drop Database (Local)     | `dotnet ef database drop --startup-project src/LootGoblin.Bot`                                           |
| Apply Migrations          | `dotnet ef database update --startup-project src/LootGoblin.Bot`                                         |
| Optimize (Compiled Model) | `dotnet ef dbcontext optimize --startup-project src/LootGoblin.Bot -o ./Data/CompiledModels --nativeaot` |


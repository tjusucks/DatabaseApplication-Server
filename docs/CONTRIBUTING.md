# Contributing to This Project

## Pre-requisites

### Install Dotnet SDK

下载并安装 Dotnet SDK 9.0: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

验证安装:

```bash
dotnet --version
```

输出应为 `9.0.x`.

### Install Oracle Database

下载并安装 Oracle Database 23ai Free: [https://www.oracle.com/database/free/get-started/](https://www.oracle.com/database/free/get-started/)

### Install Redis / Valkey

安装 Redis 或者 Valkey, Valkey 是 Redis 的开源分支, 用于数据库缓存:

[https://valkey.io/download/](https://valkey.io/download/)

## Configure Oracle Database

### First Time Setup

在 Oracle 数据库第一次启动时, 需要为 SYS 用户配置密码:

```bash
sqlplus / as sysdba
```

```sql
ALTER USER sys IDENTIFIED BY password;
```

### Create User for Development

创建一个用于开发的用户, 并授予必要的权限, 保证 Migrations 能够正常运行:

```bash
sqlplus sys/password@localhost:1521/FREEPDB1 as sysdba
```

其中 `password` 是你为 SYS 用户设置的密码.

```sql
CREATE USER username IDENTIFIED BY password;

GRANT CONNECT, RESOURCE TO username;
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO username;
GRANT DROP ANY TABLE, ALTER ANY TABLE TO username;
ALTER USER username QUOTA UNLIMITED ON users;
```

其中 `username` 和 `password` 是你为新用户设置的用户名和密码.

## Configure Connection String

在 `src/Presentation/appsettings.{Environment}.json` 中设置连接字符串:

```json
"ConnectionStrings": {
  "OracleConnection": "Data Source=localhost:1521/FREEPDB1;User ID=username;Password=password;",
  "RedisConnection": "localhost:6379"
}
```

其中 `username` 和 `password` 是你在上一步创建的用户的用户名和密码.

## Use Environment Variables

为保证安全性和灵活性, 建议不要在配置文件中写入真实的数据库密码等敏感信息. 可以通过环境变量或 `.env` 文件覆盖配置文件中的连接字符串.

### Use `.env` File

本地开发推荐使用 `.env` 文件来管理环境变量.

在 `src/Presentation` 目录下复制 `.env.example` 为 `.env`, 并填写实际信息:

```bash
cp src/Presentation/.env.example src/Presentation/.env
```

编辑 `.env` 文件内容, 例如:

```bash
ConnectionStrings__OracleConnection="Data Source=localhost:1521/FREEPDB1;User ID=<yourusername>;Password=<yourpassword>;"
ConnectionStrings__RedisConnection="localhost:6379"
```

`.env` 文件中的变量会在应用启动时自动加载, 并覆盖 `appsettings.json` 中的同名配置.

### Use Environment Variables Directly

在服务器或 CI/CD 环境中, 可以直接设置环境变量, 无需 `.env` 文件. 例如:

```bash
# Linux / macOS
export ConnectionStrings__OracleConnection="Data Source=localhost:1521/FREEPDB1;User ID=yourusername;Password=yourpassword;"
export ConnectionStrings__RedisConnection="localhost:6379"
```

```powershell
# Windows
$env:ConnectionStrings__OracleConnection="Data Source=localhost:1521/FREEPDB1;User ID=yourusername;Password=yourpassword;"
$env:ConnectionStrings__RedisConnection="localhost:6379"
```

```yaml
# GitHub Actions.
env:
  ConnectionStrings__OracleConnection: ${{ secrets.ORACLE_CONNECTION }}
  ConnectionStrings__RedisConnection: ${{ secrets.REDIS_CONNECTION }}
```

## Run the Project

### Start the Web API Project

在项目根目录下运行以下命令:

```bash
# Linux / macOS
dotnet run --project src/Presentation/Presentation.csproj --launch-profile "https"
```

```bash
# Windows
dotnet run --project src\Presentation\Presentation.csproj --launch-profile "https"
```

或者直接进入 `src/Presentation` 目录运行:

```bash
# Linux / macOS
cd src/Presentation
dotnet run --launch-profile "https"
```

```bash
# Windows
cd src\Presentation
dotnet run --launch-profile "https"
```

### Access the API

* 访问 OpenAPI 文档: [https://localhost:7220/openapi/v1.json](https://localhost:7220/openapi/v1.json)
* 访问 Scalar API Client: [https://localhost:7220/scalar/v1](https://localhost:7220/scalar/v1)

## Database Migrations

修改了数据库模型后, 需要使用 dotnet ef 工具进行数据库迁移:

安装 dotnet ef 工具:

```bash
dotnet tool restore
```

生成迁移:

```bash
# Linux / macOS
dotnet ef migrations add MigrationName \
    --project src/Infrastructure/Infrastructure.csproj \
    --startup-project src/Presentation/Presentation.csproj
```

```bash
# Windows
dotnet ef migrations add MigrationName `
    --project src\Infrastructure\Infrastructure.csproj `
    --startup-project src\Presentation\Presentation.csproj
```

其中 `MigrationName` 是你为迁移指定的名称, 类似 Git 提交信息, 但是是 PascalCase, 例如 `AddNewTable` 或 `UpdateColumnType`.

应用迁移:

```bash
# Linux / macOS
dotnet ef database update \
    --project src/Infrastructure/Infrastructure.csproj \
    --startup-project src/Presentation/Presentation.csproj
```

```bash
# Windows
dotnet ef database update `
    --project src\Infrastructure\Infrastructure.csproj `
    --startup-project src\Presentation\Presentation.csproj
```

在 Development 环境下, 运行 Web API 时会自动应用迁移.

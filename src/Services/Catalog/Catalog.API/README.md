--目录数据库迁移脚本
dotnet ef migrations add initial  -c CatalogContext -o Infrastructure/CatalogMigrations
dotnet ef database update -c CatalogContext
--一体化事件日志数据库迁移脚本
dotnet ef migrations add initial  -c IntegrationEventLogEF.IntegrationEventLogContext -o Infrastructure/IntegrationEventMigrations
dotnet ef database update -c IntegrationEventLogEF.IntegrationEventLogContext
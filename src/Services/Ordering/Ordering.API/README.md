--目录数据库迁移脚本
dotnet ef migrations add initial  -c OrderingContext -o Infrastructure/OrderingMigrations
dotnet ef database update -c OrderingContext
--一体化事件日志数据库迁移脚本
dotnet ef migrations add initial  -c IntegrationEventLogContext -o Infrastructure/IntegrationEventMigrations
dotnet ef database update -c IntegrationEventLogContext
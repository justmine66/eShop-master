--Ŀ¼���ݿ�Ǩ�ƽű�
dotnet ef migrations add initial  -c CatalogContext -o Infrastructure/CatalogMigrations
dotnet ef database update -c CatalogContext
--һ�廯�¼���־���ݿ�Ǩ�ƽű�
dotnet ef migrations add initial  -c IntegrationEventLogEF.IntegrationEventLogContext -o Infrastructure/IntegrationEventMigrations
dotnet ef database update -c IntegrationEventLogEF.IntegrationEventLogContext
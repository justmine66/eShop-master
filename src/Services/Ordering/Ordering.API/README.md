--Ŀ¼���ݿ�Ǩ�ƽű�
dotnet ef migrations add initial  -c OrderingContext -o Infrastructure/OrderingMigrations
dotnet ef database update -c OrderingContext
--һ�廯�¼���־���ݿ�Ǩ�ƽű�
dotnet ef migrations add initial  -c IntegrationEventLogEF.IntegrationEventLogContext -o Infrastructure/IntegrationEventMigrations
dotnet ef database update -c IntegrationEventLogEF.IntegrationEventLogContext
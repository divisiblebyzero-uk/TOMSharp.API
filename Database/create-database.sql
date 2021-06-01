create LOGIN tomsharpapp with password = 'TomSharpApp1'
GO
create database tomsharp
GO
use tomsharp
GO
create user tomsharpapp for login tomsharpapp
GO
exec sp_addrolemember 'db_owner', 'tomsharpapp'
GO
CREATE TABLE [dbo].[Clients]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Money] INT NOT NULL, 
    [Department] INT NOT NULL, 
    [Deposit] INT NOT NULL
)

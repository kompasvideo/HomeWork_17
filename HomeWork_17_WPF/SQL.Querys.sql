CREATE TABLE [dbo].[Clients] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [Money]      INT           NOT NULL,
    [Department] INT           NOT NULL,
    [Deposit]    INT           NOT NULL,
    [DateOpen]   DATE          ,
    [Days]       INT           ,
    [Rate]       FLOAT         ,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

SET IDENTITY_INSERT [dbo].[Clients] ON
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (1, N'Физ. лицо - 6b083', 5384, 1, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (2, N'Физ. лицо - 58416', 2300, 1, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (3, N'Физ. лицо - dd2e8', 9810, 1, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (4, N'Юр. лицо - 354f7', 1474, 2, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (5, N'Юр. лицо - b64f8', 7126, 2, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (6, N'Юр. лицо - 6caee', 9249, 2, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (7, N'VIP - 07b30', 9728, 3, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (8, N'VIP - e4e0d', 9853, 3, 0)
INSERT INTO [dbo].[Clients] ([Id], [Name], [Money], [Department], [Deposit]) VALUES (9, N'VIP - e0dee', 4591, 3, 0)
SET IDENTITY_INSERT [dbo].[Clients] OFF





CREATE TABLE [dbo].[Departments] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

SET IDENTITY_INSERT [dbo].[Departments] ON
INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (1, N'Физ. лицо')
INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (2, N'Юр. лицо')
INSERT INTO [dbo].[Departments] ([Id], [Name]) VALUES (3, N'VIP')
SET IDENTITY_INSERT [dbo].[Departments] OFF




CREATE TABLE [dbo].[Deposit] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [dbo].[Deposit] ([Id], [Name]) VALUES (1, N'вклад без капитализации %')
INSERT INTO [dbo].[Deposit] ([Id], [Name]) VALUES (2, N'вклад с капитализацией %')


CREATE TABLE [dbo].[ModuleType] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ModuleType] PRIMARY KEY CLUSTERED ([Id] ASC)
);


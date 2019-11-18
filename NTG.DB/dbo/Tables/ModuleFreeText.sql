CREATE TABLE [dbo].[ModuleFreeText] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ModuleFreeText_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);


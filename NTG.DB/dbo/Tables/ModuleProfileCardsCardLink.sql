CREATE TABLE [dbo].[ModuleProfileCardsCardLink] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Text]                     NVARCHAR (15)  NOT NULL,
    [Url]                      NVARCHAR (MAX) NOT NULL,
    [Position]                 INT            NOT NULL,
    [ModuleProfileCardsCardId] INT            NOT NULL,
    CONSTRAINT [PK_ModuleProfileCardsCardLink] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleProfileCardsCardLink_ModuleProfileCardsCard_ModuleProfileCardsCardId] FOREIGN KEY ([ModuleProfileCardsCardId]) REFERENCES [dbo].[ModuleProfileCardsCard] ([Id])
);


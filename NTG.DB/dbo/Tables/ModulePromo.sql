CREATE TABLE [dbo].[ModulePromo] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Image]       NVARCHAR (MAX) NOT NULL,
    [Text]         NVARCHAR(200)     NOT NULL,
    [ButtonText]       NVARCHAR (20)  NOT NULL,
    [ButtonLink]       NVARCHAR (MAX) NOT NULL,
    [ImageRight] BIT NOT NULL, 
    [TitleTop] BIT NOT NULL, 
    CONSTRAINT [PK_ModulePromo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

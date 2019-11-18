CREATE TABLE [dbo].[ModuleBoxesBox] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Title]         NVARCHAR (25) NOT NULL,
    [Icon]          NVARCHAR (8)  NOT NULL,
    [Color]         NVARCHAR (20) NOT NULL,
    [Text]          NVARCHAR (95) NOT NULL,
	[Url] NVARCHAR(MAX) NULL, 
    [Position]      INT           NOT NULL,
    [ModuleBoxesId] INT           NOT NULL,
    CONSTRAINT [PK_ModuleBoxesBox] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleBoxesBox_ModuleBoxes_ModuleBoxesId] FOREIGN KEY ([ModuleBoxesId]) REFERENCES [dbo].[ModuleBoxes] ([Id])
);


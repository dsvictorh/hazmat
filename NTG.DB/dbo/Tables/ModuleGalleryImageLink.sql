CREATE TABLE [dbo].[ModuleGalleryImageLink] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Text]                     NVARCHAR (15)  NOT NULL,
    [Image]                      NVARCHAR (MAX) NOT NULL,
    [Position]                 INT            NOT NULL,
    [ModuleGalleryImageId] INT            NOT NULL,
    CONSTRAINT [PK_ModuleGalleryImageLink] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleGalleryImageLink_ModuleGalleryImage_ModuleGalleryImageId] FOREIGN KEY ([ModuleGalleryImageId]) REFERENCES [dbo].[ModuleGalleryImage] ([Id])
);

CREATE TABLE [dbo].[ErrorLog] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Date]         DATETIMEOFFSET       NOT NULL,
	[Type]         NVARCHAR(MAX) NOT NULL DEFAULT '',
    [StackTrace]   NVARCHAR (MAX) NULL,
    [Message]      NVARCHAR (MAX) NULL,
    [IPAddress]    NVARCHAR (MAX) NULL,
    [Browser]      NVARCHAR (MAX) NULL,
    [Object]       NVARCHAR (MAX) NULL,
    [Function]     NVARCHAR (MAX) NOT NULL,
    [Line]         INT            NULL,
    [HResult]      NVARCHAR (MAX) NULL,
    [InnerMessage] NVARCHAR (MAX) NULL,
    [Important]    BIT            NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);



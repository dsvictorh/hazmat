CREATE TABLE [dbo].[Admin] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Email]     NVARCHAR (100) NOT NULL,
    [Password]  CHAR (64)      NOT NULL,
    [FirstName] NVARCHAR (50)  NOT NULL,
    [LastName]  NVARCHAR (50)  NOT NULL,
    [Active]    BIT            NOT NULL,
    [RoleId]    INT            NOT NULL,
    CONSTRAINT [PK_AdminUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Admin_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id])
);




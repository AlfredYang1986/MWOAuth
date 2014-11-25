
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/17/2014 19:00:39
-- Generated from EDMX file: C:\Users\yy\Desktop\authorizationserver\trunk\MWAuthorizationServer\AuthorizationDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MWOAuthDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AccessToken_Client]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccessToken] DROP CONSTRAINT [FK_AccessToken_Client];
GO
IF OBJECT_ID(N'[dbo].[FK_UserChangePasswordToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ChangePasswordTokenSet] DROP CONSTRAINT [FK_UserChangePasswordToken];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessTokenThirdPartyToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ThirdPartyToken] DROP CONSTRAINT [FK_AccessTokenThirdPartyToken];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccessToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccessToken] DROP CONSTRAINT [FK_UserAccessToken];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AccessToken]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccessToken];
GO
IF OBJECT_ID(N'[dbo].[Client]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Client];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[ThirdPartyToken]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdPartyToken];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[ChangePasswordTokenSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ChangePasswordTokenSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AccessToken'
CREATE TABLE [dbo].[AccessToken] (
    [access_token] nvarchar(50)  NOT NULL,
    [expire_to] int  NOT NULL,
    [client_id] nvarchar(50)  NOT NULL,
    [refresh_token] nvarchar(50)  NULL,
    [access_token_id] int  NOT NULL,
    [user_id] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Client'
CREATE TABLE [dbo].[Client] (
    [client_id] nvarchar(50)  NOT NULL,
    [client_seret] nvarchar(30)  NOT NULL,
    [scope] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'ThirdPartyToken'
CREATE TABLE [dbo].[ThirdPartyToken] (
    [Id] int  NOT NULL,
    [third_party_token] nvarchar(500)  NOT NULL,
    [third_party_user_id] nvarchar(50)  NOT NULL,
    [token_provider] nvarchar(50)  NOT NULL,
    [access_token_id] int  NOT NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [user_id] nvarchar(50)  NOT NULL,
    [username] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'ChangePasswordTokenSet'
CREATE TABLE [dbo].[ChangePasswordTokenSet] (
    [ChpToken_Id] int IDENTITY(1,1) NOT NULL,
    [expired] int  NOT NULL,
    [user_id] nvarchar(50)  NOT NULL,
    [ChpToken] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [access_token_id] in table 'AccessToken'
ALTER TABLE [dbo].[AccessToken]
ADD CONSTRAINT [PK_AccessToken]
    PRIMARY KEY CLUSTERED ([access_token_id] ASC);
GO

-- Creating primary key on [client_id] in table 'Client'
ALTER TABLE [dbo].[Client]
ADD CONSTRAINT [PK_Client]
    PRIMARY KEY CLUSTERED ([client_id] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [Id] in table 'ThirdPartyToken'
ALTER TABLE [dbo].[ThirdPartyToken]
ADD CONSTRAINT [PK_ThirdPartyToken]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [user_id] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([user_id] ASC);
GO

-- Creating primary key on [ChpToken_Id] in table 'ChangePasswordTokenSet'
ALTER TABLE [dbo].[ChangePasswordTokenSet]
ADD CONSTRAINT [PK_ChangePasswordTokenSet]
    PRIMARY KEY CLUSTERED ([ChpToken_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [client_id] in table 'AccessToken'
ALTER TABLE [dbo].[AccessToken]
ADD CONSTRAINT [FK_AccessToken_Client]
    FOREIGN KEY ([client_id])
    REFERENCES [dbo].[Client]
        ([client_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessToken_Client'
CREATE INDEX [IX_FK_AccessToken_Client]
ON [dbo].[AccessToken]
    ([client_id]);
GO

-- Creating foreign key on [user_id] in table 'ChangePasswordTokenSet'
ALTER TABLE [dbo].[ChangePasswordTokenSet]
ADD CONSTRAINT [FK_UserChangePasswordToken]
    FOREIGN KEY ([user_id])
    REFERENCES [dbo].[User]
        ([user_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserChangePasswordToken'
CREATE INDEX [IX_FK_UserChangePasswordToken]
ON [dbo].[ChangePasswordTokenSet]
    ([user_id]);
GO

-- Creating foreign key on [access_token_id] in table 'ThirdPartyToken'
ALTER TABLE [dbo].[ThirdPartyToken]
ADD CONSTRAINT [FK_AccessTokenThirdPartyToken]
    FOREIGN KEY ([access_token_id])
    REFERENCES [dbo].[AccessToken]
        ([access_token_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessTokenThirdPartyToken'
CREATE INDEX [IX_FK_AccessTokenThirdPartyToken]
ON [dbo].[ThirdPartyToken]
    ([access_token_id]);
GO

-- Creating foreign key on [user_id] in table 'AccessToken'
ALTER TABLE [dbo].[AccessToken]
ADD CONSTRAINT [FK_UserAccessToken]
    FOREIGN KEY ([user_id])
    REFERENCES [dbo].[User]
        ([user_id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccessToken'
CREATE INDEX [IX_FK_UserAccessToken]
ON [dbo].[AccessToken]
    ([user_id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
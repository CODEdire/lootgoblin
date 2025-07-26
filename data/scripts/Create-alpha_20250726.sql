IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [GuildEvents] (
    [Id] int NOT NULL IDENTITY,
    [GuildId] decimal(20,0) NOT NULL,
    [MessageId] decimal(20,0) NULL,
    [OriginChannelId] decimal(20,0) NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(2048) NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [StartedAt] datetimeoffset NULL,
    [CompletedAt] datetimeoffset NULL,
    [CreatedBy] decimal(20,0) NOT NULL,
    [StartedBy] decimal(20,0) NULL,
    [CompletedBy] decimal(20,0) NULL,
    [MinimumParticipantMinutes] int NULL,
    [MaximumParticipants] int NULL,
    [CurrentState] int NOT NULL,
    [ParticipantChannels] nvarchar(max) NULL,
    CONSTRAINT [PK_GuildEvents] PRIMARY KEY ([Id])
);

CREATE TABLE [GuildSettings] (
    [Id] decimal(20,0) NOT NULL,
    [EventChannelId] decimal(20,0) NULL,
    [LootChannelId] decimal(20,0) NULL,
    [EventOrganizerRoleId] decimal(20,0) NULL,
    [EventParticipantRoleId] decimal(20,0) NULL,
    CONSTRAINT [PK_GuildSettings] PRIMARY KEY ([Id])
);

CREATE TABLE [EventParticipants] (
    [Id] int NOT NULL IDENTITY,
    [EventId] int NOT NULL,
    [UserId] decimal(20,0) NOT NULL,
    [TotalParticipation] time NOT NULL,
    [ExcludedFromLoot] bit NOT NULL,
    CONSTRAINT [PK_EventParticipants] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EventParticipants_GuildEvents_EventId] FOREIGN KEY ([EventId]) REFERENCES [GuildEvents] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [LootPiles] (
    [Id] int NOT NULL IDENTITY,
    [EventId] int NOT NULL,
    [MessageId] decimal(20,0) NULL,
    [OriginChannelId] decimal(20,0) NULL,
    [Name] nvarchar(256) NOT NULL,
    [Description] nvarchar(2048) NULL,
    [CurrentStatus] tinyint NOT NULL,
    [LootType] tinyint NOT NULL,
    [RollType] tinyint NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL,
    [CompletedAt] datetimeoffset NULL,
    [CreatedBy] decimal(20,0) NOT NULL,
    [CompletedBy] decimal(20,0) NULL,
    CONSTRAINT [PK_LootPiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LootPiles_GuildEvents_EventId] FOREIGN KEY ([EventId]) REFERENCES [GuildEvents] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ParticipantSessions] (
    [Id] int NOT NULL IDENTITY,
    [EventId] int NOT NULL,
    [EventParticipantId] int NULL,
    [UserId] decimal(20,0) NOT NULL,
    [ChannelId] decimal(20,0) NOT NULL,
    [StartedAt] datetimeoffset NOT NULL,
    [EndedAt] datetimeoffset NULL,
    CONSTRAINT [PK_ParticipantSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ParticipantSessions_GuildEvents_EventId] FOREIGN KEY ([EventId]) REFERENCES [GuildEvents] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_EventParticipants_EventId] ON [EventParticipants] ([EventId]);

CREATE INDEX [IX_LootPiles_EventId] ON [LootPiles] ([EventId]);

CREATE INDEX [IX_ParticipantSessions_EventId] ON [ParticipantSessions] ([EventId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250726152759_InitialCreate', N'9.0.7');

COMMIT;
GO


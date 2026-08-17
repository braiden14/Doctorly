ALTER TABLE Events
    ADD
        IsDeleted BIT NOT NULL
    CONSTRAINT DF_Events_IsDeleted DEFAULT 0,
    IsCancelled BIT NOT NULL
        CONSTRAINT DF_Events_IsCancelled DEFAULT 0;

GO

ALTER TABLE EventHistory
    ADD
        IsDeleted BIT NOT NULL
    CONSTRAINT DF_EventHistory_IsDeleted DEFAULT 0,
    IsCancelled BIT NOT NULL
        CONSTRAINT DF_EventHistory_IsCancelled DEFAULT 0;
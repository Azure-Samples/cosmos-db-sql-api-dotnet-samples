// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

public record TodoItem(
    string id,
    string partitionKey,
    string description,
    bool done,
    int priority
);
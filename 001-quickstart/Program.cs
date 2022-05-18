// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

// <using_directives> 
using Microsoft.Azure.Cosmos;
// </using_directives>

// <client_credentials> 
// New instance of CosmosClient class
using CosmosClient client = new(
    accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")!,
    authKeyOrResourceToken: Environment.GetEnvironmentVariable("COSMOS_KEY")!
);
// </client_credentials>

// <new_database> 
// Database reference with creation if it does not already exist
Database database = await client.CreateDatabaseIfNotExistsAsync(
    id: "tododatabase"
);

Console.WriteLine($"New database:\t{database.Id}");
// </new_database>

// <new_container> 
// Container reference with creation if it does not alredy exist
Container container = await database.CreateContainerIfNotExistsAsync(
    id: "taskscontainer",
    partitionKeyPath: "/partitionKey",
    throughput: 400
);

Console.WriteLine($"New container:\t{container.Id}");
// </new_container>

// <new_item> 
// Create new object and upsert (create or replace) to container
TodoItem newItem = new(
    id: "fb59918b-fb3d-4549-9503-38bee83a6e1d",
    partitionKey: "personal-tasks-user-88033a55",
    description: "Dispose of household trash",
    done: false,
    priority: 2
);

TodoItem createdItem = await container.UpsertItemAsync<TodoItem>(
    item: newItem,
    partitionKey: new PartitionKey("personal-tasks-user-88033a55")
);

Console.WriteLine($"Created item:\t{createdItem.id}\t[{createdItem.partitionKey}]");
// </new_item>

// <query_items> 
// Create query using a SQL string and parameters
var query = new QueryDefinition(
    query: "SELECT * FROM todo t WHERE t.partitionKey = @key"
)
    .WithParameter("@key", "personal-tasks-user-88033a55");

using FeedIterator<TodoItem> feed = container.GetItemQueryIterator<TodoItem>(
    queryDefinition: query
);

while (feed.HasMoreResults)
{
    FeedResponse<TodoItem> response = await feed.ReadNextAsync();
    foreach (TodoItem item in response)
    {
        Console.WriteLine($"Found item:\t{createdItem.description}");
    }
}
// </query_items>

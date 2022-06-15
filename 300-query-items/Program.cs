// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

// <using_directives> 
using Microsoft.Azure.Cosmos;
// </using_directives>

// <endpoint_key> 
// New instance of CosmosClient class using an endpoint and key string
using CosmosClient client = new(
    accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")!,
    authKeyOrResourceToken: Environment.GetEnvironmentVariable("COSMOS_KEY")!
);
// </endpoint_key>

// <create_database>
// New instance of Database class referencing the server-side database
Database database = await client.CreateDatabaseIfNotExistsAsync(
    id: "adventureworks"
);
// </create_database>

// <create_container>
// New instance of Container class referencing the server-side container
Container container = await database.CreateContainerIfNotExistsAsync(
    id: "products",
    partitionKeyPath: "/category",
    throughput: 400
);
// </create_container>

// <create_items> 
// Create new items and add to container
Product firstNewItem = new(
    id: "68719518388",
    category: "gear-surf-surfboards",
    name: "Sunnox Surfboard",
    quantity: 8,
    sale: true
);

Product secondNewitem = new(
    id: "68719518398",
    category: "gear-surf-surfboards",
    name: "Noosa Surfboard",
    quantity: 15,
    sale: false
);

await container.CreateItemAsync<Product>(
    item: firstNewItem,
    partitionKey: new PartitionKey("gear-surf-surfboards")
);

await container.CreateItemAsync<Product>(
    item: secondNewitem,
    partitionKey: new PartitionKey("gear-surf-surfboards")
);
// </create_items> 

// <query_items_sql>
// Query multiple items from container
var query = new QueryDefinition(
    query: "SELECT * FROM products p WHERE p.partitionKey = @key"
)
    .WithParameter("@key", "gear-surf-surfboards");

using FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
    queryDefinition: query
);

while (feed.HasMoreResults)
{
    FeedResponse<Product> response = await feed.ReadNextAsync();
    foreach (Product item in response)
    {
        Console.WriteLine($"Found item:\t{item.name}");
    }
}
// </query_items_sql>

// <query_items_linq>
// Get LINQ IQueryable object
IOrderedQueryable<Product> queryable = container.GetItemLinqQueryable<Product>();

// Construct LINQ query
var matches = queryable
    .Where(p => p.category == "gear-surf-surfboards")
    .Where(p => p.sale == false)
    .Where(p => p.quantity > 10);

// Iterate over query results
foreach (Product item in matches)
{
    Console.WriteLine($"Matched item:\t{item.name}");
}
// </query_items_linq>
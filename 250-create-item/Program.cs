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

// <create_item> 
// Create new item and add to container
Product newItem = new(
    id: "68719518388",
    category: "gear-surf-surfboards",
    name: "Sunnox Surfboard",
    quantity: 8,
    sale: true
);

Product createdItem = await container.CreateItemAsync<Product>(
    item: newItem,
    partitionKey: new PartitionKey("gear-surf-surfboards")
);
// </create_item> 
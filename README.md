# MongoDb Relational

## Description

This is a simple **MongoDb** Relational ORM that lets you create and use Models as you would declare them for Entity Framework.

Each child of a model will be saved in it's own document/collection. For each Model, a collection will be created. Each collection will have the same type of documents. 

A parent will only keep a reference to it's child, not the entire model( [Referenced Relationships](https://docs.mongodb.com/manual/tutorial/model-referenced-one-to-many-relationships-between-documents/) )

<br/>

## Usage

```
public void ConfigureServices(IServiceCollection services)
{          
    services.AddMongoRelational(new MongoConnectionProvider
    {
        Database = "testdb",
        Url = "mongodb://localhost:27017"
    });
}
```

<br/>


## Examples
```
class Product : MongoModel
{
        public string Name { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; }
        public string Telephone { get; set; }
        public Producer Producer { get; set; }
}

class Producer : MongoModel
{
        public string Name { get; set; }
}


class Service 
{

    private readonly IMongoRelationalClient mongoRelationalClient;

    public Service(IMongoRelationalClient mongoRelationalClient) 
    {
        this.mongoRelationalClient = mongoRelationalClient;
    }

    public async Task Insert(Product p)
    {
        await mongoRelationalClient.SaveOneAsync(p);    
    }
}

```

Adding AddMongoRelational will inject as Transient MongoRelationalClass in App's DI Scope

<br/>


## Versions

### v1

Added support for single operations(Insert,Update,Delete,Get).
<br/> To be implemented: 
* Collections
* Set all references to an object on null when removing that object
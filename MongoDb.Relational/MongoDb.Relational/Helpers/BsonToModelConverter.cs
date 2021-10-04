using MongoDb.Relational.Constants;
using MongoDb.Relational.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDb.Relational.Helpers
{
    public class BsonToModelConverter
    {
        public static async Task<T> Convert<T>(BsonDocument bsonDocument, MethodInfo getModelCall, object caller) where T: MongoModel, new()
        {
            var result = new T();
            var referenceProperties = result.GetType().GetProperties().Where(p => !PrimitiveChecker.IsPrimitive(p));
            var primitivesProperties = result.GetType().GetProperties().Where(p => PrimitiveChecker.IsPrimitive(p) && p.Name != ModelConstants.IdIdentifier);


            result.ID = (string)bsonDocument.GetValue("_id");
            AddPrimitiveProperties(bsonDocument, result, primitivesProperties);
            await AddReferencePrimitives(bsonDocument, getModelCall, result, referenceProperties, caller);

            return result;
        }

        private static void AddPrimitiveProperties<T>(BsonDocument bsonDocument, T result, IEnumerable<System.Reflection.PropertyInfo> primitivesProperties) where T : MongoModel, new()
        {
            foreach (var prop in primitivesProperties)
            {
                prop.SetValue(result, System.Convert.ChangeType(bsonDocument.GetValue(prop.Name), prop.PropertyType));
            }
        }

        private static async Task AddReferencePrimitives<T>(BsonDocument bsonDocument, MethodInfo getModelCall, T result, IEnumerable<PropertyInfo> referenceProperties, object caller) where T : MongoModel, new()
        {
            foreach (var prop in referenceProperties)
            {
                var bsonChildId = bsonDocument.GetValue(prop.Name).AsBsonDocument.GetValue("ID");
                
                if(bsonChildId.IsBsonNull)
                {
                    continue;
                }

                FilterDefinition<BsonDocument> f = $"{{  _id : \"{ bsonChildId.AsObjectId }\"  }}";

                dynamic getCall = getModelCall
                    .MakeGenericMethod(prop.PropertyType)
                    .Invoke(caller, new[] { f });

                var child = await getCall;

                prop.SetValue(result, child);
            }
        }
    }
}

using MongoDb.Relational.Constants;
using MongoDb.Relational.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace MongoDb.Relational.Helpers
{
    public class ConvertionResult
    {
        public BsonDocument BsonDocument { get; private set; }
        public ICollection<MongoModel> Childs { get; private set; }

        public string ModelId { get; private set; }

        public ConvertionResult(BsonDocument doc, ICollection<MongoModel> childs, string id)
        {
            BsonDocument = doc;
            Childs = childs;
            ModelId = id;
        }
    }

    public static class ModelToBsonConverter
    {
        public static ConvertionResult Convert(object model)
        {
            var referenceProperties = model.GetType().GetProperties().Where(p => !PrimitiveChecker.IsPrimitive(p));
            var primitivesProperties = model.GetType().GetProperties().Where(p => PrimitiveChecker.IsPrimitive(p) && p.Name != ModelConstants.IdIdentifier);
            var childs = new List<MongoModel>();

            var newObject = new ExpandoObject() as IDictionary<string, object>;

            newObject.Add("_id", model.GetType().GetProperty(ModelConstants.IdIdentifier).GetValue(model)
                                 ?? ObjectId.GenerateNewId().ToString());

            AddPrimitiveProperties(model, primitivesProperties, newObject);
            AddReferenceProperties(model, referenceProperties, newObject, childs);

            return new ConvertionResult(newObject.ToBsonDocument(), childs, newObject["_id"].ToString());
        }

        private static void AddPrimitiveProperties(object model, IEnumerable<PropertyInfo> primitivesProperties, IDictionary<string, object> newObject)
        {
            foreach (var prop in primitivesProperties)
            {
                newObject.Add(prop.Name, prop.GetValue(model));
            }
        }

        private static void AddReferenceProperties(object model, IEnumerable<PropertyInfo> referenceProperties, IDictionary<string, object> newObject, List<MongoModel> childs)
        {
            foreach (var prop in referenceProperties)
            {
                MongoModel child = prop.GetValue(model) as MongoModel;

                if(child is null)
                {
                    throw new ArgumentException($"All children must derive {nameof(MongoModel)}");
                }

                var childId = child.ID is null ?
                    ObjectId.GenerateNewId():
                    ObjectId.Parse(child.ID);

                child.ID = childId.ToString(); //If child had no id, assign the new one. Else preserve it

                childs.Add(child);
                newObject.Add(prop.Name, new { ID = childId }); //Instead of saving the entire child, save only the reference to it
            }
        }
    }
}

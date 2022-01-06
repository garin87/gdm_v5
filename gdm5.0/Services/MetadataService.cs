using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace gdm5._0.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly DataContext _context;
        public MetadataService(DataContext context)
        {
            _context = context;
        }

        public dynamic GetMetadataTypes()
        {
            Dictionary<string, object> dbMetadata = LoadDBMetaData();
            var jsonRules = LoadViewRules();
            var viewRules = ParseViewRulesFile(jsonRules);

            var metaData = MergeMetadataWithViewRules(dbMetadata, viewRules);

            return metaData;
        }

        public dynamic MergeMetadataWithViewRules(Dictionary<string, object>  dbMetadata,
                                     Dictionary<string, Dictionary<string, Property>> viewRules)
        {
            var metadata = new Dictionary<string, object> { };
            var listViewRules = viewRules;

            foreach (var entity in dbMetadata)
            {
                if (listViewRules.Any(el => el.Key == entity.Key))
                {
                    var viewRule = new Dictionary<string, Property> { };
                    if (listViewRules.ContainsKey(entity.Key))
                    {
                        viewRule = listViewRules[entity.Key];
                    }

                    var listProperties = new List<Property> { };

                    foreach (Property property in (IList<Property>)entity.Value)
                    {
                        Property field = null;

                        if (viewRule.ContainsKey(property.Name))
                        {
                            field = viewRule[property.Name];
                        }

                        if (field != null)
                        {
                            var prop = new Property()
                            {
                                Name = property.Name != null ? property.Name : field.Name,
                                Type = property.Type != null ? property.Type : field.Type,
                                TypeView = property.TypeView != null ? property.TypeView : field.TypeView,
                                DefaultValue = property.DefaultValue != null ? property.DefaultValue : field.DefaultValue,
                                DisplayedName = property.DisplayedName != null ? property.DisplayedName : field.DisplayedName,
                                Order = property.Order != null ? property.Order : field.Order,
                                ReadOnly = property.ReadOnly != null ? property.ReadOnly : field.ReadOnly,
                                Required = property.Required != null ? property.Required : field.Required,
                                Hidden = property.Hidden != null ? property.Hidden : field.Hidden,
                                Editor = property.TypeView != null ? property.TypeView : field.TypeView,
                                Provider = property.Provider != null ? property.Provider : field.Provider,
                            };
                            listProperties.Add(prop);
                        }
                        else
                        {
                            listProperties.Add(property);
                        }
                    }


                    foreach (var property in viewRule)
                    {
                        if (!(entity.Value as List<Property>).Any(el => el.Name == property.Key))
                        {
                            listProperties.Add(property.Value);
                        }
                    }

                    metadata.Add(entity.Key, listProperties);
                }
                else
                {
                    metadata.Add(entity.Key, entity.Value);
                }
            }

            foreach (var entity in listViewRules)
            {
                if (!dbMetadata.Any(el => el.Key == entity.Key))
                {
                    var entityValues = new List<Property>{ };
                    foreach (var item in entity.Value)
                    {
                        entityValues.Add(item.Value);
                    }
                    
                    metadata.Add(entity.Key, entityValues);
                };
            }

            return metadata;
        }
        public dynamic LoadDBMetaData()
        {
            var modelTypes = _context.Model.GetEntityTypes().Select(el => new
            {
                EntityName = el.ClrType.Name,
                Entity = el.ClrType.GetProperties().Select(item =>
                        new Property
                        {
                            Name = item.Name != "Id" && item.PropertyType.Name != "ICollection`1" ? item.Name : "",
                            Type = item.Name != "Id" && item.PropertyType.Name != "ICollection`1" ? item.PropertyType.Name : "",
                        }).ToList()
            }).ToList();

            var metaData = new Dictionary<string, object> { };

            foreach (var entity in modelTypes)
            {
                foreach (var item in entity?.Entity.ToList())
                {
                    if (item.Name == "" || item.Name.Contains("Id") || (item.Name == item.Type))
                        entity.Entity.Remove(item);

                }

                if (entity.EntityName != "User" && entity.EntityName != "Role")
                    metaData.Add(entity.EntityName, entity.Entity);
            }

            return metaData;
        }


        private string LoadViewRules()
        {
            string currentPath = Directory.GetCurrentDirectory();
            string directoryPath = @$"{currentPath}\ClientApp\dist\content\metadata\";
            string path = @$"{directoryPath}viewRules.json";

            var jsonResult = File.ReadAllText(path);


            return jsonResult;
        }

        public dynamic ParseViewRulesFile(string JsonViewRulesFile)
        {
            var options = new JsonSerializerOptions { };
            var v1 = JsonSerializer.Deserialize<Dictionary<string, JsonDocument>>(JsonViewRulesFile, options);
           
            var entity = new Dictionary<string, Dictionary<string, Property>>();

            foreach (var d in v1)
            {
                // if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))

                var item = d.Value.RootElement;
                var t = JsonSerializer.Deserialize<Dictionary<string, JsonDocument>>(item.ToString(), options);
                var property = new Dictionary<string, Property>();
                foreach (var val in t)
                {
                    var rr = JsonSerializer.Deserialize<Property>(val.Value.RootElement.GetRawText());
                    property.Add(val.Key, rr);
                }

                entity.Add(d.Key, property);
            }

            return entity;

        }
    }
}

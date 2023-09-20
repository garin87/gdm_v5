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

        public Dictionary<string, object> GetMetadataTypes()
        {
            var dbMetadata = LoadDBMetaData();
            var jsonRules = LoadViewRules();
            var viewRules = ParseViewRulesFile(jsonRules);

            return MergeMetadataWithViewRules(dbMetadata, viewRules);
        }


        //public Dictionary<string, object> MergeMetadataWithViewRules(Dictionary<string, object> dbMetadata,
        //                             Dictionary<string, Dictionary<string, Property>> viewRules)
        //{
        //    var metadata = new Dictionary<string, object>();

        //    foreach (var (entityKey, entityValue) in dbMetadata)
        //    {
                
        //        if (viewRules.TryGetValue(entityKey, out var viewRule))
        //        {
        //            var listProperties = entityValue is IList<Property> properties ? properties.ToList() : new List<Property>();

        //            foreach (var property in viewRule.Values)
        //            {
        //                if (listProperties.All(p => p.Name != property.Name))
        //                {
        //                    listProperties.Add(property);
        //                }
        //            }

        //            var mergedProperties = listProperties.Select(property =>
        //            {
        //                if (viewRule.TryGetValue(property.Name, out var field))
        //                {
        //                    return new Property
        //                    {
        //                        Name = property.Name ?? field.Name,
        //                        Type = property.Type ?? field.Type,
        //                        TypeView = property.TypeView ?? field.TypeView,
        //                        DefaultValue = property.DefaultValue ?? field.DefaultValue,
        //                        DisplayedName = property.DisplayedName ?? field.DisplayedName,
        //                        Order = property.Order ?? field.Order,
        //                        ReadOnly = property.ReadOnly ?? field.ReadOnly,
        //                        Required = property.Required ?? field.Required,
        //                        Hidden = property.Hidden ?? field.Hidden,
        //                        Editor = property.TypeView ?? field.TypeView,
        //                        Provider = property.Provider ?? field.Provider,
        //                        category = property.category ?? field.category,
        //                        parentName = property.parentName ?? field.parentName ?? entityKey,
                                
        //                    };
        //                }

        //                return property;
        //            });

        //            metadata[entityKey] = mergedProperties;
        //        }
        //        else
        //        {
        //            metadata[entityKey] = entityValue;
        //        }
        //    }

        //    foreach (var (viewKey, viewValue) in viewRules)
        //    {
        //        if (!dbMetadata.ContainsKey(viewKey))
        //        {
        //            var entityValues = viewValue.Values.ToList();
        //            metadata[viewKey] = entityValues;
        //        }
        //    }

        //    return metadata;
        //}

        public Dictionary<string, object> MergeMetadataWithViewRules(Dictionary<string, object> dbMetadata,
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
                                category = property.category != null ? property.category : field.category,
                                parentName = property.parentName ?? field.parentName,
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
                    var entityValues = new List<Property> { };
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
            var modelTypes = _context.Model.GetEntityTypes()
                .Select(el => new {
                    EntityName = el.ClrType.Name,
                    Entity = el.ClrType.GetProperties()
                        .Select(item => new Property
                        {
                            Name = item.Name != "Id" && item.PropertyType.Name != "ICollection`1" ? item.Name : null,
                            Type = item.Name != "Id" && item.PropertyType.Name != "ICollection`1" ? item.PropertyType.Name : null,
                        })
                        .Where(p => !string.IsNullOrEmpty(p.Name) && !p.Name.Contains("Id") && p.Name != p.Type)
                        .ToList()
                })
                .Where(entity => entity.EntityName != "User" && entity.EntityName != "Role")
                .ToDictionary(entity => entity.EntityName, entity => (object)entity.Entity);

            return modelTypes;
        }

        private string LoadViewRules()
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist", "content", "metadata");
            string path = Path.Combine(directoryPath, "viewRules.json");

            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading view rules from file '{path}': {ex.Message}", ex);
            }
        }

        public dynamic ParseViewRulesFile(string jsonViewRulesFile)
        {
            var options = new JsonSerializerOptions();
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonDocument>>(jsonViewRulesFile, options);
            var entity = new Dictionary<string, Dictionary<string, Property>>();

            foreach (var kvp in dictionary)
            {
                var item = kvp.Value.RootElement;
                var properties = JsonSerializer.Deserialize<Dictionary<string, Property>>(item.ToString(), options);

                entity.Add(kvp.Key, properties);
            }

            return entity;
        }

    }
}

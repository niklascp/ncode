using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.Data
{
    public class Schema
    {
        private SchemaDefinition definition;

        public Schema() { }

        public Schema(string name, string definitionFile)
        {
            Name = name;
            DefinitionFile = definitionFile;
        }

        public SchemaDefinition Definition
        {
            get
            {
                if (definition == null)
                {
                    definition = new SchemaDefinition();
                    definition.Load(DefinitionFile);
                }

                return definition;
            }
        }

        public string Name { get; set; }
        public string DefinitionFile { get; set; }
        public Version CurrentVersion { get; set; }
    }
}

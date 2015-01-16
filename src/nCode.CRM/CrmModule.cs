using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.Configuration;
using nCode.Data;

namespace nCode.CRM
{    
	public class CrmModule : Module
	{
        public override decimal Version
        {
            get
            {
                return 2.21m;
            }
        }

        public override void Upgrade()
        {
            /* Uninstall old CRM schema, if installed. */
            var schema = SchemaControl.Schemas.SingleOrDefault(x => string.Equals(x.Name, "CRM", StringComparison.InvariantCultureIgnoreCase));
            if (schema != null)
                SchemaControl.Uninstall("CRM");

            /* Ensure Schema based on DbContext */
            SchemaDefinition sd = new SchemaDefinition();
            sd.LoadFromDbContext(typeof(CrmDbContext));
            sd.UpdateDatabase();
        }
	}
}

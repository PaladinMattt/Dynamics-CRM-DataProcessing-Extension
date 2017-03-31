using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
namespace Dynamics.CRM.DataProcessing.Extension
{
    class Dynamics_CRM_AttributeData
    {
        public string EntityName { get; set; }
        public string EntityAlias { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public AttributeTypeCode Type { get; set; }

        public string ColumnName
        {
            get
            {
               return CalculateColumnName();
            }
        }

        public Dynamics_CRM_AttributeData()
        {
            EntityAlias = null;
            Alias = null;
        }


        private string CalculateColumnName()
        {
            StringBuilder builder = new StringBuilder();
            if (!String.IsNullOrEmpty(Alias))
            {
                builder.Append(Alias);
            }
            else
            {
                if (!String.IsNullOrEmpty(EntityAlias))
                {
                    builder.Append(EntityAlias);
                    builder.Append("_");
                }
                builder.Append(Name);
            }

            return builder.ToString();
        }
    }
}

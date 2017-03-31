using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Data;

using System.Collections.ObjectModel;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.DataProcessing;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Dynamics.CRM.DataProcessing.Extension
{
    public class Dynamics_CRM_DataReader : Microsoft.ReportingServices.DataProcessing.IDataReader
    {
        //DataTable holds a batch of entitys in it.
        private DataTable datatable;

        private int PageNumber { get; set; }
        private string PagingCookie { get; set; }
        private int FetchCount { get; set; }

       private List<Dynamics_CRM_AttributeData> AttributeData;

        protected string FetchXML { get; set; }

        protected Dynamics_CRM_Connection service { get; set; }
        protected EntityCollection collection { get; private set; }


        public DataTable Results { get { return datatable; } }

        protected int Current { get; set; }

        public int FieldCount
        {
            get
            {
                return datatable.Columns.Count;
            }
        }

        public string GetName(int fieldIndex)
        {
            return datatable.Columns[fieldIndex].ColumnName;
        }

        public int GetOrdinal(string fieldName)
        {
            return datatable.Columns.IndexOf(fieldName);
        }

        public bool Read()
        {
            if (Current >= datatable.Rows.Count && collection.MoreRecords)
            {
                PullBatch();
                PutBatchInDataTable();

                //Set Current to -1 since it will be incremented in the next if statement.
                Current = -1;
            }
            if(Current < datatable.Rows.Count - 1 )
            {
                Current++;
                return true;
            }
            return false;
        }

        protected void PutBatchInDataTable()
        {
            foreach(Entity e in collection.Entities)
            {
                DataRow row = datatable.NewRow();
                foreach(KeyValuePair<string,object> kvp in e.Attributes)
                {
                    if(datatable.Columns.Contains(kvp.Key))
                    {
                        Dynamics_CRM_AttributeData ad = AttributeData.Where(x => x.ColumnName == kvp.Key).FirstOrDefault();
                        object Value = kvp.Value;
                        if(kvp.Value.GetType() == typeof(AliasedValue))
                        {
                            Value = ((AliasedValue)kvp.Value).Value;
                        }
                    
                        if (ad != null)
                        {
                            switch (ad.Type)
                            {
                                case AttributeTypeCode.Lookup:
                                case AttributeTypeCode.Owner:
                                case AttributeTypeCode.Customer:
           
                                    row[kvp.Key] = ((EntityReference)Value).Name;
                                    row[kvp.Key + "Value"] = ((EntityReference)Value).Id;
                                    row[kvp.Key + "EntityName"] = ((EntityReference)Value).LogicalName;
                                    break;
                                case AttributeTypeCode.Picklist:
                                case AttributeTypeCode.State:
                                case AttributeTypeCode.Status:
                                    row[kvp.Key] = e.FormattedValues[kvp.Key];
                                    row[kvp.Key + "Value"] = ((OptionSetValue)Value).Value;
                                    break;
                                case AttributeTypeCode.Money:
                                    row[kvp.Key + "Value"] = ((Money)Value).Value;
                                    row[kvp.Key] = e.FormattedValues[kvp.Key]; 
                                    break;
                                default:
                                    if (e.FormattedValues.ContainsKey(kvp.Key))
                                    {
                                        row[kvp.Key] = e.FormattedValues[kvp.Key];
                                    }
                                    else
                                    {
                                        row[kvp.Key] = Value.ToString();
                                    }

                                    if (datatable.Columns.Contains(kvp.Key + "Value"))
                                    {
                                        row[kvp.Key + "Value"] = Value;
                                    }
                                    break;
                            }
                        }
                    }
                }
                datatable.Rows.Add(row);
            }
        }

        protected void PullBatch()
        {
            string xml = CreateXml(FetchXML, PagingCookie, PageNumber, FetchCount);

            // Excute the fetch query and get the xml result.
            RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
            {
                Query = new FetchExpression(xml)
            };

            collection = ((RetrieveMultipleResponse)service.Execute(fetchRequest1)).EntityCollection;

            // Check for morerecords, if it returns 1.
            if (collection.MoreRecords)
            {
                // Increment the page number to retrieve the next page.
                PageNumber++;

                // Set the paging cookie to the paging cookie returned from current results.                            
                PagingCookie = collection.PagingCookie;
            }
            else
            {
                // If no more records in the result nodes, exit the loop.
            }
        }

        public Type GetFieldType(int fieldIndex)
        {
            return datatable.Columns[fieldIndex].DataType;
        }

        public object GetValue(int fieldIndex)
        {
            return datatable.Rows[Current][fieldIndex];
        }

        public Dynamics_CRM_DataReader(string fetch, Dynamics_CRM_Connection connection)
        {
            datatable = new DataTable();
            FetchXML = fetch;
            service = connection;
            AttributeData = new List<Dynamics_CRM_AttributeData>();

            PopulateMeta();
            PullBatch();
            PutBatchInDataTable();
        }


        private void PopulateAttributesFromXML(string fetch)
        {
            XDocument doc = XDocument.Parse(fetch);
            XElement root = doc.Root;
  
        }


        private void PopulateMeta()
        {
            XDocument doc = XDocument.Parse(FetchXML);
            RetrieveMetaData(doc, "entity");
            RetrieveMetaData(doc, "link-entity");          

        }

        private void RetrieveMetaData(XDocument doc, string nodeName)
        {
            List<XElement> namenode = doc.Descendants(nodeName).ToList();
            foreach (XElement element in namenode)
            {
                string entity = element.Attribute("name").Value;
                string entityalias = null;
                if (element.Attribute("alias") != null)
                {
                    entityalias = element.Attribute("alias").Value;
                }
                RetrieveEntityRequest request = new RetrieveEntityRequest();
                request.LogicalName = entity;
                request.EntityFilters = EntityFilters.Attributes;
                RetrieveEntityResponse response = (RetrieveEntityResponse)service.Execute(request);

                if (element.Element("all-attributes") != null)
                {
                    foreach (AttributeMetadata att in response.EntityMetadata.Attributes)
                    {
                        Dynamics_CRM_AttributeData ad = new Dynamics_CRM_AttributeData();
                        ad.Name = att.LogicalName;
                        ad.EntityName = entity;
                        ad.EntityAlias = entityalias;
                        AddAttribute(response.EntityMetadata.Attributes, ad);
                    }
                }
                else
                {
                    List<XElement> attributes = element.Elements("attribute").ToList();
                    foreach (XElement attribute in attributes)
                    {
                        string attributeAlias = null;
                        string attributeName = attribute.Attribute("name").Value;
                        if (attribute.Attribute("alias") != null)
                        {
                            attributeAlias = attribute.Attribute("alias").Value;
                        }
                        Dynamics_CRM_AttributeData ad = new Dynamics_CRM_AttributeData();
                        ad.Alias = attributeAlias;
                        ad.Name = attributeName;
                        ad.EntityAlias = entityalias;
                        ad.EntityName = entity;
                        AddAttribute(response.EntityMetadata.Attributes, ad);
                    }
                }
            }
        }


        private void AddAttribute(AttributeMetadata[] attributes, Dynamics_CRM_AttributeData attributeData)
        {
            AttributeMetadata theAttribute = attributes.Where(x => x.LogicalName == attributeData.Name).FirstOrDefault();
            if (theAttribute != null)
            {
     
                if (!datatable.Columns.Contains(attributeData.ColumnName))
                {
                    switch (theAttribute.AttributeType)
                    {
                        case AttributeTypeCode.BigInt:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(long));
                            attributeData.Type = AttributeTypeCode.BigInt;
                            break;
                        case AttributeTypeCode.Boolean:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(bool));
                            attributeData.Type = AttributeTypeCode.Boolean;
                            break;
                        case AttributeTypeCode.CalendarRules:
                            break;
                        case AttributeTypeCode.DateTime:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(DateTime));
                            attributeData.Type = AttributeTypeCode.DateTime;
                            break;
                        case AttributeTypeCode.Decimal:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(decimal));
                            attributeData.Type = AttributeTypeCode.Decimal;
                            break;
                        case AttributeTypeCode.Double:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(double));
                            attributeData.Type = AttributeTypeCode.Double;
                            break;
                        case AttributeTypeCode.EntityName:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            attributeData.Type = AttributeTypeCode.EntityName;
                            break;
                        case AttributeTypeCode.Integer:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(int));
                            attributeData.Type = AttributeTypeCode.Integer;
                            break;
                        case AttributeTypeCode.ManagedProperty:

                            break;
                        case AttributeTypeCode.Memo:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            attributeData.Type = AttributeTypeCode.Memo;
                            break;
                        case AttributeTypeCode.Money:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(decimal));
                            attributeData.Type = AttributeTypeCode.Money;
                            break;
                        case AttributeTypeCode.Owner:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(Guid));
                            datatable.Columns.Add(attributeData.ColumnName + "EntityName", typeof(string));
                            attributeData.Type = AttributeTypeCode.Owner;
                            break;
                        case AttributeTypeCode.PartyList:
                            break;
                        case AttributeTypeCode.State:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(int));
                            attributeData.Type = AttributeTypeCode.State;
                            break;
                        case AttributeTypeCode.Status:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(int));
                            attributeData.Type = AttributeTypeCode.Status;
                            break;
                        case AttributeTypeCode.Uniqueidentifier:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(Guid));
                            attributeData.Type = AttributeTypeCode.Uniqueidentifier;
                            break;
                        case AttributeTypeCode.Virtual:
                   //         datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Customer:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(Guid));
                            datatable.Columns.Add(attributeData.ColumnName + "EntityName", typeof(string));
                            attributeData.Type = AttributeTypeCode.Customer;
                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Lookup:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(Guid));
                            datatable.Columns.Add(attributeData.ColumnName + "EntityName", typeof(string));
                            attributeData.Type = AttributeTypeCode.Lookup;
                            break;
                        case AttributeTypeCode.Picklist:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            datatable.Columns.Add(attributeData.ColumnName + "Value", typeof(long));
                            attributeData.Type = AttributeTypeCode.Picklist;
                            break;
                        case AttributeTypeCode.String:
                        default:
                            datatable.Columns.Add(attributeData.ColumnName, typeof(string));
                            attributeData.Type = AttributeTypeCode.String;
                            break;
                    }
                    AttributeData.Add(attributeData);
                }
            }
        }

    

    
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Dynamics_CRM_DataReader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



        protected string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        protected string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }


    }
}

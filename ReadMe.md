
<!DOCTYPE html>
<html lang="en">
  <head>
  </head>
  <body>
<h1>**IMPORTANT**</h1>
<p>
Please note that although this currently works when running the queries and report from Visual Studio 2015 Report builder, 
Dynamics CRM will give you an Error if you try to upload this into CRM.  This is due to the slightly different formats of SSDT
(which is probably why Microsoft has not released a product for visual studio 2015)
You can actually get around that by editing the .rdl files xml, and mimicking that of SSDT 2012.
Since many people won't want to do this, I do NOT recommand using this for production use, merely as a learning tool.
</p>

<h2>How to Install</h2>
<p>
Copy the Dynamics.CRM.DataProcessing.Extension.dll, Microsoft.CRM.Sdk.Proxy.dll, and the Microsoft.XRM.Sdk.dll
files into 
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies
</p>


<h3>Edit the file:</h3>
<p>
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\RSReportDesigner.config
and under the Data node add the line:
&lt;Extension Name="Dynamics 365 Data Extension" Type="Dynamics.CRM.DataProcessing.Extension.Dynamics_CRM_Connection,Dynamics.CRM.DataProcessing.Extension"/&gt;
<p>


<h3>Edit the file:</h3>
<p>
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\RSPreviewPolicy.config
and add the following code group </br>
 &lt;CodeGroup</br>
    class="UnionCodeGroup"</br>
    version="1"</br>
    PermissionSetName="FullTrust"</br>
    Name="Dynamics365DataExtensionCodeGroup"</br>
    Description="Code group for custom data processing extension"&gt;</br>
    &lt;IMembershipCondition</br>
    class="UrlMembershipCondition"</br>
        version="1"</br>
        Url="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies\Dynamics.CRM.DataProcessing.Extension.dll" /&gt;</br>
    &lt;/CodeGroup&gt;
</p>


<h1> Edits that need made to the .rdl file if you want to import it into CRM </h1>
<p> I have included 2 sample reports, one with Microsofts Data Provider (Report 2012.rdl)
   and one with mine (Report 2015.rdl) to see the differences. For ease of use, i have also included an edited 2015 file,
   and a pdf of the diff.
<p>
<ul>
  <li>Remove the Attribute <i>MustUnderstand="df"</i> and <i>xmlns:df="http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily"</i> from the Report Node</li>
  <li>In the <i>Report</i> node, change the <i>xmlns</i> attribute to <i>http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition</i></li>
  <li>Remove the node <i>&lt;df:DefaultFontFamily&gt;Segoe UI&lt;/df:DefaultFontFamily&gt;</i></li>
  <li>Change the DataProvider from <i>Dynamics 365 Data Extension (Wont Work in CRM...)</i> to <i>MSCRMFETCH</i></li>
  <li>Remove the nodes <i>ReportSections</i> and <i>ReportSection</i> leaving the <i>Body</i> and all child nodes.</li>
  <li>Remove the nodes <i>ReportParametersLayout</i> and all child nodes. 
</ul>
<p> Now it should import into CRM.</p>
</body>
</html>
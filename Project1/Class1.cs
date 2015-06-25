using System;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Tasks.FileSystemTask;
using Microsoft.SqlServer.Dts.Tasks.BulkInsertTask;
using PipeLineWrapper = Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using RuntimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Microsoft.SqlServer.Dts.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
                    
            // Create the package and Data Flow Component

            Package package = new Package();
            Connections pkgConns = package.Connections;

            Executable e = package.Executables.Add("STOCK:PipelineTask");
            TaskHost thMainPipe = e as TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;
            Console.Write(thMainPipe.Name);
            thMainPipe.Name = "Data Flow Task";



            // Add the Flat File connection
            ConnectionManager connectionManagerFlatFile = package.Connections.Add("FLATFILE");
            connectionManagerFlatFile.ConnectionString = @"C:\Temp\FlatFile.txt";
            connectionManagerFlatFile.Name = "FlatFile";
            connectionManagerFlatFile.Properties["Format"].SetValue(connectionManagerFlatFile, "Delimited");
            connectionManagerFlatFile.Properties["ColumnNamesInFirstDataRow"].SetValue(connectionManagerFlatFile, true);

            // Get native flat file connection 
            RuntimeWrapper.IDTSConnectionManagerFlatFile100 connectionFlatFile = connectionManagerFlatFile.InnerObject as RuntimeWrapper.IDTSConnectionManagerFlatFile100;
        


            // Create the source component.  
            IDTSComponentMetaData100 source =
              dataFlowTask.ComponentMetaDataCollection.New();
            source.ComponentClassID = "DTSAdapter.FlatFileSource";
            CManagedComponentWrapper srcDesignTime = source.Instantiate();
            srcDesignTime.ProvideComponentProperties();
            
            IDTSComponentMetaData100 scd = 
                dataFlowTask.ComponentMetaDataCollection.New();
            scd.ComponentClassID = "DTSTransform.SCD";
            scd.Name = "SCD";
            scd.Description = "This is STD";
            CManagedComponentWrapper SCD_Wrapper = scd.Instantiate();
            SCD_Wrapper.ProvideComponentProperties();

            // Create the destination component.
            IDTSComponentMetaData100 destination =
              dataFlowTask.ComponentMetaDataCollection.New();
            destination.ComponentClassID = "DTSAdapter.OleDbDestination";
            CManagedComponentWrapper destDesignTime = destination.Instantiate();
            destDesignTime.ProvideComponentProperties();

            // Create the path.
            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(source.OutputCollection[0],
              scd.InputCollection[0]);

            IDTSPath100 path2 = dataFlowTask.PathCollection.New();
            path2.AttachPathAndPropagateNotifications(scd.OutputCollection[0],
              destination.InputCollection[0]);

            //  string pkg = @"C:\Program Files\Microsoft SQL Server\100\Samples\Integration Services\Package Samples\ExecuteProcess Sample\ExecuteProcess\UsingExecuteProcess.dtsx";

            Application app = new Application();
         //   Package p = app.LoadPackage(pkg, null);
            app.SaveToXml("myXMLPackage.dtsx", package, null);
          
            // Now that the package is loaded, we can query on
            // its properties.
            int n = package.Configurations.Count;
            DtsProperty p2 = package.Properties["VersionGUID"];
            DTSProtectionLevel pl = package.ProtectionLevel;

            Console.WriteLine("Number of configurations = " + n.ToString());
            Console.WriteLine("VersionGUID = " + (string)p2.GetValue(package));
            Console.WriteLine("ProtectionLevel = " + pl.ToString());
            Console.Read();

        }
    }


    // <summary>
    // This class contains the definitions for multiple
    // connection managers.
    // </summary>
    public class CreateConnection
    {
        // Private data.
        private ConnectionManager ConMgr;

        // Class definition for OLE DB Provider.
        public void CreateOLEDBConnection(Package p)
        {
            ConMgr = p.Connections.Add("OLEDB");
            ConMgr.ConnectionString = "Provider=SQLOLEDB.1;" +
              "Integrated Security=SSPI;Initial Catalog=AdventureWorks;" +
              "Data Source=(local);";
            ConMgr.Name = "SSIS Connection Manager for OLE DB";
            ConMgr.Description = "OLE DB connection to the AdventureWorks database.";
        }
        public void CreateFileConnection(Package p)
        {
            ConMgr = p.Connections.Add("File");
            ConMgr.ConnectionString = @"\\<yourserver>\<yourfolder>\books.xml";
            ConMgr.Name = "SSIS Connection Manager for Files";
            ConMgr.Description = "Flat File connection";


        }
    }
}


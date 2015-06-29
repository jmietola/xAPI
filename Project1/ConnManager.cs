using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;


namespace Project1
{

    // <summary>
    // This class contains the definitions for multiple
    // connection managers.
    // </summary>
    public class ConnManager
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

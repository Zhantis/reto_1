using System.Data.SqlClient;
using ScrapperEy.DataAccess;
using ScrapperEy.Models;
using System.Data;

namespace ScrapperEy.Services
{
    public class OffShoreService
    {
        private readonly DatabaseManager _databaseManager;

        public OffShoreService(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        }

        public async Task<List<OffShoreEntity>> ListarOffShoreEntitiesByName(string entityName)
        {
            List<OffShoreEntity> offshoreEntities = new List<OffShoreEntity>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Entity_name", SqlDbType.VarChar) { Value = entityName}
            };

            try
            {
                DataTable dataTable = await _databaseManager.ExecuteStoredProcedureDataTable(StoredProcedure.LISTAR_OFFSHORE_BY_ENTITY, parameters);
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        OffShoreEntity offshoreEntity = new OffShoreEntity
                        {
                            OffShoreEntityId = Convert.ToInt32(row[0]),
                            EntityName = Convert.ToString(row[1]),
                            LinkedTo = Convert.ToString(row[2]),
                            DataFrom = Convert.ToString(row[3]),
                            Created = Convert.ToDateTime(row[4])
                        };
                        offshoreEntities.Add(offshoreEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR en Listar OffshoreEntities", ex);
            }

            return offshoreEntities;
        }

        
              

    }
}

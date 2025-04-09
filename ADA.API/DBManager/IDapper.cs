using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.DBManager
{
    public interface IDapper:IDisposable
    {
        DbConnection GetDbconnection();
        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        List<T> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        //Dapper.SqlMapper.GridReader GetMultiple(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, object obj);
        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        void Insert(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);




        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> GetMultipleObjects<T1, T2, T3, T4, T5, T6>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T3>> func3,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T4>> func4,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T5>> func5,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T6>> func6);
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> GetMultipleObjects<T1, T2, T3, T4, T5, T6, T7>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T3>> func3,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T4>> func4,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T5>> func5,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T6>> func6,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T7>> func7);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> GetMultipleObjects<T1, T2>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2
                                      );



        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetMultipleObjects<T1, T2, T3>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T3>> func3
                                      );
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> GetMultipleObjects<T1, T2, T3, T4>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T3>> func3,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T4>> func4
                                      );
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> GetMultipleObjects<T1, T2, T3, T4, T5>(string sql, object parameters,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T1>> func1,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T2>> func2,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T3>> func3,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T4>> func4,
                                        Func<Dapper.SqlMapper.GridReader, IEnumerable<T5>> func5);
    }
}

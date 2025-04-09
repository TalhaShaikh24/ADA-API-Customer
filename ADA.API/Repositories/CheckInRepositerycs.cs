using ADA.API.DBManager;
using ADA.API.IRepositories;
using ADAClassLibrary;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.Repositories
{
    public class CheckInRepositery : ICheckInRepositery
    {
        private readonly IDapper _dapper;

        public CheckInRepositery(IDapper dapper)
        {
            _dapper = dapper;
        }

        public CheckIn Add(CheckIn obj)
        {
            DynamicParameters parameters = new DynamicParameters();


            return _dapper.Insert<CheckIn>(@"[dbo].[usp_AddCheckIn]", parameters);
        }

        public List<CheckIn> GetAll()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<CheckIn>(@"[dbo].[GetAllCheckInSchedule]", parameters);
            return data;
        }

        public CheckInDTO GetAllReserVationByFlightId(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetMultipleObjects(@"[dbo].[GetAllReservationByFlightId]", parameters, gr => gr.Read<CheckIn>(), gr => gr.Read<Aircraft>());

            CheckInDTO listobj = new CheckInDTO();

            listobj.CheckIn = data.Item1.ToList();
            listobj.Aircraft = data.Item2.ToList();

            return listobj;
        }

        public List<CheckinCardDetails> GetCheckinCardDetails(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetAll<CheckinCardDetails>(@"[dbo].[GetCheckinCardDetails]", parameters);

            return data;
        }

        public object GetCheckInsDropDown()
        {
            throw new NotImplementedException();
        }

        public CheckInDTO GetFlightNoByUserId(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@UserId", id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetMultipleObjects(@"[dbo].[GetFlightNoByUserId]", parameters, gr => gr.Read<CheckIn>(), gr => gr.Read<Aircraft>(), gr => gr.Read<UserChekinDetails>());

            CheckInDTO listobj = new CheckInDTO();

            listobj.CheckIn = data.Item1.ToList();
            listobj.Aircraft = data.Item2.ToList();
            listobj.UserChekinDetails = data.Item3.ToList();

            return listobj;
        }

        public object GetuserDatabyRsvnId(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", Id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetMultipleObjects(@"[dbo].[GetuserDatabyRsvnId]", parameters, gr => gr.Read<CheckIn>(), gr => gr.Read<Aircraft>(), gr => gr.Read<Destination>(), gr => gr.Read<Country>(), gr => gr.Read<FlightDTOShortDetails>());

            return data;
        }

        public CheckIn Update(CheckIn obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", obj.RsvnID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CardNum", obj.CardNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@BagPcs", obj.BagPcs, DbType.String, ParameterDirection.Input);
            parameters.Add("@BagWt", obj.BagWt, DbType.String, ParameterDirection.Input);
            parameters.Add("@PaxWT", obj.PaxWT, DbType.String, ParameterDirection.Input);
            parameters.Add("@SeatNum", obj.SeatNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@RsvnStatus", obj.RsvnStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@UpdatedActionSource", obj.UpdatedActionSource, DbType.String, ParameterDirection.Input);
            parameters.Add("@UpdatedBy", obj.UpdatedBy, DbType.Int32, ParameterDirection.Input);

            return _dapper.Insert<CheckIn>(@"[dbo].[usp_CustomerCheckInUpdate]", parameters);

        }
    }
}

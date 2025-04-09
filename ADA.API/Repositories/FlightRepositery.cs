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
    public class FlightRepositery : IFlightRepositery
    {
        private readonly IDapper _dapper;

        public FlightRepositery(IDapper dapper)
        {
            _dapper = dapper;
        }

        public Flight Add(Flight obj)
        {
            throw new NotImplementedException();
        }

        public object GetAircraftType(string type)
        {
            throw new NotImplementedException();
        }

        public List<Flight> GetAll()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<Flight>(@"[dbo].[GetAllFlightSchedule]", parameters);
            return data;
        }

        public List<Register> GetAllMembersDetails(int FltId, List<int> MembersIds,string RegisterType)
        {
            DynamicParameters parameters = new DynamicParameters();

            DynamicParameters parameters2 = new DynamicParameters();

            List<Register> Listobj = new List<Register>();

           
            foreach (var item in MembersIds)
            {
                parameters.Add("@RegisterType", RegisterType, DbType.String, ParameterDirection.Input);
                parameters.Add("@MembersIds", item, DbType.Int32, ParameterDirection.Input);

                var data= _dapper.Get<Register>(@"[dbo].[usp_GetMembersInformation]", parameters);

                data.DocumentType = data.DocumentTypetxt.Split(',').ToList();

                data.FileName = data.FilePathNametxt.Split(',').ToList();

                parameters2.Add("@FltId", FltId, DbType.Int32, ParameterDirection.Input);

                parameters2.Add("@NationalityCode", data.Nationality, DbType.String, ParameterDirection.Input);

                var dataObj = _dapper.GetMultipleObjects(@"[dbo].[usp_GetFlightDesitnationAndNationality]", parameters2, gr => gr.Read<Flight>(), gr => gr.Read<Country>());

                data.Nationality = dataObj.Item2.Select(x => x.CountryName).FirstOrDefault();
                data.NationalityArabic = dataObj.Item2.Select(x => x.CountryNameAR).FirstOrDefault();
                data.NationalityCode = dataObj.Item2.Select(x => x.CountryAbbrev).FirstOrDefault();
                data.FlightDestinationName = dataObj.Item1.Select(x => x.Destination).FirstOrDefault();
                data.FlightDestinationNameArabic = dataObj.Item1.Select(x => x.DestinationArabic).FirstOrDefault();
                data.DestinationId = dataObj.Item1.Select(x => x.DestID).FirstOrDefault();

                Listobj.Add(data);

            }

            return Listobj;
        }


        public Flight GetByID(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FltID", id , DbType.Int32, ParameterDirection.Input);
            return _dapper.Get<Flight>(@"[dbo].[usp_getFlightByID]", parameters);
        }

        public object GetFlightDesitnationAndNationality(DestinationAndNationality obj)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FltId", obj.FltId, DbType.String, ParameterDirection.Input);
            parameters.Add("@NationalityCode", obj.NatinalityCode, DbType.String, ParameterDirection.Input);

            FlightDestionationAndNationality objList = new FlightDestionationAndNationality();

            var data = _dapper.GetMultipleObjects(@"[dbo].[usp_GetFlightDesitnationAndNationality]", parameters,gr=>gr.Read<Flight>(),gr=>gr.Read<Country>());

            objList.Flight = data.Item1.ToList();
            objList.Country = data.Item2.ToList();

            return objList;

        }

        public object GetFlightsDropDown()
        {
            DynamicParameters parameters = new DynamicParameters();
            var data = _dapper.GetMultipleObjects("[dbo].[GetAllDropDownCustomerPortal]", parameters, gr => gr.Read<Destination>(), gr => gr.Read<Country>(),gr => gr.Read<AddMemmberRestriction>());
            DropdownList obj = new DropdownList();
            obj.Destination = data.Item1.ToList();
            obj.Country = data.Item2.ToList();
            obj.MemberCount = data.Item3.FirstOrDefault();
            return obj;
        }

        public List<FlightStatus>  GetFlightStatusByFlightId(string FNo)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FltNumber", FNo, DbType.String, ParameterDirection.Input);

            return _dapper.GetAll<FlightStatus>(@"[dbo].[GetFlightStatusByFlightId]", parameters);
        }
        public List<Flight> GetSearchFlight(SearchFlight obj)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@From", obj.From, DbType.String, ParameterDirection.Input);
            parameters.Add("@To", obj.To, DbType.String, ParameterDirection.Input);
            parameters.Add("@FromDate", obj.FromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@ToDate", obj.ToDate, DbType.DateTime, ParameterDirection.Input);
           var data= _dapper.GetAll<Flight>(@"[dbo].[usp_SearchFlight]", parameters);
            return data;
        }

        public Flight Update(Flight obj)
        {
            throw new NotImplementedException();
        }
    }
}

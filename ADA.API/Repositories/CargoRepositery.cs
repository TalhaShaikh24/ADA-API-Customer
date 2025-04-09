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
    public class CargoRepositery : ICargoRepositery
    {
        private readonly IDapper _dapper;

        public CargoRepositery(IDapper dapper)
        {
            _dapper = dapper;
        }

        public Cargo Add(Cargo obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@DateTimeRcvd", obj.DateTimeRcvd, DbType.DateTime2, ParameterDirection.Input);
            parameters.Add("@FrtDesc", obj.FrtDesc, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtWt", obj.FrtWt, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtDest", obj.FrtDest, DbType.String, ParameterDirection.Input);
            parameters.Add("@ShipperName", obj.ShipperName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ShipperRefNum", obj.ShipperRefNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtRemarks", obj.FrtRemarks, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtSerial", obj.FrtSerial, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtPcs", obj.FrtPcs, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ReceiverID", obj.CreatedBy, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtDirection", obj.FrtDirection, DbType.String, ParameterDirection.Input);
            parameters.Add("@BarCode", obj.BarCode, DbType.String, ParameterDirection.Input);

            return _dapper.Insert<Cargo>(@"[dbo].[usp_AddCargo]", parameters);
        }

        public List<Cargo> GetAll()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<Cargo>(@"[dbo].[usp_GetAllCargo]", parameters);

            return data;
        }

        public Cargo GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public object GetCargosDropDown()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<CargoDestination>("[dbo].[usp_GetALLCargoDropDown]", parameters);

            return data;
        }

        public List<CargoGetFlightByDate> GetAllFlightbyDate(CargoGetFlightByDate obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@DateTime", obj.FlightDateTime, DbType.DateTime, ParameterDirection.Input);

            var data = _dapper.GetAll<CargoGetFlightByDate>("[dbo].[usp_GetAllFlightbyDate]", parameters);

            return data;
        }

        public Cargo Update(Cargo obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@FrtID", obj.FrtID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtDesc", obj.FrtDesc, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtWt", obj.FrtWt, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtDest", obj.FrtDest, DbType.String, ParameterDirection.Input);
            parameters.Add("@ShipperName", obj.ShipperName, DbType.String, ParameterDirection.Input);
            parameters.Add("@ShipperRefNum", obj.ShipperRefNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtRemarks", obj.FrtRemarks, DbType.String, ParameterDirection.Input);
            parameters.Add("@FrtSerial", obj.FrtSerial, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtPcs", obj.FrtPcs, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@FrtDirection", obj.FrtDirection, DbType.String, ParameterDirection.Input);
            parameters.Add("@BarCode", obj.BarCode, DbType.String, ParameterDirection.Input);

            return _dapper.Insert<Cargo>(@"[dbo].[usp_UpdateCargo]", parameters);
        }

        public List<CargoFlightDetails> GetFlightDetailsbyId(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.GetAll<CargoFlightDetails>("[dbo].[usp_GetCargoFlightDetailsById]", parameters);

            return data;
        }

        public List<Cargo> CargoReceivedMultiUpdate(CargoMultiUpdate obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            List<Cargo> listobj = new List<Cargo>();

            foreach (var item in obj.CargoIds)
            {
                parameters.Add("@FltID", obj.FltID, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@Forwarding", obj.CreateBy, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@CargoId", item, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@MinfestColor", obj.MinfestColor, DbType.String, ParameterDirection.Input);
                var data = _dapper.Get<Cargo>("[dbo].[usp_CargoReceivedMultiUpdate]", parameters);
                listobj.Add(data);

            }

            return listobj;

        }


        public List<Cargo> CargoLoadedMultiUpdate(CargoMultiUpdate obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            List<Cargo> listobj = new List<Cargo>();

            foreach (var item in obj.CargoIds)
            {
                parameters.Add("@CargoId", item, DbType.Int32, ParameterDirection.Input);

                var data = _dapper.Get<Cargo>("[dbo].[usp_CargoLOADEDMultiUpdate]", parameters);

                listobj.Add(data);

            }

            return listobj;

        }

        public List<int> CargoReceiveDeletedMultiple(CargoMultiUpdate obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            List<int> listobj = new List<int>();

            foreach (var item in obj.CargoIds)
            {
                parameters.Add("@CargoId", item, DbType.Int32, ParameterDirection.Input);

                var data = _dapper.Get<int>("[dbo].[usp_CargoReceiveMultiDelete]", parameters);

                listobj.Add(data);

            }

            return listobj;
        }

        public List<CargoStaff> GetAllCargoStaff()
        {
            DynamicParameters parameters = new DynamicParameters();

            var data = _dapper.GetAll<CargoStaff>(@"[dbo].[usp_GetAllCargoStaff]", parameters);

            return data;
        }

        public CargoStaff AddStaffCargo(CargoStaff obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@EmpNum", obj.EmpNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffSurname", obj.StaffSurname, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffName", obj.StaffName, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffPwd", obj.StaffPwd, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffActive", obj.StaffActive, DbType.Boolean, ParameterDirection.Input);


            return _dapper.Insert<CargoStaff>(@"[dbo].[usp_AddStaffCargo]", parameters);
        }

        public CargoStaff UpdateStaffCargo(CargoStaff obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@StaffID", obj.StaffID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EmpNum", obj.EmpNum, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffSurname", obj.StaffSurname, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffName", obj.StaffName, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffPwd", obj.StaffPwd, DbType.String, ParameterDirection.Input);
            parameters.Add("@StaffActive", obj.StaffActive, DbType.Boolean, ParameterDirection.Input);

            return _dapper.Insert<CargoStaff>(@"[dbo].[usp_UpdateStaffCargo]", parameters);
        }

        public CargoStaff GetStaffCargoByID(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@StaffID", id, DbType.Int32, ParameterDirection.Input);

            var data = _dapper.Get<CargoStaff>(@"[dbo].[usp_GetCargStaffByID]", parameters);

            return data;
        }

        public CargoExportDataFilter GetCargoExportDataDetailById(int id)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            return _dapper.Get<CargoExportDataFilter>(@"[dbo].[usp_GetCargoExportDataDetailById]", parameters);
        }

        public CargoExportDataFilter UpdateStatusCargoExportData(CargoExportDataFilter obj)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@FrtID", obj.FrtID, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CargoStatus", obj.CargoStatus, DbType.String, ParameterDirection.Input);

            return _dapper.Insert<CargoExportDataFilter>(@"[dbo].[usp_UpdateCargoStatus]", parameters);
        }

        public List<Cargo> GetAllCargoStatusByBarcode(string Barcode)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Barcode", Barcode, DbType.String, ParameterDirection.Input);
            var data =_dapper.GetAll<Cargo>(@"[dbo].[usp_GetAllCargoStatusByBarcode]", parameters);
            return data;
        }
    }
}

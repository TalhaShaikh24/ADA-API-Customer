using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IRepositories
{
    public interface ICargoRepositery
    {
        Cargo Add(Cargo obj);
        CargoExportDataFilter UpdateStatusCargoExportData(CargoExportDataFilter obj);
        Cargo Update(Cargo obj);
        CargoStaff AddStaffCargo(CargoStaff obj);
        CargoStaff UpdateStaffCargo(CargoStaff obj);
        CargoStaff GetStaffCargoByID(int id);
        CargoExportDataFilter GetCargoExportDataDetailById(int id);
        List<Cargo> GetAll();
        List<Cargo> GetAllCargoStatusByBarcode(string Barcode);
        List<CargoStaff> GetAllCargoStaff();
        List<Cargo> CargoReceivedMultiUpdate(CargoMultiUpdate obj);
        List<Cargo> CargoLoadedMultiUpdate(CargoMultiUpdate obj);
        List<int> CargoReceiveDeletedMultiple(CargoMultiUpdate obj);
        List<CargoFlightDetails> GetFlightDetailsbyId(int id);
        Cargo GetByID(int id);
        object GetCargosDropDown();

        List<CargoGetFlightByDate> GetAllFlightbyDate(CargoGetFlightByDate obj);
    }
}

using ADAClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IServices
{
    public interface ICargoService : IService<Cargo>
    {
        Cargo Add(Cargo obj);
        Cargo Update(Cargo obj);

        List<Cargo> GetAllCargoStatusByBarcode(string Barcode);
        CargoExportDataFilter UpdateStatusCargoExportData(CargoExportDataFilter obj);
        CargoStaff AddStaffCargo(CargoStaff obj);
        CargoStaff UpdateStaffCargo(CargoStaff obj);
        CargoStaff GetStaffCargoByID(int id);
        Cargo GetCargoBtID(int id);
        CargoExportDataFilter GetCargoExportDataDetailById(int id);
        object GetCargosDropDown();
        List<CargoStaff> GetAllCargoStaff();
        List<Cargo> CargoReceivedMultiUpdate(CargoMultiUpdate obj);
        List<Cargo> CargoLoadedMultiUpdate(CargoMultiUpdate obj);
        List<int> CargoReceiveDeletedMultiple(CargoMultiUpdate obj);
        List<CargoFlightDetails> GetFlightDetailsbyId(int id);
        List<CargoGetFlightByDate> GetAllFlightbyDate(CargoGetFlightByDate obj);

    }
}
